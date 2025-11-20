# 🔍 Kiểm Tra Migrations Đã Chạy Thành Công

## ❌ Vấn Đề

Dù đã chạy migrations nhưng vẫn lỗi: `relation "iam_service.Users" does not exist`

**Nguyên nhân có thể:**
1. Migrations chưa thực sự chạy thành công
2. Migrations chạy sai database hoặc schema
3. Connection string không đúng khi chạy migrations
4. Migrations files chưa có hoặc chưa đúng

## ✅ Kiểm Tra Migrations

### Bước 1: Kiểm Tra Migrations Files

**Kiểm tra xem có migrations files chưa:**

```bash
cd F:\OJT_Assigment\OJT_Project\Deploy\OJT_Laboratory_Project\IAM_Service
dir /s /b *Migration*.cs
```

**Hoặc kiểm tra folder:**
```
OJT_Laboratory_Project/IAM_Service/IAM_Service.Infrastructure/Migrations/
```

**Phải có:**
- ✅ Files `.cs` với tên như `2024XXXXXX_InitialCreate.cs` hoặc tương tự
- ❌ Nếu không có → Cần tạo migrations trước

### Bước 2: Kiểm Tra Migrations Status

**Chạy lệnh để xem migrations status:**

```bash
cd F:\OJT_Assigment\OJT_Project\Deploy\OJT_Laboratory_Project\IAM_Service

dotnet ef migrations list \
  --project IAM_Service.Infrastructure/IAM_Service.Infrastructure.csproj \
  --startup-project IAM_Service.API/IAM_Service.API.csproj \
  --configuration Development
```

**Kết quả mong đợi:**
```
20240101000000_InitialCreate
20240102000000_AddUserTable
...
```

**Nếu không có migrations nào:** Cần tạo migrations trước (xem Bước 4)

### Bước 3: Kiểm Tra Database Info

**Xem database đã có migrations nào chưa:**

```bash
dotnet ef database info \
  --project IAM_Service.Infrastructure/IAM_Service.Infrastructure.csproj \
  --startup-project IAM_Service.API/IAM_Service.API.csproj \
  --configuration Development
```

**Kết quả mong đợi:**
- ✅ List các migrations đã được apply
- ❌ Nếu báo "No migrations have been applied" → Migrations chưa chạy thành công

### Bước 4: Tạo Migrations (Nếu Chưa Có)

**Nếu chưa có migrations files, tạo migrations:**

```bash
cd F:\OJT_Assigment\OJT_Project\Deploy\OJT_Laboratory_Project\IAM_Service

dotnet ef migrations add InitialCreate \
  --project IAM_Service.Infrastructure/IAM_Service.Infrastructure.csproj \
  --startup-project IAM_Service.API/IAM_Service.API.csproj \
  --configuration Development
```

**Lưu ý:**
- Tên migration: `InitialCreate` hoặc tên khác
- Migrations files sẽ được tạo trong `IAM_Service.Infrastructure/Migrations/`

### Bước 5: Chạy Migrations Với Connection String Đúng

**Đảm bảo `appsettings.Development.json` có connection string đúng:**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=dpg-d4fcsm95pdvs73ader70-a.singapore-postgres.render.com;Port=5432;Database=laboratory_service;Username=laboratory_service_user;Password=geeqHh8B6xA8oQNkNHw0K0AoJKSZhji2;SSL Mode=Require;Trust Server Certificate=true"
  },
  "Database": {
    "Schema": "iam_service"
  }
}
```

**Chạy migrations:**

```bash
cd F:\OJT_Assigment\OJT_Project\Deploy\OJT_Laboratory_Project\IAM_Service

dotnet ef database update \
  --project IAM_Service.Infrastructure/IAM_Service.Infrastructure.csproj \
  --startup-project IAM_Service.API/IAM_Service.API.csproj \
  --configuration Development
```

**Kiểm tra output:**
- ✅ `Done.` hoặc `Applying migration '...'`
- ❌ Nếu có lỗi → Xem error message

### Bước 6: Verify Tables Đã Được Tạo

**Kết nối database và kiểm tra:**

**Qua Render Dashboard:**
1. Vào Database → **"Connect"** hoặc dùng psql
2. Chạy SQL:

```sql
-- Kiểm tra schema
SELECT schema_name 
FROM information_schema.schemata 
WHERE schema_name = 'iam_service';

