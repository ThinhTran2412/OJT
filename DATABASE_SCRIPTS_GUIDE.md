# 🗄️ Database Migration Scripts Guide

Hướng dẫn sử dụng các scripts quản lý database migrations cho OJT Laboratory Project.

## 📋 Tổng quan

Các scripts được đặt ở thư mục `Deploy` (cùng cấp với `setup_project.bat`) và sẽ được tự động copy vào `OJT_Laboratory_Project` khi chạy `setup_project.bat`.

## 🏗️ Cấu trúc Database

- **Database:** `OJT_Laboratory_DB` (chung cho tất cả services)
- **Schemas:** Mỗi service sử dụng schema riêng:
  - `iam_service` - IAM_Service
  - `laboratory_service` - Laboratory_Service
  - `monitoring_service` - Monitoring_Service
  - `simulator_service` - Simulator_Service

## 📝 Các Scripts

### 1. `clear_all_migrations.bat`

**XÓA TẤT CẢ** migrations và snapshots cho tất cả services.

**⚠️ WARNING:** Script này sẽ **XÓA VĨNH VIỄN** tất cả migrations!

**Cách sử dụng:**
```batch
cd Deploy
clear_all_migrations.bat
```

**Chức năng:**
- Xóa tất cả file migration (*.cs) trong thư mục Migrations
- Xóa tất cả snapshots (AppDbContextModelSnapshot.cs)
- Sử dụng khi muốn reset hoàn toàn migrations

---

### 2. `create_all_migrations.bat`

Tạo migrations mới cho **TẤT CẢ** services (mặc định dùng Development config).

**Cách sử dụng:**
```batch
cd Deploy
create_all_migrations.bat "MigrationName"
```

**Lưu ý:** 
- Đây là script mặc định, tương đương với `create_migrations_dev.bat`
- Để tạo migrations với Production config, dùng `create_migrations_prod.bat`

---

### 2a. `create_migrations_dev.bat`

Tạo migrations mới cho **TẤT CẢ** services với **Development configuration** (localhost database).

**Cách sử dụng:**
```batch
cd Deploy
create_migrations_dev.bat "MigrationName"
```

**Ví dụ:**
```batch
create_migrations_dev.bat "AddNewTable"
create_migrations_dev.bat "InitialCreate"
create_migrations_dev.bat "UpdateUserSchema"
```

**Chức năng:**
- Kiểm tra và cài đặt EF Core Tools (nếu chưa có)
- Tạo migration mới với tên đã chỉ định cho từng service
- Sử dụng **Development configuration** (`appsettings.Development.json`)
- Kết nối đến localhost database để tạo migrations
- Hiển thị summary kết quả

**Lưu ý:**
- Migration name sẽ được áp dụng cho tất cả services
- Sau khi tạo migrations, chạy `update_databases_dev.bat` để apply
- **Khuyến nghị:** Luôn tạo migrations với Dev config, sau đó apply cho cả Dev và Prod

---

### 2b. `create_migrations_prod.bat`

Tạo migrations mới cho **TẤT CẢ** services với **Production configuration** (Render database).

**⚠️ WARNING:** Script này sẽ tạo migrations sử dụng Production database connection!

**Cách sử dụng:**
```batch
cd Deploy
create_migrations_prod.bat "MigrationName"
```

Script sẽ yêu cầu xác nhận trước khi chạy:
```
Are you sure you want to continue? (y/n):
```

**Chức năng:**
- Kiểm tra và cài đặt EF Core Tools (nếu chưa có)
- Tạo migration mới với tên đã chỉ định cho từng service
- Sử dụng **Production configuration** (`appsettings.Production.json`)
- Kết nối đến Render database để tạo migrations
- Yêu cầu xác nhận trước khi chạy
- Hiển thị summary kết quả

**Lưu ý:**
- **Không khuyến nghị** sử dụng trong hầu hết trường hợp
- Nên tạo migrations với Dev config (`create_migrations_dev.bat`)
- Sau khi tạo migrations, commit và push lên Git
- Chạy `update_databases_prod.bat` để apply migrations

