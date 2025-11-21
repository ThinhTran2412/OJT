# 🐳 Docker Setup Guide - OJT Laboratory Project

Hướng dẫn setup và chạy toàn bộ backend services với Docker + Nginx, sử dụng database online từ Render.

## 📋 Mục Lục

1. [Yêu Cầu](#1-yêu-cầu)
2. [Cài Đặt Nhanh](#2-cài-đặt-nhanh)
3. [Chi Tiết Cấu Hình](#3-chi-tiết-cấu-hình)
4. [Chạy Services](#4-chạy-services)
5. [Kiểm Tra Services](#5-kiểm-tra-services)
6. [API Endpoints](#6-api-endpoints)
7. [Troubleshooting](#7-troubleshooting)

---

## 1. Yêu Cầu

- **Docker Desktop**: Version 20.10 trở lên
- **Docker Compose**: Version 2.0 trở lên (thường đi kèm với Docker Desktop)
- **Ports**: 80, 443, 5672, 15672 (phải trống)

### Kiểm tra Docker đã cài đặt

```bash
docker --version
docker-compose --version
```

Nếu chưa cài, download tại: https://www.docker.com/products/docker-desktop

---

## 2. Cài Đặt Nhanh

### Bước 1: Clone Repository

```bash
git clone https://github.com/ThinhTran2412/OJT.git
cd OJT/Deploy
```

### Bước 2: Tạo File `.env`

Tạo file `.env` ở thư mục `Deploy` (cùng cấp với `docker-compose.yml`):

```bash
# Copy từ file .env.example
copy .env.example .env
```

Hoặc tạo thủ công file `.env` với nội dung:

```env
DATABASE_CONNECTION_STRING=Host=dpg-d4fcsm95pdvs73ader70-a.singapore-postgres.render.com;Port=5432;Database=laboratory_service;Username=laboratory_service_user;Password=geeqHh8B6xA8oQNkNHw0K0AoJKSZhji2;SSL Mode=Require;Trust Server Certificate=true
```

**Lưu ý:** File `.env` chứa thông tin nhạy cảm, không commit lên Git.

### Bước 3: Chạy Services

```bash
# Build và start tất cả services
docker-compose up -d --build

# Xem logs
docker-compose logs -f
```

### Bước 4: Kiểm Tra

Mở trình duyệt và truy cập:
- **Nginx Proxy**: http://localhost
- **Health Check**: http://localhost/health
- **RabbitMQ Management**: http://localhost:15672 (guest/guest)

---

## 3. Chi Tiết Cấu Hình

### 3.1. Database Connection

Project sử dụng **PostgreSQL database online từ Render**, đã có sẵn dữ liệu.

**Connection String** được cấu hình trong:
- File `.env` (cho Docker Compose)
- File `appsettings.Production.json` (cho mỗi service)

**Database Schemas:**
- `iam_service` - IAM Service
- `laboratory_service` - Laboratory Service
- `monitoring_service` - Monitoring Service
- `simulator_service` - Simulator Service

### 3.2. Services Configuration

Tất cả services được cấu hình để:
- Chạy trên port **8080** (internal, không expose ra ngoài)
- Giao tiếp với nhau qua Docker network (service names: `iam-service`, `laboratory-service`, etc.)
- Sử dụng `ASPNETCORE_ENVIRONMENT=Production` để load `appsettings.Production.json`

### 3.3. Nginx Reverse Proxy

Nginx đóng vai trò reverse proxy, route requests đến đúng services:

- **Port 80**: HTTP (main entry point)
- **Port 443**: HTTPS (nếu cần, có thể cấu hình sau)

**API Routes:**
- `/api/Auth`, `/api/User`, `/api/Role`, etc. → **IAM Service**
- `/api/Patient`, `/api/TestOrder`, etc. → **Laboratory Service**
- `/api/Monitoring` → **Monitoring Service**
- `/api/Simulator` → **Simulator Service**

### 3.4. RabbitMQ

RabbitMQ được sử dụng cho message queue giữa services:

- **Port 5672**: AMQP protocol (internal)
- **Port 15672**: Management UI (expose ra localhost)

**Access:** http://localhost:15672
- Username: `guest`
- Password: `guest`

---

## 4. Chạy Services

### 4.1. Start All Services

```bash
docker-compose up -d --build
```

**Giải thích:**
- `up`: Start services
- `-d`: Run in background (detached mode)
- `--build`: Build images trước khi start

### 4.2. Xem Logs

```bash
# Xem logs của tất cả services
docker-compose logs -f

# Xem logs của service cụ thể
docker-compose logs -f iam-service
docker-compose logs -f laboratory-service
docker-compose logs -f nginx
```

### 4.3. Stop Services

```bash
# Stop tất cả services (giữ lại containers)
docker-compose stop

# Stop và xóa containers (giữ lại volumes)
docker-compose down

# Stop và xóa tất cả (containers, volumes, networks)
docker-compose down -v
```

### 4.4. Restart Service

```bash
# Restart tất cả services
docker-compose restart

# Restart service cụ thể
docker-compose restart iam-service
```

---

## 5. Kiểm Tra Services

### 5.1. Kiểm Tra Containers Đang Chạy

```bash
docker-compose ps
```

Kết quả mong đợi:
```
NAME                  STATUS              PORTS
iam_service           Up (healthy)        8080/tcp
laboratory_service    Up (healthy)        8080/tcp
monitoring_service    Up (healthy)        8080/tcp
simulator_service     Up (healthy)        8080/tcp
nginx_proxy           Up                  0.0.0.0:80->80/tcp
rabbitmq              Up (healthy)        5672/tcp, 0.0.0.0:15672->15672/tcp
```

### 5.2. Kiểm Tra Health Endpoints

```bash
# Health check của Nginx
curl http://localhost/health

# Kiểm tra IAM Service (thông qua Nginx)
curl http://localhost/api/Auth/health

# Kiểm tra Laboratory Service (thông qua Nginx)
curl http://localhost/api/Patient
```

### 5.3. Kiểm Tra Database Connection

Vào logs của services để xem có kết nối database thành công không:

```bash
docker-compose logs iam-service | grep -i "database\|connection\|migration"
```

---

## 6. API Endpoints

### 6.1. IAM Service

**Base URL:** `http://localhost/api`

**Endpoints:**
- `/api/Auth/connect/token` - Login
- `/api/User` - User management
- `/api/Role` - Role management
- `/api/EventLog` - Event logs
- `/api/PatientInfo` - Patient info (gRPC)
- `/api/Registers` - Registration

### 6.2. Laboratory Service

**Base URL:** `http://localhost/api`

**Endpoints:**
- `/api/Patient` - Patient management
- `/api/TestOrder` - Test order management
- `/api/TestResult` - Test result management
- `/api/MedicalRecord` - Medical record management
- `/api/ai-review` - AI review

### 6.3. Monitoring Service

**Base URL:** `http://localhost/api`

**Endpoints:**
- `/api/Monitoring` - Monitoring data

### 6.4. Simulator Service

**Base URL:** `http://localhost/api`

**Endpoints:**
- `/api/Simulator` - Simulator operations

### 6.5. Swagger UI

Mỗi service có Swagger UI (nếu enable trong Production):

- IAM Service: `http://localhost/swagger` (qua Nginx proxy)
- Hoặc trực tiếp vào container: `http://iam-service:8080/swagger`

---

## 7. Troubleshooting

### 7.1. Service Không Start

**Kiểm tra logs:**
```bash
docker-compose logs [service-name]
```

**Common issues:**

1. **Port đã được sử dụng:**
   ```
   Error: bind: address already in use
   ```
   **Giải pháp:** Đổi port trong `docker-compose.yml` hoặc stop service đang dùng port

2. **Database connection failed:**
   ```
   Error: Connection refused / Connection timeout
   ```
   **Giải pháp:** 
   - Kiểm tra connection string trong `.env`
   - Kiểm tra network connection đến Render database
   - Kiểm tra firewall settings

3. **Build failed:**
   ```
   Error: failed to solve
   ```
   **Giải pháp:**
   - Kiểm tra Dockerfile
   - Kiểm tra dependencies trong csproj files
   - Clear Docker cache: `docker system prune -a`

### 7.2. Nginx Không Route Đúng

**Kiểm tra Nginx config:**
```bash
docker-compose exec nginx nginx -t
```

**Reload Nginx:**
```bash
docker-compose exec nginx nginx -s reload
```

**Xem Nginx logs:**
```bash
docker-compose logs nginx
```

### 7.3. Services Không Giao Tiếp Được

**Kiểm tra network:**
```bash
docker network ls
docker network inspect deploy_ojt_network
```

**Test connectivity giữa services:**
```bash
# Test IAM Service có reach được Laboratory Service không
docker-compose exec iam-service wget -O- http://laboratory-service:8080/health
```

### 7.4. Database Connection Issues

**Kiểm tra connection string:**
```bash
# Xem environment variable
docker-compose exec iam-service env | grep DATABASE
```

**Test connection:**
```bash
# Test kết nối từ container
docker-compose exec iam-service ping dpg-d4fcsm95pdvs73ader70-a.singapore-postgres.render.com
```

### 7.5. RabbitMQ Connection Issues

**Kiểm tra RabbitMQ:**
```bash
docker-compose logs rabbitmq
```

**Access Management UI:**
- URL: http://localhost:15672
- Username: `guest`
- Password: `guest`

**Test connection:**
```bash
docker-compose exec monitoring-service ping rabbitmq
```

---

## 8. Quick Commands Reference

```bash
# Start services
docker-compose up -d --build

# Stop services
docker-compose stop

# Stop and remove
docker-compose down

# View logs
docker-compose logs -f

# View logs of specific service
docker-compose logs -f iam-service

# Restart service
docker-compose restart iam-service

# Check status
docker-compose ps

# Execute command in container
docker-compose exec iam-service sh

# Rebuild specific service
docker-compose build iam-service
docker-compose up -d iam-service

# Remove all (containers, volumes, networks)
docker-compose down -v

# View network
docker network ls
docker network inspect deploy_ojt_network
```

---

## 9. Cấu Trúc Files

```
Deploy/
├── docker-compose.yml              # Docker Compose configuration
├── .env                           # Environment variables (database connection)
├── .env.example                   # Example environment file
├── nginx/
│   ├── nginx.conf                 # Nginx main configuration
│   └── conf.d/
│       └── default.conf           # Nginx service routing
├── OJT_Laboratory_Project/
│   ├── IAM_Service/
│   │   └── IAM_Service.API/
│   │       ├── Dockerfile
│   │       ├── appsettings.json
│   │       └── appsettings.Production.json
│   ├── Laboratory_Service/
│   │   └── Laboratory_Service.API/
│   │       ├── Dockerfile
│   │       ├── appsettings.json
│   │       └── appsettings.Production.json
│   ├── Monitoring_Service/
│   │   └── Monitoring_Service.API/
│   │       ├── Dockerfile
│   │       ├── appsettings.json
│   │       └── appsettings.Production.json
│   └── Simulator_Service/
│       └── Simulator.API/
│           ├── Dockerfile
│           ├── appsettings.json
│           └── appsettings.Production.json
└── DOCKER_SETUP.md                # This file
```

---

## 10. Next Steps

1. **Frontend Setup:**
   - Frontend đã được tách sang repo riêng: https://github.com/ThinhTran2412/Front_End_OJT.git
   - Có thể chạy local với Vite dev server: `npm run dev`
   - Hoặc deploy lên Render như Static Site

2. **API Base URL:**
   - Development: `http://localhost/api`
   - Production: Update trong `src/services/api.js` của frontend

3. **SSL/HTTPS (Optional):**
   - Có thể setup Let's Encrypt cho Nginx
   - Hoặc dùng self-signed certificate cho local development

---

## 11. Support

Nếu gặp vấn đề:
1. Kiểm tra logs: `docker-compose logs -f`
2. Kiểm tra health: `docker-compose ps`
3. Kiểm tra network: `docker network inspect deploy_ojt_network`
4. Xem Troubleshooting section ở trên

---

**Chúc bạn setup thành công! 🚀**

