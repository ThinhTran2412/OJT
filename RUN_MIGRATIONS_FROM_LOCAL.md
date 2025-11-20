# 🚀 Chạy Migrations Từ Local Đến Render Database

## 📋 Yêu Cầu

- .NET SDK 8.0 hoặc cao hơn
- Entity Framework Core Tools
- Connection string đến Render database (External URL)

## ⚙️ Cấu Hình Connection String

### Bước 1: Cập nhật `appsettings.Development.json`

**File:** `OJT_Laboratory_Project/IAM_Service/IAM_Service.API/appsettings.Development.json`

**Thêm connection string đến Render database:**

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=dpg-d4fcsm95pdvs73ader70-a.singapore-postgres.render.com;Port=5432;Database=laboratory_service;Username=laboratory_service_user;Password=geeqHh8B6xA8oQNkNHw0K0AoJKSZhji2;SSL Mode=Require"
  },
  "Database": {
    "Schema": "iam_service"
  }
}
```

**⚠️ Lưu ý:**
- Dùng **External URL** (`.singapore-postgres.render.com`)
- Thêm `SSL Mode=Require` để kết nối qua internet
- Connection string này chỉ dùng để chạy migrations, không commit vào Git (nếu có password)

### Bước 2: Kiểm Tra EF Core Tools

Mở Command Prompt hoặc PowerShell trong folder `OJT_Laboratory_Project/IAM_Service`:

```bash
# Kiểm tra EF Core Tools đã cài chưa
dotnet tool list -g | findstr "dotnet-ef"

# Nếu chưa có, cài đặt:
dotnet tool install --global dotnet-ef
```

## 🚀 Chạy Migrations

### Cách 1: Sử dụng Script Có Sẵn (KHUYẾN NGHỊ)

**File:** `OJT_Laboratory_Project/IAM_Service/Scripts/migration_update.bat`

1. **Mở file `appsettings.Development.json`** và cập nhật connection string như ở trên
2. **Double-click vào file `Scripts/migration_update.bat`**
3. **Đợi migrations chạy xong**

Script sẽ tự động:
- ✅ Kiểm tra và cài EF Core Tools
- ✅ Restore packages
- ✅ Build solution
- ✅ Chạy migrations

### Cách 2: Chạy Thủ Công

1. **Mở Command Prompt hoặc PowerShell:**
   ```bash
   cd F:\OJT_Assigment\OJT_Project\Deploy\OJT_Laboratory_Project\IAM_Service
   ```

2. **Kiểm tra connection string:**
   - Đảm bảo `appsettings.Development.json` có connection string đúng

3. **Chạy migrations:**
   ```bash
   dotnet ef database update \
     --project IAM_Service.Infrastructure/IAM_Service.Infrastructure.csproj \
     --startup-project IAM_Service.API/IAM_Service.API.csproj \
     --configuration Development
   ```

   **Lưu ý:** `--configuration Development` để dùng `appsettings.Development.json`

4. **Kiểm tra kết quả:**
   - Nếu thành công: `Done.` hoặc `No migrations were applied.`
   - Nếu có lỗi: Xem error message

## ✅ Kiểm Tra Sau Khi Chạy Migrations

### Kiểm Tra Tables Đã Được Tạo

**Kết nối Database trên Render Dashboard:**
1. Vào Database → **"Connect"** hoặc dùng psql
2. Chạy SQL:

```sql
-- Kiểm tra schema có tồn tại không
SELECT schema_name 
FROM information_schema.schemata 
WHERE schema_name = 'iam_service';

-- Xem tất cả tables trong schema iam_service
SELECT table_name 
FROM information_schema.tables 
WHERE table_schema = 'iam_service';
```

**Tables phải có:**
- ✅ `Users`
- ✅ `Roles`
- ✅ `Privileges`
- ✅ `UserRoles` (hoặc junction table)
- ✅ `__EFMigrationsHistory` (EF Core migrations tracking)

### Test Lại API

1. **Vào `https://front-end-fnfs.onrender.com/register`**
2. **Điền form và submit**
3. **Kiểm tra:**
   - ✅ `201 Created` → Thành công! Tables đã được tạo
   - ❌ `500` → Vẫn còn lỗi, kiểm tra logs tiếp

