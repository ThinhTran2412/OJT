# OJT Laboratory Project Setup Scripts

Bộ script tự động hóa việc thiết lập môi trường phát triển cho dự án **OJT Laboratory Management System**.

## 📋 Mục đích

Script này tự động hóa quá trình:
- Tạo cấu trúc thư mục dự án
- Clone các Git repositories từ remote
- Restore các NuGet packages cho .NET services
- Cài đặt npm dependencies cho Frontend
- Build các services

## 📁 Cấu trúc Project

Sau khi chạy script, cấu trúc thư mục sẽ như sau:

```
OJT_Laboratory_Project/
├── IAM_Service/          # Identity and Access Management Service (.NET)
├── Laboratory_Service/   # Laboratory Management Service (.NET)
└── Front_End/            # Frontend Application
```

## 🔗 Git Repositories

Các Git repositories được sử dụng trong dự án:

- **IAM_Service**: [https://git.fsoft-academy.edu.vn/hcm25_cpl_net_08/team04/laboratorymanagementproject.git](https://git.fsoft-academy.edu.vn/hcm25_cpl_net_08/team04/laboratorymanagementproject.git)
  - Branch: `develop`

- **Laboratory_Service**: [https://git.fsoft-academy.edu.vn/hcm25_cpl_net_08/team04/laboratoryservice.git](https://git.fsoft-academy.edu.vn/hcm25_cpl_net_08/team04/laboratoryservice.git)
  - Branch: `develop`

- **Front_End**: [https://git.fsoft-academy.edu.vn/hcm25_cpl_net_08/team04/IAM_Service_FrontEnd.git](https://git.fsoft-academy.edu.vn/hcm25_cpl_net_08/team04/IAM_Service_FrontEnd.git)
  - Branch: `develop`

## 📄 Các File trong Script

### 1. `setup_project.bat`
Script chính để thiết lập toàn bộ dự án. Thực hiện các bước:
1. Tạo thư mục gốc `OJT_Laboratory_Project`
2. Tạo các thư mục con cho từng service
3. Clone và cấu hình Git repositories cho từng service
4. Restore NuGet packages cho các .NET services
5. Cài đặt npm dependencies cho Frontend
6. Build các .NET services

**Cách sử dụng:**
```batch
setup_project.bat
```

### 2. `git_config.txt`
File cấu hình text chứa thông tin Git repositories và branches cho từng service.

**Cấu trúc:**
```
# Git Configuration File
# Lines starting with # are comments and will be ignored

# IAM_Service Configuration
IAM_SERVICE_REPO_URL=https://git.fsoft-academy.edu.vn/hcm25_cpl_net_08/team04/laboratorymanagementproject.git
IAM_SERVICE_BRANCH=develop

# Laboratory_Service Configuration
LABORATORY_SERVICE_REPO_URL=https://git.fsoft-academy.edu.vn/hcm25_cpl_net_08/team04/laboratoryservice.git
LABORATORY_SERVICE_BRANCH=develop

# Front_End Configuration
FRONT_END_REPO_URL=https://git.fsoft-academy.edu.vn/hcm25_cpl_net_08/team04/IAM_Service_FrontEnd.git
FRONT_END_BRANCH=develop
```

**Cách chỉnh sửa:**
- Mở file `git_config.txt` bằng bất kỳ text editor nào (Notepad, Notepad++, VS Code, v.v.)
- Thay đổi giá trị sau dấu `=` cho từng biến
- Dòng bắt đầu bằng `#` là comment và sẽ bị bỏ qua
- Lưu file và chạy lại `setup_project.bat`

### 3. `git_config.bat`
Script phụ trợ để đọc và load cấu hình từ `git_config.txt` vào các biến môi trường. Script này được gọi tự động bởi `setup_project.bat`.

### 4. `install_npm.bat`
Script cài đặt npm dependencies cho Frontend application.

## 🚀 Hướng dẫn Sử dụng

### Yêu cầu Hệ thống

Trước khi chạy script, đảm bảo bạn đã cài đặt:

- ✅ **Git** - Để clone repositories
- ✅ **.NET SDK** - Để build và restore packages cho các services
- ✅ **Node.js và npm** - Để cài đặt dependencies cho Frontend

### Các bước Thực hiện

1. **Cấu hình Git repositories** (nếu cần):
   - Mở file `git_config.txt`
   - Chỉnh sửa URL repository hoặc branch nếu cần
   - Lưu file

2. **Chạy script setup:**
   ```batch
   setup_project.bat
   ```

3. **Chờ script hoàn thành:**
   - Script sẽ tự động thực hiện tất cả các bước
   - Theo dõi tiến trình qua các thông báo trên màn hình
   - Thời gian hoàn thành phụ thuộc vào tốc độ mạng và kích thước repositories

4. **Kiểm tra kết quả:**
   - Thư mục `OJT_Laboratory_Project` sẽ được tạo tại cùng vị trí với script
   - Tất cả các services đã được clone và build thành công

## ⚙️ Cấu hình

### Thay đổi Git Repository

Để thay đổi repository hoặc branch cho một service:

1. Mở `git_config.txt`
2. Tìm dòng cấu hình cần thay đổi (ví dụ: `IAM_SERVICE_REPO_URL` hoặc `IAM_SERVICE_BRANCH`)
3. Cập nhật giá trị sau dấu `=`
4. Lưu file
5. Chạy lại `setup_project.bat`

**Ví dụ:**
```
IAM_SERVICE_REPO_URL=https://git.fsoft-academy.edu.vn/hcm25_cpl_net_08/team04/laboratorymanagementproject.git
IAM_SERVICE_BRANCH=main
```

### Thêm Service mới

Để thêm một service mới:

1. Thêm cấu hình vào `git_config.txt`:
   ```
   # New_Service Configuration
   NEW_SERVICE_REPO_URL=https://git.example.com/new-service.git
   NEW_SERVICE_BRANCH=develop
   ```

2. Cập nhật `git_config.bat` để load biến mới (nếu cần)
3. Cập nhật `setup_project.bat` để thêm các bước:
   - Tạo thư mục
   - Clone repository
   - Restore packages (nếu là .NET)
   - Build (nếu cần)

## 🔧 Xử lý Lỗi

### Lỗi: "git_config.txt not found"
- Đảm bảo file `git_config.txt` nằm cùng thư mục với `setup_project.bat`

### Lỗi: "Git clone failed"
- Kiểm tra kết nối mạng
- Xác minh URL repository trong `git_config.txt` là đúng
- Kiểm tra quyền truy cập repository

### Lỗi: "dotnet restore failed"
- Kiểm tra .NET SDK đã được cài đặt: `dotnet --version`
- Kiểm tra kết nối mạng để download NuGet packages

### Lỗi: "npm install failed"
- Kiểm tra Node.js đã được cài đặt: `node --version` và `npm --version`
- Kiểm tra kết nối mạng

## 📝 Ghi chú

- Script sẽ tạo thư mục `OJT_Laboratory_Project` tại vị trí hiện tại
- Nếu thư mục đã tồn tại, script sẽ tiếp tục clone vào các thư mục con
- Đảm bảo có đủ dung lượng ổ đĩa cho các repositories và dependencies
- Thời gian chạy script phụ thuộc vào tốc độ mạng và kích thước dự án

**Lưu ý:** Script này được thiết kế cho môi trường Windows. Để sử dụng trên Linux/Mac, cần chuyển đổi sang shell script tương ứng.
