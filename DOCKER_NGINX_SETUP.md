# 🐳 Docker + Nginx Setup Guide

Hướng dẫn triển khai OJT Laboratory Project với Docker và Nginx.

## 📋 Mục Lục

1. [Yêu Cầu Hệ Thống](#1-yêu-cầu-hệ-thống)
2. [Cài Đặt](#2-cài-đặt)
3. [Cấu Hình Database](#3-cấu-hình-database)
4. [Chạy Services](#4-chạy-services)
5. [Cấu Trúc Project](#5-cấu-trúc-project)
6. [Troubleshooting](#6-troubleshooting)

---

## 1. Yêu Cầu Hệ Thống

- **Docker**: Version 20.10 trở lên
- **Docker Compose**: Version 2.0 trở lên
- **PostgreSQL Database**: Online (Render) hoặc Local
- **Ports**: 80, 443, 5672, 15672 (RabbitMQ management)

### Kiểm tra Docker

```bash
docker --version
docker-compose --version
```

---

## 2. Cài Đặt

### 2.1. Clone Repository

```bash
git clone https://github.com/ThinhTran2412/OJT.git
cd OJT/Deploy
```

### 2.2. Tạo `.env` File (Optional)

Tạo file `.env` ở root directory để cấu hình database connection string:

```env
# Database Connection String (PostgreSQL)
# Option 1: Online Database (Render)
DATABASE_CONNECTION_STRING=Host=dpg-d4fcsm95pdvs73ader70-a.singapore-postgres.render.com;Port=5432;Database=laboratory_service;Username=laboratory_service_user;Password=geeqHh8B6xA8oQNkNHw0K0AoJKSZhji2;Ssl Mode=Require;Trust Server Certificate=true

# Option 2: Local Database
# DATABASE_CONNECTION_STRING=Host=host.docker.internal;Port=5432;Database=OJT_Laboratory_DB;Username=postgres;Password=12345
```

**Lưu ý:**
- **Option 1 (Online Database)**: Tận dụng database online từ Render, bạn bè có thể dùng được ngay
- **Option 2 (Local Database)**: Dùng database local, cần cài đặt PostgreSQL trên máy

---

## 3. Cấu Hình Database

### 3.1. Option 1: Online Database (Render) - Khuyến nghị ✅

**Ưu điểm:**
- Không cần cài đặt PostgreSQL
- Bạn bè có thể truy cập ngay
- Database luôn sẵn sàng

**Cấu hình:**
1. Sử dụng connection string mặc định từ Render (đã cấu hình sẵn trong `docker-compose.yml`)
2. Hoặc set environment variable trong `.env` file:

```env
DATABASE_CONNECTION_STRING=Host=dpg-d4fcsm95pdvs73ader70-a.singapore-postgres.render.com;Port=5432;Database=laboratory_service;Username=laboratory_service_user;Password=geeqHh8B6xA8oQNkNHw0K0AoJKSZhji2;Ssl Mode=Require;Trust Server Certificate=true
```

**Database Schemas:**
- `iam_service` - IAM Service
- `laboratory_service` - Laboratory Service
- `monitoring_service` - Monitoring Service
- `simulator_service` - Simulator Service

### 3.2. Option 2: Local Database

**Ưu điểm:**
- Hoàn toàn offline
- Full control over database

**Cài đặt PostgreSQL:**

1. **Windows:**
   - Download và cài đặt từ: https://www.postgresql.org/download/windows/
   - Hoặc dùng Docker:
   ```bash
   docker run --name postgres -e POSTGRES_PASSWORD=12345 -e POSTGRES_DB=OJT_Laboratory_DB -p 5432:5432 -d postgres:15
   ```

2. **Tạo Database:**
   ```sql
   CREATE DATABASE "OJT_Laboratory_DB";
   ```

3. **Cấu hình Connection String trong `.env`:**
   ```env
   DATABASE_CONNECTION_STRING=Host=host.docker.internal;Port=5432;Database=OJT_Laboratory_DB;Username=postgres;Password=12345
   ```

**Lưu ý:** `host.docker.internal` cho phép Docker containers kết nối đến PostgreSQL trên host machine.

### 3.3. Chạy Migrations

Sau khi cấu hình database, chạy migrations để tạo tables:

```bash
# Development migrations
cd Scripts_Database_Dev
update_databases_dev.bat

# Hoặc Production migrations (nếu dùng Render database)
cd Scripts_Database_Pro
update_databases_prod.bat
```

---

## 4. Chạy Services

### 4.1. Build và Start Services

```bash
# Build và start tất cả services
docker-compose up -d --build

# Xem logs
docker-compose logs -f

# Xem logs của service cụ thể
docker-compose logs -f iam-service
docker-compose logs -f laboratory-service
```

### 4.2. Kiểm tra Services

```bash
# Kiểm tra tất cả containers đang chạy
docker-compose ps

# Kiểm tra health của services
curl http://localhost/health
```

### 4.3. Stop Services

```bash
# Stop tất cả services
docker-compose down

# Stop và xóa volumes (cẩn thận - sẽ xóa dữ liệu)
docker-compose down -v
```

---

## 5. Cấu Trúc Project

```
Deploy/
├── docker-compose.yml          # Docker Compose configuration
├── .env                        # Environment variables (optional)
├── nginx/
│   ├── nginx.conf              # Nginx main configuration
│   └── conf.d/
│       └── default.conf        # Nginx service routing
├── OJT_Laboratory_Project/
│   ├── IAM_Service/
│   │   └── IAM_Service.API/
│   │       ├── Dockerfile
│   │       └── appsettings.json
│   ├── Laboratory_Service/
│   │   └── Laboratory_Service.API/
│   │       ├── Dockerfile
│   │       └── appsettings.json
│   ├── Monitoring_Service/
│   │   └── Monitoring_Service.API/
│   │       ├── Dockerfile
│   │       └── appsettings.json
│   ├── Simulator_Service/
│   │   └── Simulator.API/
│   │       ├── Dockerfile
│   │       └── appsettings.json
│   └── Front_End/
│       └── (Frontend sẽ deploy riêng lên Render)
└── Scripts_Database_Dev/       # Database migration scripts
```

### 5.1. Service Ports

| Service | Internal Port | External Port (via Nginx) |
|---------|--------------|---------------------------|
| Nginx | 80 | 80 (HTTP) |
| IAM Service | 8080 | - |
| Laboratory Service | 8080 | - |
| Monitoring Service | 8080 | - |
| Simulator Service | 8080 | - |
| RabbitMQ | 5672 | 5672 |
| RabbitMQ Management | 15672 | 15672 |

### 5.2. API Routes (qua Nginx)

- **IAM Service**: `/api/Auth`, `/api/User`, `/api/Role`, `/api/EventLog`, `/api/PatientInfo`, `/api/Registers`
- **Laboratory Service**: `/api/Patient`, `/api/TestOrder`, `/api/TestResult`, `/api/MedicalRecord`, `/api/ai-review`
- **Monitoring Service**: `/api/Monitoring`
- **Simulator Service**: `/api/Simulator`

### 5.3. gRPC Endpoints

- **IAM Service gRPC**: `http://localhost/IAM_Service.API.Protos.UserService`
- **Laboratory Service gRPC**: `http://localhost/Laboratory_Service.API.Protos.PatientService`

**Lưu ý:** gRPC endpoints chỉ dùng cho inter-service communication, không expose ra public qua Nginx.

---

## 6. Troubleshooting

### 6.1. Service không start

**Kiểm tra logs:**
```bash
docker-compose logs [service-name]
```

**Common issues:**
- **Database connection failed**: Kiểm tra connection string trong `.env` hoặc `docker-compose.yml`
- **Port already in use**: Stop service đang dùng port hoặc đổi port trong `docker-compose.yml`
- **Build failed**: Kiểm tra Dockerfile và dependencies

### 6.2. Nginx không route requests

**Kiểm tra Nginx config:**
```bash
docker-compose exec nginx nginx -t
```

**Reload Nginx:**
```bash
docker-compose exec nginx nginx -s reload
```

### 6.3. gRPC connection failed

**Kiểm tra:**
1. Services đã start thành công
2. Service names trong `appsettings.json` đúng với docker-compose service names
3. Network `ojt_network` đã được tạo

**Test gRPC connection:**
```bash
docker-compose exec laboratory-service curl -v http://iam-service:8080
```

### 6.4. Database connection issues

**Option 1 (Online Database):**
- Kiểm tra connection string có đúng không
- Kiểm tra network connection đến Render
- Kiểm tra SSL settings (`Ssl Mode=Require;Trust Server Certificate=true`)

**Option 2 (Local Database):**
- Kiểm tra PostgreSQL đang chạy
- Kiểm tra `host.docker.internal` có hoạt động không (Windows/Mac)
- Kiểm tra firewall settings

### 6.5. RabbitMQ connection issues

**Kiểm tra RabbitMQ:**
```bash
docker-compose logs rabbitmq
```

**Access RabbitMQ Management:**
- URL: `http://localhost:15672`
- Username: `guest`
- Password: `guest`

---

## 7. Frontend Deployment

Frontend sẽ được deploy riêng lên Render (Static Site):

1. **Build Frontend:**
   ```bash
   cd OJT_Laboratory_Project/Front_End
   npm install
   npm run build
   ```

2. **Deploy lên Render:**
   - Service Type: **Static Site**
   - Repository: `https://github.com/ThinhTran2412/OJT`
   - Root Directory: `OJT_Laboratory_Project/Front_End`
   - Build Command: `npm install; npm run build`
   - Publish Directory: `dist`

3. **Cấu hình API Base URL:**
   - Update `src/services/api.js` để trỏ đến Nginx proxy: `http://your-nginx-ip/api`

---

## 8. Production Deployment

### 8.1. Với Online Database (Render) - Khuyến nghị ✅

1. **Setup Nginx trên Server:**
   - Cài đặt Docker và Docker Compose
   - Clone repository
   - Tạo `.env` file với Render database connection string
   - Chạy `docker-compose up -d --build`

2. **Setup Domain (Optional):**
   - Point domain đến server IP
   - Cấu hình SSL certificate (Let's Encrypt) cho Nginx

3. **Deploy Frontend:**
   - Deploy lên Render (Static Site)
   - Update API base URL trong `api.js`

### 8.2. Với Local Database

1. **Setup PostgreSQL trên Server:**
   - Cài đặt PostgreSQL
   - Tạo database và schemas
   - Run migrations

2. **Setup Docker Services:**
   - Cài đặt Docker và Docker Compose
   - Clone repository
   - Tạo `.env` file với local database connection string
   - Chạy `docker-compose up -d --build`

---

## 9. Quick Commands

```bash
# Start all services
docker-compose up -d

# Stop all services
docker-compose down

# Rebuild and restart
docker-compose up -d --build

# View logs
docker-compose logs -f

# View logs of specific service
docker-compose logs -f iam-service

# Check service status
docker-compose ps

# Execute command in container
docker-compose exec iam-service sh

# Restart specific service
docker-compose restart iam-service

# Remove all containers and volumes
docker-compose down -v
```

---

## 10. Support

Nếu gặp vấn đề, kiểm tra:
1. Docker và Docker Compose đã được cài đặt đúng
2. Ports 80, 443, 5672, 15672 không bị chiếm
3. Database connection string đúng
4. Network connection ổn định (nếu dùng online database)

---

**Chúc bạn deploy thành công! 🚀**

