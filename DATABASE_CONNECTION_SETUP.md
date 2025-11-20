# 🔧 Cấu Hình Database Connection cho Render

## 📋 Thông Tin Database

**Database:** PostgreSQL trên Render  
**Host (Internal):** `dpg-d4fcsm95pdvs73ader70-a`  
**Host (External):** `dpg-d4fcsm95pdvs73ader70-a.singapore-postgres.render.com`  
**Port:** `5432`  
**Database Name:** `laboratory_service`  
**Username:** `laboratory_service_user`  
**Password:** `geeqHh8B6xA8oQNkNHw0K0AoJKSZhji2`

## 🔗 Connection Strings

### Internal (Dùng trong cùng Render Project/Region)
```
postgresql://laboratory_service_user:geeqHh8B6xA8oQNkNHw0K0AoJKSZhji2@dpg-d4fcsm95pdvs73ader70-a/laboratory_service
```

### External (Dùng từ bên ngoài Render)
```
postgresql://laboratory_service_user:geeqHh8B6xA8oQNkNHw0K0AoJKSZhji2@dpg-d4fcsm95pdvs73ader70-a.singapore-postgres.render.com/laboratory_service
```

## ⚙️ Cấu Hình trên Render

### Cách 1: Automatic (KHUYẾN NGHỊ - Nếu Database và Services trong cùng Project)

1. **Vào Render Dashboard:**
   - https://dashboard.render.com
   - Chọn Project chứa Database

2. **Settings → Project:**
   - Đảm bảo Database và tất cả Services trong cùng Project
   - Render sẽ tự động set `DATABASE_URL` environment variable

3. **Kiểm tra:**
   - Vào mỗi Service → **"Environment"** tab
   - Xem có `DATABASE_URL` tự động chưa? ✅

**Lưu ý:** Render sẽ dùng Internal Database URL tự động.

### Cách 2: Manual (Nếu Database và Services khác Project)

Vào mỗi Service → **"Environment"** tab → Add:

#### IAM_Service

- **Key:** `DATABASE_URL`
- **Value:** `postgresql://laboratory_service_user:geeqHh8B6xA8oQNkNHw0K0AoJKSZhji2@dpg-d4fcsm95pdvs73ader70-a/laboratory_service`
- **Usage:** Internal (nếu cùng region) hoặc External URL

#### Laboratory_Service

- **Key:** `DATABASE_URL`
- **Value:** `postgresql://laboratory_service_user:geeqHh8B6xA8oQNkNHw0K0AoJKSZhji2@dpg-d4fcsm95pdvs73ader70-a/laboratory_service`

#### Monitoring_Service

- **Key:** `DATABASE_URL`
- **Value:** `postgresql://laboratory_service_user:geeqHh8B6xA8oQNkNHw0K0AoJKSZhji2@dpg-d4fcsm95pdvs73ader70-a/laboratory_service`

#### Simulator_Service

- **Key:** `DATABASE_URL`
- **Value:** `postgresql://laboratory_service_user:geeqHh8B6xA8oQNkNHw0K0AoJKSZhji2@dpg-d4fcsm95pdvs73ader70-a/laboratory_service`

## ✅ Kiểm Tra Connection

### 1. Kiểm tra Environment Variable

Vào Service → **"Environment"** tab → Xem có `DATABASE_URL`:
- ✅ Có: Render sẽ dùng environment variable
- ❌ Không có: Service sẽ dùng `appsettings.Production.json` (đã được cập nhật)

### 2. Kiểm tra Logs

Sau khi deploy, xem Service logs:

**Success:**
```
info: Microsoft.EntityFrameworkCore.Database.Connection[20004]
      An error occurred using the connection to database 'laboratory_service'
```

**Hoặc:**
```
Applied migration: [MigrationName]
```

**Error:**
```
Unable to connect to database
Connection refused
```

### 3. Test Database Connection

Vào Service → **"Shell"** tab:

```bash
# Test connection
psql $DATABASE_URL

# Hoặc nếu không có psql
dotnet ef database update --project [Infrastructure]/[Infrastructure].csproj --startup-project [API]/[API].csproj
```

## 📝 Schema Configuration

Mỗi service đã được cấu hình schema trong `appsettings.json`:

- **IAM_Service:** `iam_service` ✅
- **Laboratory_Service:** `laboratory_service` ✅
- **Monitoring_Service:** `monitoring_service` ✅
- **Simulator_Service:** `simulator_service` ✅

**Không cần thêm environment variable cho schema** - đã có trong `appsettings.json`.

## 🔄 Code Logic

Services đã được cấu hình để ưu tiên `DATABASE_URL` environment variable:

```csharp
// Ưu tiên lấy từ biến môi trường DATABASE_URL (Render, Railway, v.v.)
var envConnection = Environment.GetEnvironmentVariable("DATABASE_URL");

// Nếu không có thì lấy trong appsettings.json
var connectionString = !string.IsNullOrEmpty(envConnection)
    ? ConvertPostgresUrlToConnectionString(envConnection)
    : config.GetConnectionString("DefaultConnection");
```

**Flow:**
1. Kiểm tra `DATABASE_URL` environment variable → ✅ Dùng nếu có
2. Nếu không có → Dùng `appsettings.Production.json` → `DefaultConnection`

## 🚀 Các Bước Thực Hiện

### Bước 1: Cập nhật appsettings.Production.json (Đã làm ✅)

File `IAM_Service/IAM_Service.API/appsettings.Production.json` đã được cập nhật với connection string.

### Bước 2: Cấu hình Environment Variable trên Render

1. Vào IAM_Service → **"Environment"** tab
2. Add `DATABASE_URL` với Internal URL (nếu cùng Project)
3. Save và Redeploy

### Bước 3: Deploy và Test

1. Render sẽ tự động rebuild
2. Kiểm tra logs xem có kết nối database thành công không
3. Test API endpoints

## ⚠️ Lưu Ý

1. **Internal vs External URL:**
   - Internal URL: Dùng khi services và database trong cùng Render Project/Region
   - External URL: Dùng khi services ở ngoài Render hoặc khác region

2. **Security:**
   - Connection string chứa password - không commit vào Git
   - Chỉ dùng environment variables trên Render

3. **Schema Isolation:**
   - Mỗi service dùng schema riêng trong cùng database
   - Không ảnh hưởng lẫn nhau

## 🔍 Troubleshooting

### Lỗi: "Unable to connect to database"

**Nguyên nhân:**
- `DATABASE_URL` không đúng
- Database chưa được tạo
- Firewall/Network issue

**Giải pháp:**
1. Kiểm tra `DATABASE_URL` trong Environment tab
2. Kiểm tra database đã được tạo trên Render
3. Thử dùng External URL nếu Internal không hoạt động

### Lỗi: "Schema does not exist"

**Nguyên nhân:**
- Schema chưa được tạo (chưa chạy migrations)

**Giải pháp:**
1. Chạy migrations:
   ```bash
   dotnet ef database update --project [Infrastructure]/[Infrastructure].csproj --startup-project [API]/[API].csproj
   ```

2. Hoặc migrations sẽ tự động chạy trong Build Command (đã cấu hình)

### Lỗi: "Connection timeout"

**Nguyên nhân:**
- Services và database khác region
- Database không accessible

**Giải pháp:**
1. Đảm bảo database và services trong cùng region
2. Kiểm tra database status trên Render Dashboard

