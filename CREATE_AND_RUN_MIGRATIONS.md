# 🚀 Tạo và Chạy Migrations cho IAM_Service

## ❌ Vấn Đề

**Lỗi:** `relation "iam_service.Users" does not exist`  
**Nguyên nhân:** Migrations files chưa có hoặc migrations chưa được chạy thành công

## ✅ Giải Pháp: Tạo và Chạy Migrations

### Bước 1: Kiểm Tra Migrations Files

**Kiểm tra xem có migrations files chưa:**

```bash
cd F:\OJT_Assigment\OJT_Project\Deploy\OJT_Laboratory_Project\IAM_Service
dir IAM_Service.Infrastructure\Migrations\*.cs
```

**Nếu không có files `.cs`** → Cần tạo migrations (Bước 2)

**Nếu có files `.cs`** → Skip Bước 2, chạy migrations (Bước 3)

### Bước 2: Tạo Migrations (Nếu Chưa Có)

**1. Đảm bảo connection string đúng trong `appsettings.Development.json`:**

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

**2. Cài đặt EF Core Tools (nếu chưa có):**

```bash
dotnet tool install --global dotnet-ef
```

**3. Tạo migrations:**

```bash
cd F:\OJT_Assigment\OJT_Project\Deploy\OJT_Laboratory_Project\IAM_Service

dotnet ef migrations add InitialCreate \
  --project IAM_Service.Infrastructure/IAM_Service.Infrastructure.csproj \
  --startup-project IAM_Service.API/IAM_Service.API.csproj \
  --configuration Development
```

**Lưu ý:**
- Tên migration: `InitialCreate` (hoặc tên khác)
- Migrations files sẽ được tạo trong `IAM_Service.Infrastructure/Migrations/`
- Nếu đã có migrations, có thể dùng tên khác: `AddUserTable`, `AddRolesTable`, etc.

**4. Kiểm tra migrations files đã được tạo:**

```bash
dir IAM_Service.Infrastructure\Migrations\*.cs
```

**Phải có:**
- ✅ `XXXXXXXXXXXXXX_InitialCreate.cs` (hoặc tên migration khác)
- ✅ `AppDbContextModelSnapshot.cs`

### Bước 3: Chạy Migrations

**1. Chạy migrations để tạo schema và tables:**

```bash
cd F:\OJT_Assigment\OJT_Project\Deploy\OJT_Laboratory_Project\IAM_Service

dotnet ef database update \
  --project IAM_Service.Infrastructure/IAM_Service.Infrastructure.csproj \
  --startup-project IAM_Service.API/IAM_Service.API.csproj \
  --configuration Development
```

**Output mong đợi:**
```
Applying migration 'XXXXXXXXXXXXXX_InitialCreate'.
Done.
```

**Hoặc nếu migrations đã được apply:**
```
No migrations were applied. The database is already up to date.
```

**2. Kiểm tra kết quả:**

**Qua Render Dashboard → Database → Connect:**
```sql
-- Kiểm tra schema
SELECT schema_name 
FROM information_schema.schemata 
WHERE schema_name = 'iam_service';

-- Kiểm tra tables
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
- ✅ `RolePrivileges`
- ✅ `UserPrivileges`
- ✅ `RefreshTokens`
- ✅ `PasswordResets`
- ✅ `AuditLogs`
- ✅ `__EFMigrationsHistory`

### Bước 4: Commit Migrations Files

**Sau khi tạo migrations, commit vào Git:**

```bash
cd F:\OJT_Assigment\OJT_Project\Deploy

