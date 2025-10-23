# Face Recognition Liveness Challenge

Ini adalah solusi untuk **Face Recognition Liveness Challenge**. Aplikasi ini mengimplementasikan deteksi wajah dan deteksi *liveness* (kehidupan) untuk mencegah upaya *spoofing*.

Aplikasi ini menggunakan arsitektur *microservice* yang diorkestrasi oleh `docker-compose` dan terdiri dari dua komponen utama:

1.  **Backend API (`webapi`)**: Sebuah aplikasi .NET 7 yang menyajikan Web UI statis (Razor Page) dan menangani permintaan dari *frontend*.
2.  **Vision Service (`vision`)**: Sebuah service Python (Flask) yang menjalankan model *machine learning* (face_recognition, OpenCV) untuk melakukan pengenalan wajah dan deteksi *liveness*.

## Fitur

* **Face Recognition**: Mengidentifikasi wajah yang ditangkap dari webcam.
* **Liveness Detection**: Membedakan antara wajah asli dan upaya *spoofing* (misalnya foto) menggunakan analisis gerakan sederhana.
* **Web UI**: Antarmuka berbasis web sederhana untuk interaksi webcam.
* **Multi-Face Support**: Mendukung deteksi beberapa wajah dalam satu frame.
* **Containerized**: Seluruh aplikasi di-container-kan menggunakan Docker dan `docker-compose`.

## Prasyarat

* [Docker](https://www.docker.com/get-started)
* [Docker Compose](https://docs.docker.com/compose/install/) (biasanya sudah termasuk dalam Docker Desktop)

## Instalasi & Menjalankan Proyek

1.  **Clone Repository**
    ```bash
    git clone [URL_GITHUB_ANDA]
    cd [NAMA_FOLDER_REPO]
    ```

2.  **(PENTING) Tambahkan Wajah yang Dikenali**
    *Service* `vision` perlu "belajar" wajah siapa yang harus dikenali.
    * Buat folder baru: `vision/known_faces`
    * Tambahkan file gambar (misal: `.jpg`, `.png`) ke dalam folder `vision/known_faces`.
    * Nama file akan digunakan sebagai label. Contoh: `vision/known_faces/Tek Kheng.jpg` akan dikenali sebagai "Tek Kheng".

3.  **Build Docker Images**
    Buka terminal di *root* folder (tempat file `docker-compose.yml` berada) dan jalankan:
    ```bash
    docker-compose build
    ```
    **Peringatan:** Proses *build* service `vision` akan **memakan waktu lama** (bisa 5-15 menit) karena perlu meng-compile `dlib` dan dependensi C++ lainnya.

4.  **Jalankan Services**
    Setelah proses *build* selesai, jalankan aplikasi:
    ```bash
    docker-compose up -d
    ```
    Opsi `-d` (detached) akan menjalankan kontainer di latar belakang.

## Cara Menggunakan

1.  Buka browser Anda dan akses aplikasi di:
    **`http://localhost:8080`**

2.  Klik tombol **"Start"** untuk mengaktifkan webcam Anda. Anda mungkin perlu memberikan izin pada browser.

3.  Posisikan wajah Anda di depan kamera.

4.  Klik tombol **"Capture & Recognize"**.

5.  Aplikasi akan mengambil gambar, mengirimkannya ke *backend* untuk diproses, dan menampilkan hasil JSON mentah di bagian bawah halaman.

## Menghentikan Aplikasi

Untuk menghentikan dan menghapus kontainer, jalankan:

```bash
docker-compose down