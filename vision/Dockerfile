FROM python:3.11-slim

# # Install system dependencies untuk dlib dan face_recognition
# RUN apt-get update && apt-get install -y \
#     build-essential cmake pkg-config \
#     libopenblas-dev liblapack-dev libboost-all-dev \
#     libjpeg-dev libpng-dev libtiff-dev libxext6 libsm6 libxrender-dev libglib2.0-0 && \
#     rm -rf /var/lib/apt/lists/*

# Install system dependencies untuk dlib dan face_recognition
RUN apt-get update && apt-get install -y \
    build-essential cmake pkg-config \
    libopenblas-dev liblapack-dev libboost-all-dev \
    libjpeg-dev libpng-dev libtiff-dev libxext6 libsm6 libxrender-dev libglib2.0-0 \
    libgl1 \
    && \
    rm -rf /var/lib/apt/lists/*

WORKDIR /app

# Salin requirements
COPY requirements.txt .

# Install Python dependencies
RUN pip install --upgrade pip && pip install -r requirements.txt

COPY . .

EXPOSE 5000
CMD ["python", "app.py"]

