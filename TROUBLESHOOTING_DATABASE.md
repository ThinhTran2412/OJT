# 🔧 Troubleshooting Database Connection - 500 PostgresException

## 🐛 Lỗi

**Error:** `500 Internal Server Error` với `ExceptionType: PostgresException`

**Nguyên nhân có thể:**
1. Database chưa kết nối được
2. Migrations chưa được chạy (tables/schema chưa được tạo)
3. Connection string không đúng
4. Schema chưa được tạo trong database

## ✅ Kiểm Tra và Khắc Phục

### Bước 1: Kiểm Tra DATABASE_URL Environment Variable

1. **Vào Render Dashboard:**
   - https://dashboard.render.com
   - Chọn **IAM_Service** → **"Environment"** tab

2. **Kiểm tra có `DATABASE_URL` chưa:**
   - ✅ **Có:** Xem giá trị có đúng không
   - ❌ **Không có:** Thêm `DATABASE_URL`:
     - **Key:** `DATABASE_URL`
     - **Value:** `postgresql://laboratory_service_user:geeqHh8B6xA8oQNkNHw0K0AoJKSZhji2@dpg-d4fcsm95pdvs73ader70-a/laboratory_service`
     - **Lưu ý:** Dùng Internal URL nếu database và service trong cùng Project

3. **Save và Redeploy:**
   - Click **"Save Changes"**
   - Render sẽ tự động rebuild

### Bước 2: Kiểm Tra Logs trên Render

1. **Vào IAM_Service → "Logs"** tab
2. **Tìm các dòng lỗi liên quan đến database:**
   ```
   Unable to connect to database
   Connection refused
   PostgresException
   schema "iam_service" does not exist
   relation "users" does not exist
   ```

**Lỗi thường gặp:**

#### Lỗi 1: "schema 'iam_service' does not exist"
**Nguyên nhân:** Schema chưa được tạo trong database

**Giải pháp:** Chạy migrations để tạo schema (xem Bước 3)

#### Lỗi 2: "relation 'users' does not exist" 
**Nguyên nhân:** Tables chưa được tạo (migrations chưa chạy)

**Giải pháp:** Chạy migrations để tạo tables (xem Bước 3)

#### Lỗi 3: "Unable to connect to database" hoặc "Connection refused"
**Nguyên nhân:** Connection string không đúng hoặc database không accessible

**Giải pháp:**
- Kiểm tra `DATABASE_URL` có đúng không
- Kiểm tra database status trên Render Dashboard
- Thử dùng External URL nếu Internal không hoạt động

### Bước 3: Chạy Migrations trên Render

#### Cách 1: Qua Render Shell (KHUYẾN NGHỊ)

1. **Vào IAM_Service → "Shell"** tab
2. **Chạy các lệnh sau:**

```bash
# Navigate to project directory
cd /opt/render/project/src

# Install EF Core Tools (nếu chưa có)
dotnet tool install --global dotnet-ef || true

# Run migrations
dotnet ef database update \
  --project IAM_Service.Infrastructure/IAM_Service.Infrastructure.csproj \
  --startup-project IAM_Service.API/IAM_Service.API.csproj
```

**Lưu ý:** 
- EF Core sẽ tự động tạo schema `iam_service` nếu chưa có
- EF Core sẽ tạo tất cả tables theo migrations

#### Cách 2: Qua Build Command (Tự động - Nếu đã cấu hình)

Build Command đã có sẵn migration update:
```bash
dotnet tool install --global dotnet-ef || true && \
dotnet restore IAM_Service.sln && \
dotnet build IAM_Service.sln -c Release && \
dotnet ef database update --project IAM_Service.Infrastructure/IAM_Service.Infrastructure.csproj --startup-project IAM_Service.API/IAM_Service.API.csproj && \
dotnet publish IAM_Service.API/IAM_Service.API.csproj -c Release -o ./publish
```

**Nếu migrations đã chạy trong Build Command nhưng vẫn lỗi:**
- Xem logs xem migrations có chạy thành công không
- Kiểm tra xem có lỗi gì trong quá trình migration không

### Bước 4: Kiểm Tra Database Schema

1. **Kết nối Database:**
   - Vào Database trên Render Dashboard
   - Click **"Connect"** hoặc dùng connection string

