# 1. Tạo thư mục gốc và Solution
mkdir SoulViet.Backend
cd SoulViet.Backend
dotnet new sln -n SoulViet

# 2. Tạo project API (Điểm chạy chính của ứng dụng)
dotnet new webapi -n SoulViet.API -o src/SoulViet.API

# 3. Tạo project Shared Infrastructure (Dùng chung cho toàn hệ thống)
dotnet new classlib -n SoulViet.Shared.Infrastructure -o src/SoulViet.Shared.Infrastructure

# 4. Tạo các project Modules (Mỗi Module là 1 Class Library)
dotnet new classlib -n SoulViet.Modules.Marketplace -o src/SoulViet.Modules.Marketplace
dotnet new classlib -n SoulViet.Modules.SoulMap -o src/SoulViet.Modules.SoulMap
dotnet new classlib -n SoulViet.Modules.Social -o src/SoulViet.Modules.Social

# 5. Thêm tất cả projects vào Solution
dotnet sln add src/SoulViet.API/SoulViet.API.csproj
dotnet sln add src/SoulViet.Shared.Infrastructure/SoulViet.Shared.Infrastructure.csproj
dotnet sln add src/SoulViet.Modules.Marketplace/SoulViet.Modules.Marketplace.csproj
dotnet sln add src/SoulViet.Modules.SoulMap/SoulViet.Modules.SoulMap.csproj
dotnet sln add src/SoulViet.Modules.Social/SoulViet.Modules.Social.csproj

# 6. Thiết lập tham chiếu (References) cho project API
dotnet add src/SoulViet.API/SoulViet.API.csproj reference src/SoulViet.Shared.Infrastructure/SoulViet.Shared.Infrastructure.csproj
dotnet add src/SoulViet.API/SoulViet.API.csproj reference src/SoulViet.Modules.Marketplace/SoulViet.Modules.Marketplace.csproj
dotnet add src/SoulViet.API/SoulViet.API.csproj reference src/SoulViet.Modules.SoulMap/SoulViet.Modules.SoulMap.csproj
dotnet add src/SoulViet.API/SoulViet.API.csproj reference src/SoulViet.Modules.Social/SoulViet.Modules.Social.csproj