git add OJT_Laboratory_Project/IAM_Service/IAM_Service.Infrastructure/Migrations/*.cs
git commit -m "Add EF Core migrations for IAM_Service"
git push origin master
```

**Lưu ý:** Migrations files **PHẢI** được commit vào Git để Render có thể chạy migrations trong Build Command.

### Bước 5: Test Lại API

**1. Đợi IAM_Service rebuild trên Render (nếu có Build Command với migrations)**

**2. Test registration:**
- Vào `https://front-end-fnfs.onrender.com/register`
- Điền form và submit
- Kiểm tra:
  - ✅ `201 Created` → Thành công! Tables đã được tạo
  - ❌ `500` → Vẫn còn lỗi, kiểm tra logs tiếp

## 🔍 Troubleshooting

### Lỗi: "No database provider has been configured"

**Nguyên nhân:** Connection string không đúng hoặc không được tìm thấy

**Giải pháp:**
1. Kiểm tra `appsettings.Development.json` có connection string chưa
2. Đảm bảo `--configuration Development` được dùng

### Lỗi: "Unable to connect to database"

**Nguyên nhân:** Connection string không đúng hoặc database không accessible

**Giải pháp:**
1. Kiểm tra connection string trong `appsettings.Development.json`
2. Kiểm tra database status trên Render Dashboard
3. Thử ping database host: `ping dpg-d4fcsm95pdvs73ader70-a.singapore-postgres.render.com`

### Lỗi: "Migration already exists"

**Nguyên nhân:** Migrations files đã có

**Giải pháp:**
- Không cần tạo migrations nữa, chạy migrations (Bước 3)

### Lỗi: "Schema already exists" nhưng vẫn không có tables

**Nguyên nhân:** Migrations đã apply nhưng có lỗi

**Giải pháp:**
1. Drop schema và chạy lại migrations (CẨN THẬN!):
   ```sql
   DROP SCHEMA IF EXISTS iam_service CASCADE;
   ```
2. Chạy lại migrations:
   ```bash
   dotnet ef database update --configuration Development
   ```

## ⚠️ Lưu Ý Quan Trọng

1. **Migrations Files Phải Có:**
   - Kiểm tra folder `IAM_Service.Infrastructure/Migrations/` có files `.cs` chưa
   - Nếu chưa có, **PHẢI** tạo migrations trước khi chạy

2. **Connection String Đúng:**
   - Dùng External URL với SSL cho local
   - Đảm bảo `Database:Schema` đã set trong config

3. **Migrations Chỉ Chạy Một Lần:**
   - EF Core track migrations trong `__EFMigrationsHistory`
   - Nếu migrations đã apply, sẽ skip

4. **Commit Migrations Files:**
   - Migrations files **PHẢI** được commit vào Git
   - Render cần migrations files để chạy migrations trong Build Command

## ✅ Checklist

### Trước khi chạy migrations:
- [ ] `appsettings.Development.json` có connection string đúng
- [ ] `Database:Schema` đã set trong config
- [ ] EF Core Tools đã được cài đặt
- [ ] Migrations files đã có (hoặc đã tạo migrations)

### Sau khi chạy migrations:
- [ ] Migrations đã được chạy (qua `dotnet ef database update`)
- [ ] Schema `iam_service` đã được tạo trong database
- [ ] Tables (Users, Roles, etc.) đã được tạo
- [ ] `__EFMigrationsHistory` table có trong database
- [ ] Migrations files đã được commit vào Git
- [ ] Test registration endpoint → `201 Created` ✅

## 🎯 Quick Commands

### 1. Tạo migrations:
```bash
cd F:\OJT_Assigment\OJT_Project\Deploy\OJT_Laboratory_Project\IAM_Service
dotnet ef migrations add InitialCreate --project IAM_Service.Infrastructure/IAM_Service.Infrastructure.csproj --startup-project IAM_Service.API/IAM_Service.API.csproj --configuration Development
```

### 2. Chạy migrations:
```bash
dotnet ef database update --project IAM_Service.Infrastructure/IAM_Service.Infrastructure.csproj --startup-project IAM_Service.API/IAM_Service.API.csproj --configuration Development
```

### 3. Kiểm tra migrations status:
```bash
dotnet ef migrations list --project IAM_Service.Infrastructure/IAM_Service.Infrastructure.csproj --startup-project IAM_Service.API/IAM_Service.API.csproj --configuration Development
```

### 4. Kiểm tra database info:
```bash
dotnet ef database info --project IAM_Service.Infrastructure/IAM_Service.Infrastructure.csproj --startup-project IAM_Service.API/IAM_Service.API.csproj --configuration Development
```

---

## 📝 Next Steps

1. **Tạo migrations** (nếu chưa có) - Bước 2
2. **Chạy migrations** - Bước 3
3. **Commit migrations files** - Bước 4
4. **Test API** - Bước 5

**Sau khi hoàn thành, test lại registration endpoint!** ✅