-- Kiểm tra tables trong schema iam_service
SELECT table_name 
FROM information_schema.tables 
WHERE table_schema = 'iam_service';

-- Kiểm tra migrations đã apply
SELECT * FROM iam_service."__EFMigrationsHistory" ORDER BY MigrationId;
```

**Tables phải có:**
- ✅ `Users`
- ✅ `Roles`
- ✅ `Privileges`
- ✅ `UserRoles` (hoặc junction table tương tự)
- ✅ `__EFMigrationsHistory` (EF Core tracking table)

## 🔧 Troubleshooting

### Lỗi: "No migrations found"

**Nguyên nhân:** Chưa có migrations files

**Giải pháp:**
1. Tạo migrations (Bước 4)
2. Commit migrations files vào Git
3. Chạy migrations (Bước 5)

### Lỗi: "Connection refused" hoặc "Unable to connect"

**Nguyên nhân:** Connection string không đúng hoặc database không accessible

**Giải pháp:**
1. Kiểm tra connection string trong `appsettings.Development.json`
2. Kiểm tra database status trên Render Dashboard
3. Thử ping database host để kiểm tra connectivity

### Lỗi: "Schema 'iam_service' does not exist"

**Nguyên nhân:** Schema chưa được tạo

**Giải pháp:**
1. Migrations sẽ tự động tạo schema khi chạy
2. Nếu vẫn lỗi, tạo schema thủ công:
   ```sql
   CREATE SCHEMA IF NOT EXISTS iam_service;
   ```

### Lỗi: "Migrations already applied" nhưng vẫn không có tables

**Nguyên nhân:** Migrations đã apply nhưng có lỗi trong quá trình tạo tables

**Giải pháp:**
1. Drop schema và chạy lại migrations (CẨN THẬN!):
   ```sql
   DROP SCHEMA IF EXISTS iam_service CASCADE;
   ```
2. Chạy lại migrations:
   ```bash
   dotnet ef database update
   ```

## ⚠️ Lưu Ý Quan Trọng

1. **Migrations Files Phải Có:**
   - Kiểm tra folder `IAM_Service.Infrastructure/Migrations/` có files `.cs` chưa
   - Nếu chưa có, cần tạo migrations trước

2. **Connection String Đúng:**
   - Dùng External URL với SSL cho local
   - Đảm bảo `Database:Schema` đã set trong config

3. **Migrations Chỉ Chạy Một Lần:**
   - EF Core track migrations trong `__EFMigrationsHistory`
   - Nếu migrations đã apply, sẽ skip

## ✅ Checklist

- [ ] Migrations files có tồn tại trong `Migrations/` folder
- [ ] `appsettings.Development.json` có connection string đúng
- [ ] `Database:Schema` đã set trong config
- [ ] Migrations đã được chạy (qua `dotnet ef database update`)
- [ ] Schema `iam_service` đã được tạo trong database
- [ ] Tables (Users, Roles, etc.) đã được tạo
- [ ] `__EFMigrationsHistory` table có trong database

## 🎯 Quick Fix Commands

### 1. Kiểm tra migrations status:
```bash
cd F:\OJT_Assigment\OJT_Project\Deploy\OJT_Laboratory_Project\IAM_Service
dotnet ef migrations list --project IAM_Service.Infrastructure/IAM_Service.Infrastructure.csproj --startup-project IAM_Service.API/IAM_Service.API.csproj --configuration Development
```

### 2. Tạo migrations (nếu chưa có):
```bash
dotnet ef migrations add InitialCreate --project IAM_Service.Infrastructure/IAM_Service.Infrastructure.csproj --startup-project IAM_Service.API/IAM_Service.API.csproj --configuration Development
```

### 3. Chạy migrations:
```bash
dotnet ef database update --project IAM_Service.Infrastructure/IAM_Service.Infrastructure.csproj --startup-project IAM_Service.API/IAM_Service.API.csproj --configuration Development
```

### 4. Verify tables:
```sql
SELECT table_name FROM information_schema.tables WHERE table_schema = 'iam_service';
```

---

**Sau khi verify và fix, test lại registration endpoint!** ✅

