# 🚨 KHẮC PHỤC NGAY: Chạy Migrations trên Render

## ❌ Lỗi Hiện Tại

```
relation "iam_service.Users" does not exist
```

**Nguyên nhân:** Migrations chưa được chạy → Tables chưa được tạo trong database.

## ✅ Giải Pháp: Chạy Migrations trên Render

### Cách 1: Qua Render Shell (KHUYẾN NGHỊ - Nhanh nhất)

1. **Vào Render Dashboard:**
   - https://dashboard.render.com
   - Chọn **IAM_Service**

2. **Vào "Shell" tab:**
   - Click tab **"Shell"** (hoặc **"SSH"**)
   - Mở terminal window

3. **Chạy các lệnh sau:**

```bash
# Navigate to project directory
cd /opt/render/project/src

# Install EF Core Tools (nếu chưa có)
dotnet tool install --global dotnet-ef || true

# Run migrations - TẠO SCHEMA VÀ TABLES
dotnet ef database update \
  --project IAM_Service.Infrastructure/IAM_Service.Infrastructure.csproj \
  --startup-project IAM_Service.API/IAM_Service.API.csproj
```

**Lệnh này sẽ:**
- ✅ Tạo schema `iam_service` nếu chưa có
- ✅ Tạo tất cả tables (Users, Roles, Privileges, etc.)
- ✅ Apply tất cả migrations

4. **Kiểm tra kết quả:**
   - Nếu thành công: `Done.` hoặc `No migrations were applied.`
   - Nếu có lỗi: Xem error message

### Cách 2: Trigger Rebuild (Nếu Build Command có migrations)

1. **Vào IAM_Service → "Settings"** tab

2. **Kiểm tra Build Command có migration update chưa:**
   ```bash
   dotnet ef database update --project IAM_Service.Infrastructure/IAM_Service.Infrastructure.csproj --startup-project IAM_Service.API/IAM_Service.API.csproj
   ```
   
   **Nếu có:** Click **"Manual Deploy"** → **"Deploy latest commit"**

   **Nếu chưa có:** Thêm vào Build Command:
   ```bash
   dotnet tool install --global dotnet-ef || true && \
   dotnet restore IAM_Service.sln && \
   dotnet build IAM_Service.sln -c Release && \
   dotnet ef database update --project IAM_Service.Infrastructure/IAM_Service.Infrastructure.csproj --startup-project IAM_Service.API/IAM_Service.API.csproj && \
   dotnet publish IAM_Service.API/IAM_Service.API.csproj -c Release -o ./publish
   ```

3. **Save và đợi rebuild hoàn tất**

## ✅ Sau Khi Chạy Migrations

### Kiểm Tra Tables Đã Được Tạo

1. **Kết nối Database (nếu có quyền):**
   - Vào Database trên Render Dashboard
   - Click **"Connect"** hoặc dùng psql

2. **Kiểm tra schema và tables:**
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
- ✅ `__EFMigrationsHistory` (EF Core migrations tracking table)

### Test Lại API

1. **Vào `https://front-end-fnfs.onrender.com/register`**
2. **Điền form và submit**
3. **Kiểm tra:**
   - ✅ `201 Created` → Thành công! Tables đã được tạo
   - ❌ `500` → Vẫn còn lỗi, kiểm tra logs tiếp

## 🔍 Nếu Vẫn Lỗi Sau Khi Chạy Migrations

### Lỗi 1: "No migrations found"

**Nguyên nhân:** Migrations files chưa có trong repository

**Giải pháp:**
- Kiểm tra folder `IAM_Service.Infrastructure/Migrations/` có files `.cs` chưa
- Nếu chưa có, tạo migrations (cần chạy local)

### Lỗi 2: "Connection refused" hoặc "Unable to connect"

**Nguyên nhân:** Connection string không đúng

**Giải pháp:**
1. Kiểm tra `DATABASE_URL` trong Environment tab
2. Kiểm tra database status trên Render
3. Thử dùng External URL nếu Internal không hoạt động

### Lỗi 3: "Permission denied" hoặc "Access denied"

**Nguyên nhân:** User không có quyền tạo schema/tables

**Giải pháp:**
1. Kiểm tra database user permissions
2. Đảm bảo user có quyền `CREATE SCHEMA` và `CREATE TABLE`

## ⚠️ Lưu Ý

1. **Schema sẽ được tự động tạo:** EF Core sẽ tự động tạo schema `iam_service` nếu chưa có
2. **Migrations sẽ chạy tự động:** Nếu Build Command có migration update, migrations sẽ chạy mỗi lần rebuild
3. **Một lần duy nhất:** Migrations chỉ cần chạy một lần (hoặc khi có migration mới)

## 🎯 Quick Fix (Copy-Paste)

**Vào Render Shell và chạy:**

```bash
cd /opt/render/project/src && \
dotnet tool install --global dotnet-ef || true && \
dotnet ef database update \
  --project IAM_Service.Infrastructure/IAM_Service.Infrastructure.csproj \
  --startup-project IAM_Service.API/IAM_Service.API.csproj
```

**Đợi xong, test lại registration endpoint!** ✅

