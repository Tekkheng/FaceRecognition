from flask import Flask, request, jsonify
from flask_cors import CORS
import face_recognition
import cv2
import numpy as np
import os
import base64
import time

app = Flask(__name__)
CORS(app)

KNOWN_DIR = "known_faces"
known_encodings = []
known_names = []

def load_known_faces():
    global known_encodings, known_names
    known_encodings = []
    known_names = []
    if not os.path.exists(KNOWN_DIR):
        os.makedirs(KNOWN_DIR)
    for filename in os.listdir(KNOWN_DIR):
        if filename.lower().endswith(('.png', '.jpg', '.jpeg')):
            path = os.path.join(KNOWN_DIR, filename)
            image = face_recognition.load_image_file(path)
            enc = face_recognition.face_encodings(image)
            if len(enc) > 0:
                known_encodings.append(enc[0])
                known_names.append(os.path.splitext(filename)[0])
                print(f"Loaded {filename}")
            else:
                print(f"No face in {filename}")

class LivenessChecker:
    def __init__(self):
        self.prev_gray = None
        self.history = []

    def check(self, frame_bgr):
        gray = cv2.cvtColor(frame_bgr, cv2.COLOR_BGR2GRAY)
        if self.prev_gray is None:
            self.prev_gray = gray
            self.history.append(0.0)
            return {"score": 1.0, "live": True, "raw": 0.0}
        diff = cv2.absdiff(self.prev_gray, gray)
        mean_diff = float(np.mean(diff))
        self.history.append(mean_diff)
        if len(self.history) > 5:
            self.history.pop(0)
        self.prev_gray = gray
        smoothed = float(np.mean(self.history))
        live = smoothed > 2.5
        score = min(1.0, smoothed / 50.0)
        return {"score": score, "live": bool(live), "raw": smoothed}

liveness = LivenessChecker()

def decode_base64_image(data_b64):
    if ',' in data_b64:
        data_b64 = data_b64.split(',')[1]
    img_bytes = base64.b64decode(data_b64)
    arr = np.frombuffer(img_bytes, np.uint8)
    img = cv2.imdecode(arr, cv2.IMREAD_COLOR)
    return img

@app.route("/health", methods=["GET"])
def health():
    return jsonify({"status": "ok"})

@app.route("/recognize", methods=["POST"])
def recognize():
    data = request.get_json()
    if not data or "image" not in data:
        return jsonify({"error":"image required"}), 400

    img = decode_base64_image(data["image"])
    multi = data.get("multi", True)
    live_info = liveness.check(img)

    rgb = cv2.cvtColor(img, cv2.COLOR_BGR2RGB)
    face_locations = face_recognition.face_locations(rgb)
    face_encodings = face_recognition.face_encodings(rgb, face_locations)
    results = []
    for (top, right, bottom, left), encoding in zip(face_locations, face_encodings):
        matches = face_recognition.compare_faces(known_encodings, encoding, tolerance=0.5)
        name = "Unknown"
        dist = None
        if True in matches:
            idx = matches.index(True)
            name = known_names[idx]
            dist = float(np.linalg.norm(known_encodings[idx] - encoding))
        results.append({
            "name": name,
            "distance": dist,
            "box": {"top": int(top), "right": int(right), "bottom": int(bottom), "left": int(left)}
        })
        if not multi:
            break

    return jsonify({"faces": results, "liveness": live_info, "face_count": len(results)}), 200

if __name__ == "__main__":
    print("Loading known faces...")
    load_known_faces()
    print("Starting Vision on 0.0.0.0:5000")
    app.run(host="0.0.0.0", port=5000)