2. **Kiểm tra Schema:**
   ```sql
   -- Kiểm tra schema có tồn tại không
   SELECT schema_name 
   FROM information_schema.schemata 
   WHERE schema_name = 'iam_service';
   
   -- Nếu chưa có, tạo schema
   CREATE SCHEMA IF NOT EXISTS iam_service;
   ```

3. **Kiểm tra Tables:**
   ```sql
   -- Xem tất cả tables trong schema iam_service
   SELECT table_name 
   FROM information_schema.tables 
   WHERE table_schema = 'iam_service';
   ```

**Nếu chưa có tables:** Chạy migrations (Bước 3)

### Bước 5: Kiểm Tra Connection String Format

**Connection String trong `appsettings.Production.json`:**
```
Host=dpg-d4fcsm95pdvs73ader70-a;Port=5432;Database=laboratory_service;Username=laboratory_service_user;Password=geeqHh8B6xA8oQNkNHw0K0AoJKSZhji2
```

**DATABASE_URL format (PostgreSQL URL):**
```
postgresql://laboratory_service_user:geeqHh8B6xA8oQNkNHw0K0AoJKSZhji2@dpg-d4fcsm95pdvs73ader70-a/laboratory_service
```

**Code tự động convert PostgreSQL URL → Connection String:** ✅ Đã có trong `InfrastructureDI.cs`

### Bước 6: Test Connection Thủ Công

1. **Vào IAM_Service → "Shell"** tab
2. **Test connection:**

```bash
# Export DATABASE_URL (nếu chưa có trong environment)
export DATABASE_URL="postgresql://laboratory_service_user:geeqHh8B6xA8oQNkNHw0K0AoJKSZhji2@dpg-d4fcsm95pdvs73ader70-a/laboratory_service"

# Test với psql (nếu có)
psql $DATABASE_URL -c "SELECT version();"

# Hoặc test với dotnet
cd /opt/render/project/src
dotnet ef database info \
  --project IAM_Service.Infrastructure/IAM_Service.Infrastructure.csproj \
  --startup-project IAM_Service.API/IAM_Service.API.csproj
```

## ✅ Checklist

### Database Connection:
- [ ] `DATABASE_URL` đã được set trong Render Environment
- [ ] Connection string đúng format
- [ ] Database status là "Available" trên Render
- [ ] Service và Database trong cùng region/network

### Migrations:
- [ ] Migrations đã được chạy (qua Build Command hoặc Shell)
- [ ] Schema `iam_service` đã được tạo
- [ ] Tables đã được tạo (users, roles, privileges, etc.)
- [ ] EF Core migrations table (`__EFMigrationsHistory`) có trong database

### Service Logs:
- [ ] Không có lỗi "Connection refused"
- [ ] Không có lỗi "schema does not exist"
- [ ] Không có lỗi "relation does not exist"
- [ ] Service đã start và running

## 🔍 Debug Commands

### Kiểm tra migrations status:
```bash
dotnet ef migrations list \
  --project IAM_Service.Infrastructure/IAM_Service.Infrastructure.csproj \
  --startup-project IAM_Service.API/IAM_Service.API.csproj
```

### Kiểm tra database info:
```bash
dotnet ef database info \
  --project IAM_Service.Infrastructure/IAM_Service.Infrastructure.csproj \
  --startup-project IAM_Service.API/IAM_Service.API.csproj
```

### Xem connection string được dùng:
Thêm vào `Program.cs` tạm thời để debug:
```csharp
var connectionString = !string.IsNullOrEmpty(envConnection)
    ? ConvertPostgresUrlToConnectionString(envConnection)
    : config.GetConnectionString("DefaultConnection");
    
Console.WriteLine($"Using connection string: {connectionString?.Replace(connectionString?.Split(';').FirstOrDefault(s => s.Contains("Password")), "Password=***")}");
```

## 📞 Next Steps

1. **Kiểm tra logs** trên Render → IAM_Service → Logs
2. **Kiểm tra DATABASE_URL** trong Environment tab
3. **Chạy migrations** qua Shell (Bước 3)
4. **Test lại** registration endpoint

---

**Sau khi fix, test lại:**
- Vào `https://front-end-fnfs.onrender.com/register`
- Điền form và submit
- Kiểm tra response:
  - ✅ `201 Created` → Thành công!
  - ❌ `500` → Vẫn còn lỗi, kiểm tra logs tiếp

