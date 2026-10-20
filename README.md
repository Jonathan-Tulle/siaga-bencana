# Siaga Bencana (Simulasi Gempa Bumi)

Game edukasi mitigasi gempa bumi sederhana berbasis Unity URP untuk simulasi evakuasi dan perlindungan diri (drop, cover, hold on) sesuai arahan dasar BNPB.

## Cara Buka Proyek di Unity
1. Buka Unity Hub.
2. Klik tombol Add -> Add project from disk -> pilih folder proyek ini (`Siaga Bencana`).
3. Buka menggunakan Unity 6 (versi 6000.5.10f1 atau yang kompatibel).
4. Di tab Project, buka scene di: `Assets/Scenes/SampleScene.unity`.
5. Jika objek di scene belum terpasang atau ingin mereset ruangan, klik menu di toolbar atas:
   `Tools > Siaga Bencana > Generate Level 1 (Rumah - Gempa)` lalu klik `Buat Sekarang`.
6. Klik tombol Play untuk mencoba gamenya.

## Kontrol
- Gerak: W, A, S, D atau tombol Panah
- Lari: Tahan Left Shift
- Berlindung di bawah meja: C
- Pilih jawaban gempa: Tombol 1 / 2 atau klik tombol di layar
- Pause game: Escape
- Lanjut dialog tutorial: Space atau Enter

## Alur Permainan Level 1
- Persiapan (15 detik): Pemain bisa keliling ruangan di dalam rumah, mengenali letak meja kokoh dan pintu keluar.
- Gempa (12 detik): Gempa mulai berguncang. Pilih opsi mitigasi pada pop-up pertanyaan, lalu segera berlindung di bawah meja (tekan C).
- Evakuasi: Setelah gempa reda, segera lari keluar rumah ke area hijau di halaman (titik kumpul) untuk menyelesaikan level.

## Struktur File Penting
- `Assets/_Project/Scripts/Core`: GameManager, GameEvents, Singleton.
- `Assets/_Project/Scripts/Gameplay`: PlayerController, EarthquakeController, CoverZone, SafeZone.
- `Assets/_Project/Scripts/UI`: UIManager (tutorial, HUD timer, pilihan keputusan, debrief).
- `Assets/_Project/Scripts/Editor`: Level1GreyboxBuilder (script untuk generate layout level).
- `Assets/Scenes`: SampleScene.unity (scene utama permainan).
