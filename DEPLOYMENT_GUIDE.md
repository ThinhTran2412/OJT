# 🚀 OJT Laboratory Project - Deployment Guide

Hướng dẫn đầy đủ để deploy OJT Laboratory Project lên Render.

## 📋 Mục Lục

1. [Cấu hình Environment](#1-cấu-hình-environment)
2. [Database Setup](#2-database-setup)
3. [Backend Services Deployment](#3-backend-services-deployment)
4. [Frontend Deployment](#4-frontend-deployment)
5. [Environment Variables](#5-environment-variables)
6. [Troubleshooting](#6-troubleshooting)

---

## 1. Cấu hình Environment

### 1.1. Development (Local)

**Ports Configuration:**

| Service | HTTP | HTTPS | gRPC |
|---------|------|-------|------|
| IAM_Service | 5044 | 7155 | 7001 |
| Laboratory_Service | 5002 | 7157 | 7002 |
| Monitoring_Service | 5004 | 7159 | - |
| Simulator_Service | 5003 | 7158 | 7003 |

**Configuration Files:**
- `appsettings.json` - Default configuration
- `appsettings.Development.json` - Development overrides (localhost)

### 1.2. Production (Render)

**Configuration Files:**
- `appsettings.Production.json` - Production overrides (Render database)

**Port Configuration:**
- Services sử dụng `PORT` environment variable từ Render
- Kestrel được cấu hình tự động trong `Program.cs`

---

## 2. Database Setup

### 2.1. Database Information

**Render PostgreSQL Database:**
- **Internal URL:** `postgresql://laboratory_service_user:geeqHh8B6xA8oQNkNHw0K0AoJKSZhji2@dpg-d4fcsm95pdvs73ader70-a/laboratory_service`
- **External URL:** `postgresql://laboratory_service_user:geeqHh8B6xA8oQNkNHw0K0AoJKSZhji2@dpg-d4fcsm95pdvs73ader70-a.singapore-postgres.render.com/laboratory_service`

**Database Schemas:**
- `iam_service` - IAM_Service
- `laboratory_service` - Laboratory_Service
- `monitoring_service` - Monitoring_Service
- `simulator_service` - Simulator_Service

### 2.2. Chạy Migrations

**Development (Local Database):**
```batch
cd Deploy
update_databases_dev.bat
```

**Production (Render Database):**
```batch
cd Deploy
update_databases_prod.bat
```

> 📖 Xem chi tiết trong [DATABASE_SCRIPTS_GUIDE.md](./DATABASE_SCRIPTS_GUIDE.md)

---

## 3. Backend Services Deployment

### 3.1. Repository Configuration

**Repository:** `https://github.com/ThinhTran2412/OJT`  
**Branch:** `master`  
**Type:** Monorepo (tất cả services trong 1 repository)

### 3.2. IAM_Service

**Service Type:** Web Service (Docker)  
**Root Directory:** `OJT_Laboratory_Project/IAM_Service`  
**Dockerfile Path:** `IAM_Service.API/Dockerfile`  
**Dockerfile Location:** `OJT_Laboratory_Project/IAM_Service/IAM_Service.API/Dockerfile`  
**.dockerignore Location:** `OJT_Laboratory_Project/IAM_Service/IAM_Service.API/.dockerignore`

**Build Configuration:**
- **Build Command:** (Render tự động build từ Dockerfile)
- **Start Command:** (Không cần - Dockerfile đã có ENTRYPOINT)
- **Docker Build Context:** Root Directory (`OJT_Laboratory_Project/IAM_Service`)

**Dockerfile Entry Point:**
```dockerfile
ENTRYPOINT ["dotnet", "IAM_Service.API.dll"]
```

**Environment Variables:**
- `DATABASE_URL` - Tự động từ Render database (nếu cùng Project)
- `PORT` - Tự động từ Render (default: 8080)

**Files Structure:**
```
OJT_Laboratory_Project/IAM_Service/
├── IAM_Service.API/
│   ├── Dockerfile          # Docker build file
│   ├── .dockerignore       # Docker ignore patterns
│   └── Program.cs          # Main entry point
├── NuGet.config            # NuGet package configuration
└── nuget/                  # Local NuGet packages
```

**Deploy URL:** `https://iam-service-fz3h.onrender.com`

---

### 3.3. Laboratory_Service

**Service Type:** Web Service (Docker)  
**Root Directory:** `OJT_Laboratory_Project/Laboratory_Service`  
**Dockerfile Path:** `Laboratory_Service.API/Dockerfile`  
**Dockerfile Location:** `OJT_Laboratory_Project/Laboratory_Service/Laboratory_Service.API/Dockerfile`  
**.dockerignore Location:** `OJT_Laboratory_Project/Laboratory_Service/Laboratory_Service.API/.dockerignore`

**Build Configuration:**
- **Build Command:** (Render tự động build từ Dockerfile)
- **Start Command:** (Không cần - Dockerfile đã có ENTRYPOINT)
- **Docker Build Context:** Root Directory (`OJT_Laboratory_Project/Laboratory_Service`)

**Dockerfile Entry Point:**
```dockerfile
ENTRYPOINT ["dotnet", "Laboratory_Service.API.dll"]
```

**Environment Variables:**
- `DATABASE_URL` - Tự động từ Render database (nếu cùng Project)
- `PORT` - Tự động từ Render (default: 8080)

**Files Structure:**
```
OJT_Laboratory_Project/Laboratory_Service/
├── Laboratory_Service.API/
│   ├── Dockerfile          # Docker build file
│   ├── .dockerignore       # Docker ignore patterns
│   └── Program.cs          # Main entry point
├── NuGet.config            # NuGet package configuration
└── nuget/                  # Local NuGet packages
```

---

### 3.4. Monitoring_Service

**Service Type:** Web Service (Docker)  
**Root Directory:** `OJT_Laboratory_Project/Monitoring_Service`  
**Dockerfile Path:** `Monitoring_Service.API/Dockerfile`  
**Dockerfile Location:** `OJT_Laboratory_Project/Monitoring_Service/Monitoring_Service.API/Dockerfile`  
**.dockerignore Location:** `OJT_Laboratory_Project/Monitoring_Service/Monitoring_Service.API/.dockerignore`

**Build Configuration:**
- **Build Command:** (Render tự động build từ Dockerfile)
- **Start Command:** (Không cần - Dockerfile đã có ENTRYPOINT)
- **Docker Build Context:** Root Directory (`OJT_Laboratory_Project/Monitoring_Service`)

**Dockerfile Entry Point:**
```dockerfile
ENTRYPOINT ["dotnet", "Monitoring_Service.API.dll"]
```

**Environment Variables:**
- `DATABASE_URL` - Tự động từ Render database (nếu cùng Project)
- `PORT` - Tự động từ Render (default: 8080)

**Files Structure:**
```
OJT_Laboratory_Project/Monitoring_Service/
├── Monitoring_Service.API/
│   ├── Dockerfile          # Docker build file
│   ├── .dockerignore       # Docker ignore patterns
│   └── Program.cs          # Main entry point
```

---

### 3.5. Simulator_Service

**Service Type:** Web Service (Docker)  
**Root Directory:** `OJT_Laboratory_Project/Simulator_Service`  
**Dockerfile Path:** `Simulator.API/Dockerfile`  
**Dockerfile Location:** `OJT_Laboratory_Project/Simulator_Service/Simulator.API/Dockerfile`  
**.dockerignore Location:** `OJT_Laboratory_Project/Simulator_Service/Simulator.API/.dockerignore`

**Build Configuration:**
- **Build Command:** (Render tự động build từ Dockerfile)
- **Start Command:** (Không cần - Dockerfile đã có ENTRYPOINT)
- **Docker Build Context:** Root Directory (`OJT_Laboratory_Project/Simulator_Service`)

**Dockerfile Entry Point:**
```dockerfile
ENTRYPOINT ["dotnet", "Simulator.API.dll"]
```

**Environment Variables:**
- `DATABASE_URL` - Tự động từ Render database (nếu cùng Project)
- `PORT` - Tự động từ Render (default: 8080)
- `RabbitMQ:HostName` - RabbitMQ server hostname (từ appsettings.json)

**Files Structure:**
```
OJT_Laboratory_Project/Simulator_Service/
├── Simulator.API/
│   ├── Dockerfile          # Docker build file
│   ├── .dockerignore       # Docker ignore patterns
│   └── Program.cs          # Main entry point
```

---

## 4. Frontend Deployment

### 4.1. Service Configuration

**Service Type:** Static Site  
**Root Directory:** `OJT_Laboratory_Project/Front_End`  
**Build Command:** `npm install; npm run build`  
**Publish Directory:** `dist`  
**Start Command:** (Để trống - Static Site không cần start command)

### 4.1.1. File Structure

**Important Files:**
- `package.json` - Node.js dependencies và scripts
- `vite.config.js` - Vite build configuration
- `public/_redirects` - Routing configuration cho React Router
- `src/services/api.js` - API client configuration

**Build Output:**
- `dist/` - Directory chứa built static files (HTML, CSS, JS)

**package.json Scripts:**
```json
{
  "scripts": {
    "dev": "vite",              # Development server
    "build": "vite build",      # Production build
    "preview": "vite preview"   # Preview production build
  }
}
```

### 4.1.2. Build Process

**Build Command (Render tự động chạy):**
```bash
npm install; npm run build
```

**Build Output Location:**
- Publish Directory: `dist`
- Main HTML file: `dist/index.html`
- Static assets: `dist/assets/`

### 4.2. Environment Variables

**Trong Render Dashboard → Environment (Optional - có default values):**
- `VITE_IAM_SERVICE_URL` = `https://iam-service-fz3h.onrender.com` (Default)
- `VITE_LABORATORY_SERVICE_URL` = `https://laboratory-service.onrender.com` (Default)

**Lưu ý:** Frontend tự động route requests đến đúng service dựa trên endpoint:
- IAM Service: `/api/Auth`, `/api/User`, `/api/Role`, `/api/EventLog`, `/api/PatientInfo`, `/api/Registers`
- Laboratory Service: `/api/Patient`, `/api/TestOrder`, `/api/TestResult`, `/api/MedicalRecord`, `/api/ai-review`

### 4.3. Routing Configuration

**File:** `OJT_Laboratory_Project/Front_End/public/_redirects`

**Content:**
```
/*    /index.html   200
```

File này tự động được Render nhận diện để handle client-side routing (React Router).

**Manual Configuration (nếu cần):**
1. Vào Render Dashboard → Frontend Service → Settings
2. Tìm "Redirects/Rewrites"
3. Add: `/*` → `/index.html` (Status: 200)

**Deploy URL:** `https://front-end-fnfs.onrender.com`

---

## 5. Environment Variables

### 5.1. Backend Services

#### Automatic (Khuyến nghị)
- Nếu database và services trong cùng Render Project:
  - Render tự động set `DATABASE_URL`
  - Không cần cấu hình thủ công

#### Manual (Nếu cần)
**Key:** `DATABASE_URL`  
**Value:** Internal Database URL (nếu cùng region) hoặc External Database URL

### 5.2. Frontend

| Key | Value | Description |
|-----|-------|-------------|
| `VITE_API_BASE_URL` | `https://iam-service-fz3h.onrender.com` | Base URL cho API calls |
| `VITE_AUTH_API_URL` | `https://iam-service-fz3h.onrender.com` | Auth service URL |
| `VITE_PATIENT_API_URL` | `https://laboratory-service-url.onrender.com` | Patient/Lab service URL |

---

## 6. Troubleshooting

### 6.1. Backend Issues

#### HTTPS Configuration Error
**Error:** `Unable to configure HTTPS endpoint`

**Solution:**
- Đảm bảo `appsettings.Production.json` chỉ cấu hình HTTP endpoints
- Kestrel được cấu hình trong `Program.cs` để sử dụng `PORT` env var

#### Database Connection Error
**Error:** `PostgresException: relation "schema.table" does not exist`

**Solution:**
- Chạy migrations: `update_databases_prod.bat`
- Kiểm tra connection string trong `appsettings.Production.json`
- Kiểm tra database đang hoạt động trên Render

#### CORS Error
**Error:** `Access-Control-Allow-Origin header is missing`

**Solution:**
- Đảm bảo frontend URL được thêm vào `Cors:AllowedOrigins` trong `appsettings.Production.json`
- Kiểm tra CORS middleware được add trong `Program.cs`

### 6.2. Frontend Issues

#### "Not Found" khi truy cập routes
**Solution:**
- Đảm bảo file `public/_redirects` tồn tại với content: `/*    /index.html   200`
- Hoặc cấu hình redirects/rewrites trong Render Dashboard

#### API calls trả về 404
**Solution:**
- Kiểm tra `VITE_API_BASE_URL` environment variable
- Đảm bảo `/api` prefix được tự động thêm vào routes (đã có trong `api.js` interceptor)
- Kiểm tra backend service đang chạy

#### Build Errors
**Error:** `Missing script: "start"`

**Solution:**
- Đảm bảo service type là **Static Site** (không phải Web Service)
- **Start Command** phải để trống

---

## 7. Cấu hình Microservices

### 7.1. IAM_Service

**GrpcSettings:**
- Development: `http://localhost:7002`
- Production: `http://laboratory-service-onrender:7002`

**IAMService Config:**
- Development: `https://localhost:7155`
- Production: `https://iam-service-fz3h.onrender.com`

### 7.2. Laboratory_Service

**IAMService Config:**
- Development: `https://localhost:7155`
- Production: `https://iam-service-fz3h.onrender.com`

---

## 8. Quick Reference - Render Dashboard Configuration

### 8.1. Backend Services Configuration

#### IAM_Service

| Setting | Value |
|---------|-------|
| **Service Type** | Web Service |
| **Repository** | `https://github.com/ThinhTran2412/OJT` |
| **Branch** | `master` |
| **Root Directory** | `OJT_Laboratory_Project/IAM_Service` |
| **Dockerfile Path** | `IAM_Service.API/Dockerfile` |
| **Build Command** | (Để trống - Render tự động build Docker) |
| **Start Command** | (Để trống - Dockerfile đã có ENTRYPOINT) |
| **Environment** | Docker |

#### Laboratory_Service

| Setting | Value |
|---------|-------|
| **Service Type** | Web Service |
| **Repository** | `https://github.com/ThinhTran2412/OJT` |
| **Branch** | `master` |
| **Root Directory** | `OJT_Laboratory_Project/Laboratory_Service` |
| **Dockerfile Path** | `Laboratory_Service.API/Dockerfile` |
| **Build Command** | (Để trống - Render tự động build Docker) |
| **Start Command** | (Để trống - Dockerfile đã có ENTRYPOINT) |
| **Environment** | Docker |

#### Monitoring_Service

| Setting | Value |
|---------|-------|
| **Service Type** | Web Service |
| **Repository** | `https://github.com/ThinhTran2412/OJT` |
| **Branch** | `master` |
| **Root Directory** | `OJT_Laboratory_Project/Monitoring_Service` |
| **Dockerfile Path** | `Monitoring_Service.API/Dockerfile` |
| **Build Command** | (Để trống - Render tự động build Docker) |
| **Start Command** | (Để trống - Dockerfile đã có ENTRYPOINT) |
| **Environment** | Docker |

#### Simulator_Service

| Setting | Value |
|---------|-------|
| **Service Type** | Web Service |
| **Repository** | `https://github.com/ThinhTran2412/OJT` |
| **Branch** | `master` |
| **Root Directory** | `OJT_Laboratory_Project/Simulator_Service` |
| **Dockerfile Path** | `Simulator.API/Dockerfile` |
| **Build Command** | (Để trống - Render tự động build Docker) |
| **Start Command** | (Để trống - Dockerfile đã có ENTRYPOINT) |
| **Environment** | Docker |

---

### 8.2. Frontend Configuration

| Setting | Value |
|---------|-------|
| **Service Type** | Static Site |
| **Repository** | `https://github.com/ThinhTran2412/OJT` |
| **Branch** | `master` |
| **Root Directory** | `OJT_Laboratory_Project/Front_End` |
| **Build Command** | `npm install; npm run build` |
| **Publish Directory** | `dist` |
| **Start Command** | (Để trống - Static Site) |

---

### 8.3. Development Workflow

1. **Setup project:**
   ```batch
   cd Deploy
   setup_project.bat
   ```

2. **Create migrations:**
   ```batch
   create_migrations_dev.bat "MigrationName"
   ```
   Hoặc:
   ```batch
   create_all_migrations.bat "MigrationName"
   ```

3. **Update database:**
   ```batch
   update_databases_dev.bat
   ```
   Hoặc:
   ```batch
   update_all_databases.bat
   ```

4. **Run services locally:**
   - IAM_Service: `https://localhost:7155`
   - Laboratory_Service: `https://localhost:7157`
   - Monitoring_Service: `https://localhost:7159`
   - Simulator_Service: `https://localhost:7158`
   - Frontend: `http://localhost:5173`

---

### 8.4. Production Workflow

1. **Commit and push changes:**
   ```batch
   cd Deploy
   git add .
   git commit -m "Description"
   git push origin master
   ```

2. **Render tự động deploy** (nếu đã cấu hình Auto-Deploy)

3. **Run migrations (nếu cần):**
   ```batch
   update_databases_prod.bat
   ```

---

## 9. Render Deployment Checklist

### 9.1. Before Deploy

- [ ] Database đã được tạo trên Render
- [ ] Tất cả migration files đã được commit và push
- [ ] Dockerfiles đã có trong repository
- [ ] `.dockerignore` files đã có
- [ ] `appsettings.Production.json` đã được cấu hình đúng
- [ ] CORS origins đã được cấu hình trong `appsettings.Production.json`

### 9.2. Backend Services Setup

Cho mỗi service (IAM_Service, Laboratory_Service, Monitoring_Service, Simulator_Service):

- [ ] **Service Type:** Web Service
- [ ] **Environment:** Docker
- [ ] **Repository:** `https://github.com/ThinhTran2412/OJT`
- [ ] **Branch:** `master`
- [ ] **Root Directory:** `OJT_Laboratory_Project/[Service_Name]`
- [ ] **Dockerfile Path:** `[Service_API]/Dockerfile`
- [ ] **Build Command:** (Để trống)
- [ ] **Start Command:** (Để trống)
- [ ] **Auto-Deploy:** Enabled (khuyến nghị)
- [ ] **DATABASE_URL:** (Tự động nếu cùng Project)

### 9.3. Frontend Setup

- [ ] **Service Type:** Static Site
- [ ] **Repository:** `https://github.com/ThinhTran2412/OJT`
- [ ] **Branch:** `master`
- [ ] **Root Directory:** `OJT_Laboratory_Project/Front_End`
- [ ] **Build Command:** `npm install; npm run build`
- [ ] **Publish Directory:** `dist`
- [ ] **Start Command:** (Để trống)
- [ ] **Auto-Deploy:** Enabled (khuyến nghị)
- [ ] **Environment Variables:**
  - [ ] `VITE_API_BASE_URL` = `https://iam-service-fz3h.onrender.com`
  - [ ] `VITE_AUTH_API_URL` = `https://iam-service-fz3h.onrender.com`
  - [ ] `VITE_PATIENT_API_URL` = `https://laboratory-service-url.onrender.com`

### 9.4. After Deploy

- [ ] Kiểm tra services đã start thành công
- [ ] Kiểm tra logs để đảm bảo không có lỗi
- [ ] Chạy migrations: `update_databases_prod.bat`
- [ ] Test API endpoints
- [ ] Test frontend routing
- [ ] Test CORS (từ frontend gọi API)

---

## 10. Useful Links

- **Frontend:** https://front-end-fnfs.onrender.com
- **IAM Service:** https://iam-service-fz3h.onrender.com
- **Database Scripts Guide:** [DATABASE_SCRIPTS_GUIDE.md](./DATABASE_SCRIPTS_GUIDE.md)
- **Project Setup:** [README.md](./README.md)

---

## 📌 Notes

- Tất cả services dùng **chung 1 database** với **schemas riêng**
- Development và Production configurations được tách biệt rõ ràng
- Luôn test trên Development trước khi deploy Production
- Render tự động handle HTTPS termination - services chỉ cần HTTP
- Frontend sử dụng `/api` prefix tự động cho tất cả API calls

---

**Last Updated:** 2025-01-20

