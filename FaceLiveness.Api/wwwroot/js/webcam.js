const video = document.getElementById('video');
const canvas = document.getElementById('canvas');
const startBtn = document.getElementById('start');
const captureBtn = document.getElementById('capture');
const resultPre = document.getElementById('result');

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
    resultPre.textContent = "Processing...";
    try {
        const res = await fetch('/api/recognition/recognize', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ image: dataUrl, multi: document.getElementById('multi').checked })
        });
        const json = await res.json();
        resultPre.textContent = JSON.stringify(json, null, 2);
    } catch (e) {
        resultPre.textContent = "Error: " + e.message;
    }
};