**Khi nào dùng:**
- Khi cần tạo migrations trực tiếp từ Production database schema
- Khi có sự khác biệt đáng kể giữa Dev và Prod database structure

---

### 3. `update_all_databases.bat`

Chạy migrations cho tất cả services (mặc định dùng Development config).

**Cách sử dụng:**
```batch
cd Deploy
update_all_databases.bat
```

**Chức năng:**
- Apply migrations cho tất cả services
- Sử dụng **Development configuration** (localhost database)
- Hiển thị summary kết quả

**Lưu ý:** 
- Đây là script mặc định, tương đương với `update_databases_dev.bat`
- Để chạy Production, dùng `update_databases_prod.bat`

---

### 4. `update_databases_dev.bat`

Chạy migrations cho tất cả services với **Development configuration** (localhost database).

**Cách sử dụng:**
```batch
cd Deploy
update_databases_dev.bat
```

**Chức năng:**
- Apply migrations cho tất cả services
- Sử dụng `appsettings.Development.json`
- Kết nối đến localhost PostgreSQL database
- Hiển thị summary kết quả

**Database Target:**
- Host: `localhost:5432`
- Database: `OJT_Laboratory_DB`
- Username: `postgres`
- Password: `12345`

---

### 5. `update_databases_prod.bat`

Chạy migrations cho tất cả services với **Production configuration** (Render database).

**⚠️ WARNING:** Script này sẽ update database Production trên Render!

**Cách sử dụng:**
```batch
cd Deploy
update_databases_prod.bat
```

Script sẽ yêu cầu xác nhận trước khi chạy:
```
Do you want to continue? (y/n): 
```

**Chức năng:**
- Apply migrations cho tất cả services
- Sử dụng `appsettings.Production.json`
- Kết nối đến Render PostgreSQL database
- Yêu cầu xác nhận trước khi chạy
- Hiển thị summary kết quả

**Database Target:**
- Host: `dpg-d4fcsm95pdvs73ader70-a` (Render internal)
- Database: `laboratory_service`
- Username: `laboratory_service_user`

---

## 🔄 Workflow Khuyến nghị

### Khi setup project mới

Sau khi chạy `setup_project.bat`, script sẽ hỏi:
```
Do you want to reset database migrations? (y/n):
```

**Nếu chọn `y`:**
1. Clear tất cả migrations cũ (`clear_all_migrations.bat`)
2. Tạo migrations mới (`create_all_migrations.bat "InitialCreate"`)
3. Apply migrations (`update_databases_dev.bat`)

**Nếu chọn `n`:**
- Bỏ qua bước reset migrations
- Có thể chạy thủ công sau

---

### Khi có thay đổi database schema

#### Development Environment:

1. **Tạo migrations mới:**
   ```batch
   cd Deploy
   create_migrations_dev.bat "DescriptionOfChanges"
   ```
   Hoặc:
   ```batch
   create_all_migrations.bat "DescriptionOfChanges"  # Tương đương với create_migrations_dev.bat
   ```

2. **Kiểm tra migration files:**
   - Xem các file trong `OJT_Laboratory_Project/*_Service.*/Migrations/`
   - Đảm bảo migrations đúng như mong muốn

3. **Apply migrations (Development):**
   ```batch
   cd Deploy
   update_databases_dev.bat
   ```

#### Production Environment:

1. **Tạo migrations mới** (từ local với Development config - khuyến nghị):
   ```batch
   cd Deploy
   create_migrations_dev.bat "DescriptionOfChanges"
   ```
   
   **Lưu ý:** Luôn tạo migrations với Dev config, sau đó apply cho cả Dev và Prod. Migration files sẽ giống nhau.

2. **Commit và push migrations:**
   ```batch
   git add OJT_Laboratory_Project/*/Migrations/*.cs
   git commit -m "Add migrations: DescriptionOfChanges"
   git push origin master
   ```

