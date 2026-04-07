# 📕 SoulViet Backend - Development Cheat Sheet

Tài liệu này cung cấp các lệnh (CLI) và quy trình chuẩn để phát triển dự án **SoulViet Backend**. 
**LƯU Ý QUAN TRỌNG:** Luôn mở Terminal ở thư mục gốc của dự án (nơi chứa file `SoulViet.slnx`).

---

## 1. 🚀 Khởi động & Chạy ứng dụng

### 1.1. Khởi động Infrastructure (Docker)
Dự án sử dụng Docker để chạy PostgreSQL, Redis và RabbitMQ. Đảm bảo Docker Desktop đã mở:
```bash
# Khởi chạy các service chạy ngầm
docker-compose up -d

# Dừng các service
docker-compose down

# Xem log của các service
docker-compose logs -f
```

### 1.2. Chạy API Backend (Development)
Có 2 cách để chạy project `SoulViet.API`:
```bash
# Cách 1: Chạy có Hot-Reload (Tự động cập nhật khi lưu code - Khuyên dùng)
dotnet watch --project src/SoulViet.API/SoulViet.API.csproj run

# Cách 2: Chạy bình thường (Không hot-reload)
dotnet run --project src/SoulViet.API/SoulViet.API.csproj
```

### 1.3. Xây dựng (Build) & Dọn dẹp (Clean)
```bash
# Build toàn bộ Solution để check lỗi
dotnet build SoulViet.slnx

# Dọn dẹp các file build cũ (obj/bin)
dotnet clean SoulViet.slnx
```

---

## 2. 🗄️ Quản lý Entity Framework Core & Database

Tất cả cấu hình Database nằm ở `AppMigrationDbContext` trong project `SoulViet.API`. 

### 2.1. Tạo Migration mới
Mỗi khi bạn thêm/sửa/xóa class Entity trong các Module (Marketplace, Social, SoulMap...), chạy lệnh sau (thay `TenMigration`):
```bash
dotnet ef migrations add <TenMigration> --context AppMigrationDbContext --project src/SoulViet.API --startup-project src/SoulViet.API
```

### 2.2. Áp dụng sửa đổi xuống Database
```bash
dotnet ef database update --context AppMigrationDbContext --project src/SoulViet.API --startup-project src/SoulViet.API
```

### 2.3. Hủy bỏ Migration vừa tạo (Chưa update DB)
Nếu gõ sai, hoặc file entity bị dính lỗi, cần xóa migration cuối cùng:
```bash
dotnet ef migrations remove --context AppMigrationDbContext --project src/SoulViet.API --startup-project src/SoulViet.API                
```

### 2.4. Xóa trắng Database (Nguy hiểm)
Dùng khi database gặp lỗi khó sửa và muốn khởi tạo lại toàn bộ cấu trúc:
```bash
dotnet ef database drop --context AppMigrationDbContext --project src/SoulViet.API --startup-project src/SoulViet.API
```
*(Hệ thống sẽ hỏi bạn chắc chắn muốn Drop không, gõ `y` để đồng ý).*
---

## 3. 📦 Quản lý Nuget & Project Reference

### 3.1. Cài đặt Package (Nuget)
Thêm thư viện vào một module bất kỳ. Chẳng hạn thêm `Newtonsoft.Json` vào dự án `Social.Infrastructure`:
```bash
dotnet add src/Modules/SoulViet.Modules.Social/SoulViet.Modules.Social.csproj package Newtonsoft.Json
```

### 3.2. Cấu hình Tham chiếu Project (Reference)
Nếu module `API` cần sử dụng logic của module `Marketplace`:
```bash
dotnet add src/SoulViet.API/SoulViet.API.csproj reference src/Modules/SoulViet.Modules.Marketplace/SoulViet.Modules.Marketplace.csproj
```

---

## 4. 🌱 Seed Dữ Liệu Ban Đầu
Dự án được cấu hình tự động đọc file CSV trong folder `data` để nạp vào DB cho schema SoulMap.

- **Nơi cấu hình:** `src/SoulViet.API/Program.cs`
- **Cách sử dụng:**
  - Ở gần cuối file `Program.cs`, bạn sẽ thấy đoạn `using (var scope = app.Services.CreateScope())`.
  - Bạn có thể **Bật** *(bỏ comment)* hoặc **Tắt** *(thêm `//` vào đầu dòng)* lệnh `await soulMapSeeder.SeedDataAsync(...)` tùy vào nhu cầu. Đôi khi nên tắt đi để tăng tốc độ start app nếu database đã có sẵn dữ liệu.

---

## 5. 🏗️ Cấu trúc dự án (Tóm tắt)
- `data/`: Chứa các file dữ liệu (ví dụ `.csv`) để mồi (seed) vào hệ thống.
- `docs/`: Chứa các tài liệu thiết kế hệ thống, conventions, workflow.
- `src/SoulViet.API/`: Host layer chính, kết nối các service, chứa file cấu hình appsettings, docker, Swagger...
- `src/SoulViet.Shared.*/`: Định nghĩa các cấu trúc dùng chung toàn cục (Entities core như User/Role, Interfaces chung).
- `src/Modules/`: Chứa các Modules (Bounded Contexts) độc lập (như Marketplace, Social, SoulMap) theo kiến trúc Clean Architecture cho từng Module.

