# Database Migration Scripts

## Tổng quan

Thư mục `Deploy` chứa các script để quản lý database migrations cho tất cả services trong `OJT_Laboratory_Project`.

**Lưu ý:** Các scripts này được đặt ở thư mục `Deploy` (cùng cấp với `setup_project.bat`) và sẽ được tự động copy vào `OJT_Laboratory_Project` khi chạy `setup_project.bat`.

## Cấu trúc Database

- **Database Name:** `OJT_Laboratory_DB` (chung cho tất cả services)
- **Schemas:** Mỗi service sử dụng schema riêng:
  - `iam_service` - IAM_Service
  - `laboratory_service` - Laboratory_Service
  - `monitoring_service` - Monitoring_Service
  - `simulator_service` - Simulator_Service

## Scripts

### 1. `clear_all_migrations.bat`

**XÓA TẤT CẢ** migrations và snapshots cho tất cả services (cẩn thận!).

**Cách sử dụng:**
```bash
cd Deploy
clear_all_migrations.bat
```

**Chức năng:**
- Xóa tất cả file migration (*.cs) trong thư mục Migrations của từng service
- Xóa tất cả snapshots (AppDbContextModelSnapshot.cs)
- Sử dụng khi muốn reset hoàn toàn migrations và tạo lại từ đầu

**⚠️ WARNING:**
- Script này sẽ **XÓA VĨNH VIỄN** tất cả migrations
- Chỉ sử dụng khi muốn bắt đầu lại từ đầu
- Thường được gọi từ `setup_project.bat` khi setup mới

---

### 2. `update_all_databases.bat`

Chạy migrations cho **TẤT CẢ** services cùng lúc.

**Cách sử dụng:**
```bash
cd Deploy
update_all_databases.bat
```

**Chức năng:**
- Kiểm tra và cài đặt EF Core Tools (nếu chưa có)
- Restore packages cho tất cả services
- Build tất cả services
- Apply database migrations cho từng service:
  1. IAM_Service
  2. Laboratory_Service
  3. Monitoring_Service
  4. Simulator_Service
- Hiển thị summary kết quả

**Kết quả:**
- Nếu tất cả thành công: Exit code 0
- Nếu có lỗi: Exit code 1 và hiển thị chi tiết

---

### 3. `create_all_migrations.bat`

Tạo migrations mới cho **TẤT CẢ** services cùng lúc.

**Cách sử dụng:**
```bash
cd Deploy
create_all_migrations.bat "MigrationName"
```

**Ví dụ:**
```bash
create_all_migrations.bat "AddNewTable"
create_all_migrations.bat "UpdateUserSchema"
```

**Chức năng:**
- Kiểm tra và cài đặt EF Core Tools (nếu chưa có)
- Tạo migration mới với tên đã chỉ định cho từng service
- Hiển thị summary kết quả

**Lưu ý:**
- Migration name sẽ được áp dụng cho tất cả services
- Sau khi tạo migrations, chạy `update_all_databases.bat` để apply

---

## Workflow khuyến nghị

### Khi setup project mới (sử dụng setup_project.bat):

Sau khi chạy `setup_project.bat` ở thư mục Deploy, script sẽ:
1. Pull tất cả repositories (5 services)
2. Copy các database scripts vào `OJT_Laboratory_Project`
3. Hỏi: `Do you want to reset database migrations? (y/n):`

Nếu nhập **`y`**, script sẽ tự động:
1. Clear tất cả migrations cũ (`clear_all_migrations.bat`)
2. Tạo migrations mới (`create_all_migrations.bat "InitialCreate"`)
3. Apply migrations (`update_all_databases.bat`)

Nếu nhập **`n`**, script sẽ bỏ qua bước này.

---

### Khi có thay đổi database schema:

1. **Tạo migrations mới:**
   ```bash
   cd Deploy
   create_all_migrations.bat "DescriptionOfChanges"
   ```

2. **Kiểm tra migration files:**
   - Xem các file migration trong `OJT_Laboratory_Project/*_Service.*/Migrations/`
   - Đảm bảo migrations đúng như mong muốn

3. **Apply migrations:**
   ```bash
   cd Deploy
   update_all_databases.bat
   ```

---

### Khi muốn reset hoàn toàn migrations:

1. **Clear tất cả migrations:**
   ```bash
   cd Deploy
   clear_all_migrations.bat
   ```

2. **Tạo migrations mới:**
   ```bash
   create_all_migrations.bat "InitialCreate"
   ```

3. **Apply migrations:**
   ```bash
   update_all_databases.bat
   ```

---

## Vị trí Scripts

- **Thư mục gốc (Deploy):** Scripts được đặt ở đây để có sẵn khi pull code về
- **Sau khi setup:** Scripts được copy vào `OJT_Laboratory_Project` để sử dụng

Các scripts có thể được chạy từ cả 2 vị trí:
- Từ `Deploy/` - tự động tìm `OJT_Laboratory_Project` 
- Từ `OJT_Laboratory_Project/` - chạy trực tiếp trong project folder

---

## Troubleshooting

### Lỗi: "EF Core Tools not found"
- Script sẽ tự động cài đặt
- Hoặc cài thủ công: `dotnet tool install --global dotnet-ef`

### Lỗi: "Cannot connect to database"
- Kiểm tra connection string trong `appsettings.json`
- Đảm bảo PostgreSQL đang chạy
- Kiểm tra database `OJT_Laboratory_DB` đã được tạo chưa

### Lỗi: "Schema already exists"
- Đây là bình thường nếu schema đã được tạo từ trước
- Có thể bỏ qua hoặc drop schema nếu cần reset

### Migration conflicts
- Nếu một service migration fail, các service khác vẫn tiếp tục chạy
- Kiểm tra log để xem service nào bị lỗi
- Chạy lại script cho service cụ thể nếu cần

### Lỗi: "OJT_Laboratory_Project folder not found"
- Chạy `setup_project.bat` trước để tạo project structure
- Đảm bảo đang chạy script từ thư mục `Deploy`

---

## Notes

- Tất cả services dùng **chung 1 database** (`OJT_Laboratory_DB`)
- Mỗi service có **schema riêng** để tránh conflict
- Scripts được thiết kế để chạy độc lập - nếu một service fail, các service khác vẫn tiếp tục
- Scripts có colored output để dễ theo dõi progress
- Scripts có thể chạy từ `Deploy/` hoặc `OJT_Laboratory_Project/` folder