3. **Apply migrations (Production):**
   ```batch
   cd Deploy
   update_databases_prod.bat
   ```
   (Hoặc đợi Render tự động deploy nếu đã cấu hình)

---

### Khi muốn reset hoàn toàn migrations

1. **Clear tất cả migrations:**
   ```batch
   cd Deploy
   clear_all_migrations.bat
   ```

2. **Tạo migrations mới:**
   ```batch
   create_migrations_dev.bat "InitialCreate"
   ```
   Hoặc:
   ```batch
   create_all_migrations.bat "InitialCreate"  # Tương đương với create_migrations_dev.bat
   ```

3. **Apply migrations:**
   ```batch
   update_databases_dev.bat
   ```
   Hoặc cho Production:
   ```batch
   update_databases_prod.bat
   ```

---

## 📍 Vị trí Scripts

- **Thư mục gốc (Deploy):** Scripts được đặt ở đây để có sẵn khi pull code về
- **Sau khi setup:** Scripts được copy vào `OJT_Laboratory_Project` để sử dụng

Các scripts có thể được chạy từ cả 2 vị trí:
- Từ `Deploy/` - tự động tìm `OJT_Laboratory_Project`
- Từ `OJT_Laboratory_Project/` - chạy trực tiếp trong project folder

---

## 🔧 Troubleshooting

### Lỗi: "EF Core Tools not found"
- Script sẽ tự động cài đặt
- Hoặc cài thủ công: `dotnet tool install --global dotnet-ef`

### Lỗi: "Cannot connect to database"
**Development:**
- Kiểm tra PostgreSQL đang chạy: `pg_isready` hoặc `psql -U postgres`
- Kiểm tra database `OJT_Laboratory_DB` đã được tạo chưa
- Kiểm tra connection string trong `appsettings.Development.json`

**Production:**
- Kiểm tra connection string trong `appsettings.Production.json`
- Kiểm tra Render database đang hoạt động
- Kiểm tra firewall/network connectivity

### Lỗi: "Schema already exists"
- Đây là bình thường nếu schema đã được tạo từ trước
- Có thể bỏ qua hoặc drop schema nếu cần reset:
  ```sql
  DROP SCHEMA IF EXISTS iam_service CASCADE;
  ```

### Migration conflicts
- Nếu một service migration fail, các service khác vẫn tiếp tục chạy
- Kiểm tra log để xem service nào bị lỗi
- Chạy lại script cho service cụ thể nếu cần:
  ```batch
  cd OJT_Laboratory_Project/IAM_Service
  dotnet ef database update --project IAM_Service.Infrastructure/IAM_Service.Infrastructure.csproj --startup-project IAM_Service.API/IAM_Service.API.csproj --configuration Development
  ```

### Lỗi: "OJT_Laboratory_Project folder not found"
- Chạy `setup_project.bat` trước để tạo project structure
- Đảm bảo đang chạy script từ thư mục `Deploy`

---

## 📌 Notes

- Tất cả services dùng **chung 1 database** (`OJT_Laboratory_DB`)
- Mỗi service có **schema riêng** để tránh conflict
- Scripts được thiết kế để chạy độc lập - nếu một service fail, các service khác vẫn tiếp tục
- Scripts có colored output để dễ theo dõi progress
- Development và Production configurations được tách biệt rõ ràng
- Luôn test migrations trên Development trước khi chạy Production

---

## 🚀 Quick Reference

| Script | Environment | Purpose |
|--------|-------------|---------|
| `clear_all_migrations.bat` | - | Xóa tất cả migrations |
| `create_all_migrations.bat "Name"` | Development | Tạo migrations mới (mặc định) |
| `create_migrations_dev.bat "Name"` | Development | Tạo migrations mới (Dev DB) |
| `create_migrations_prod.bat "Name"` | Production | Tạo migrations mới (Render DB) |
| `update_all_databases.bat` | Development | Apply migrations (mặc định) |
| `update_databases_dev.bat` | Development | Apply migrations (Dev DB) |
| `update_databases_prod.bat` | Production | Apply migrations (Render DB) |