## 🔍 Troubleshooting

### Lỗi: "Connection refused" hoặc "Unable to connect"

**Nguyên nhân:** Connection string không đúng hoặc database không accessible

**Giải pháp:**
1. Kiểm tra External URL có đúng không
2. Kiểm tra database status trên Render Dashboard
3. Thử dùng connection string với `SSL Mode=Require;Trust Server Certificate=true`

**Connection string với SSL:**
```
Host=dpg-d4fcsm95pdvs73ader70-a.singapore-postgres.render.com;Port=5432;Database=laboratory_service;Username=laboratory_service_user;Password=geeqHh8B6xA8oQNkNHw0K0AoJKSZhji2;SSL Mode=Require;Trust Server Certificate=true
```

### Lỗi: "No migrations found"

**Nguyên nhân:** Migrations files chưa có

**Giải pháp:**
- Kiểm tra folder `IAM_Service.Infrastructure/Migrations/` có files `.cs` chưa
- Nếu chưa có, tạo migrations:
  ```bash
  dotnet ef migrations add InitialCreate \
    --project IAM_Service.Infrastructure/IAM_Service.Infrastructure.csproj \
    --startup-project IAM_Service.API/IAM_Service.API.csproj
  ```

### Lỗi: "Permission denied" hoặc "Access denied"

**Nguyên nhân:** User không có quyền tạo schema/tables

**Giải pháp:**
1. Kiểm tra database user permissions trên Render
2. Đảm bảo user có quyền `CREATE SCHEMA` và `CREATE TABLE`

### Lỗi: "Schema already exists" hoặc "Table already exists"

**Nguyên nhân:** Migrations đã được chạy trước đó

**Giải pháp:**
- Không cần làm gì, tables đã tồn tại rồi
- Nếu muốn reset, drop schema và chạy lại migrations (CẨN THẬN!)

## ⚠️ Lưu Ý Quan Trọng

1. **Connection String Security:**
   - Không commit `appsettings.Development.json` có password vào Git (nếu chưa có `.gitignore`)
   - Nên dùng environment variable hoặc User Secrets cho production

2. **Migrations Chỉ Chạy Một Lần:**
   - Migrations chỉ cần chạy một lần (hoặc khi có migration mới)
   - EF Core sẽ track migrations đã apply trong table `__EFMigrationsHistory`

3. **Backup Database:**
   - Trước khi chạy migrations trong production, nên backup database
   - Render có tính năng backup tự động

## 📝 Quick Command Reference

### Kiểm tra migrations status:
```bash
dotnet ef migrations list \
  --project IAM_Service.Infrastructure/IAM_Service.Infrastructure.csproj \
  --startup-project IAM_Service.API/IAM_Service.API.csproj
```

### Xem database info:
```bash
dotnet ef database info \
  --project IAM_Service.Infrastructure/IAM_Service.Infrastructure.csproj \
  --startup-project IAM_Service.API/IAM_Service.API.csproj
```

### Chạy migrations (Development):
```bash
dotnet ef database update \
  --project IAM_Service.Infrastructure/IAM_Service.Infrastructure.csproj \
  --startup-project IAM_Service.API/IAM_Service.API.csproj \
  --configuration Development
```

---

## ✅ Checklist

- [ ] Connection string đã được cập nhật trong `appsettings.Development.json`
- [ ] EF Core Tools đã được cài đặt (`dotnet tool install --global dotnet-ef`)
- [ ] Migrations đã được chạy từ local
- [ ] Schema `iam_service` đã được tạo trong database
- [ ] Tables (Users, Roles, etc.) đã được tạo
- [ ] Test registration endpoint → `201 Created` ✅

