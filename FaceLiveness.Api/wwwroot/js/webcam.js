const video = document.getElementById('video');
const canvas = document.getElementById('canvas');
const startBtn = document.getElementById('start');
const captureBtn = document.getElementById('capture');

const resultContainer = document.getElementById('result-container');

startBtn.onclick = async () => {
    try {
        const stream = await navigator.mediaDevices.getUserMedia({ video: true, audio: false });
        video.srcObject = stream;
        await video.play();
    } catch (e) {
        alert("Could not access webcam: " + e.message);
    }
};

function getBase64FromVideo() {
    const ctx = canvas.getContext('2d');
    canvas.width = video.videoWidth || 640;
    canvas.height = video.videoHeight || 480;
    ctx.drawImage(video, 0, 0, canvas.width, canvas.height);
    return canvas.toDataURL('image/jpeg', 0.9);
}

captureBtn.onclick = async () => {
    const dataUrl = getBase64FromVideo();
    resultContainer.innerHTML = '<p>Processing...</p>';

    try {
        const res = await fetch('/api/recognition/recognize', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ image: dataUrl, multi: document.getElementById('multi').checked })
        });

        const json = await res.json();

        const liveness = json.liveness;
        const liveText = liveness.live ? 'Yes' : 'No';
        const liveColor = liveness.live ? '#28a745' : '#dc3545';

        let html = `<h3>Recognition Result</h3>`;
        html += `<p><strong>Face Count:</strong> ${json.faceCount}</p>`;
        html += `<p><strong>Is Live?</strong> <span style="color: ${liveColor}; font-weight: bold;">${liveText}</span> (Raw: ${liveness.raw.toFixed(2)})</p>`;

        if (json.faces && json.faces.length > 0) {
            html += `<h4>Detected Faces:</h4>`;
            html += '<ul>';
            json.faces.forEach(face => {
                html += `<li>`;
                html += `<strong>Name:</strong> ${face.name}<br/>`;

                const distanceText = face.distance ? face.distance.toFixed(4) : 'N/A';
                html += `<strong>Distance:</strong> ${distanceText}<br/>`;

                if (face.box) {
                    html += `<strong>Box:</strong> (T: ${face.box.top}, L: ${face.box.left}, R: ${face.box.right}, B: ${face.box.bottom})`;
                }

                html += `</li>`;
            });
            html += '</ul>';
        } else if (json.faceCount > 0) {
            html += `<p>Wajah terdeteksi, namun tidak dikenali.</p>`;
        } else {
            html += `<p>Tidak ada wajah yang terdeteksi.</p>`;
        }
        resultContainer.innerHTML = html;

    } catch (e) {
        resultContainer.innerHTML = `<p style="color: red; font-weight: bold;">Error: ${e.message}</p>`;
    }
};