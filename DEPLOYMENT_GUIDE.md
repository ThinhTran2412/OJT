# 🚀 Hướng Dẫn Deploy OJT_Laboratory_Project lên Render

## 📋 Mục Lục

1. [Kết Nối Database](#1-kết-nối-database)
2. [Deploy Backend Services](#2-deploy-backend-services)
3. [Deploy Frontend React](#3-deploy-frontend-react)
4. [Kiểm Tra và Test](#4-kiểm-tra-và-test)

---

## 1. Kết Nối Database

### 1.1. Database Information (Đã có sẵn)

**Internal Database URL:**
```
postgresql://laboratory_service_user:geeqHh8B6xA8oQNkNHw0K0AoJKSZhji2@dpg-d4fcsm95pdvs73ader70-a/laboratory_service
```

**External Database URL:**
```
postgresql://laboratory_service_user:geeqHh8B6xA8oQNkNHw0K0AoJKSZhji2@dpg-d4fcsm95pdvs73ader70-a.singapore-postgres.render.com/laboratory_service
```

### 1.2. Cấu hình Database trên Render

#### Cách 1: Automatic (Khuyến nghị)

**Nếu tất cả services và database trong cùng Render Project:**

1. Vào Database → **"Settings"** → **"Project"**
2. Add database vào cùng Project với các services
3. Render sẽ tự động set `DATABASE_URL` environment variable cho tất cả services
4. Không cần cấu hình thủ công!

#### Cách 2: Manual (Nếu services và database khác Project)

Vào mỗi service → **"Environment"** tab → Add:

- **Key:** `DATABASE_URL`
- **Value:** Internal Database URL (nếu trong cùng region) hoặc External Database URL

### 1.3. Cấu hình Schema cho từng Service

Mỗi service đã được cấu hình schema trong `appsettings.json`:

- **IAM_Service:** `iam_service`
- **Laboratory_Service:** `laboratory_service`
- **Monitoring_Service:** `monitoring_service`
- **Simulator_Service:** `simulator_service`

**Không cần thêm environment variable cho schema** - đã có trong `appsettings.json`.

### 1.4. Chạy Migrations

Sau khi services deploy xong, cần chạy migrations:

#### Option 1: Tự động trong Build Command (Đã có)

Services đã được cấu hình để tự động chạy migrations trong Build Command.

#### Option 2: Chạy thủ công qua Render Shell

1. Vào service → **"Shell"** tab
2. Chạy:
```bash
cd /opt/render/project/src
dotnet tool install --global dotnet-ef
dotnet ef database update --project [Infrastructure]/[Infrastructure].csproj --startup-project [API]/[API].csproj
```

**Ví dụ cho IAM_Service:**
```bash
dotnet ef database update --project IAM_Service.Infrastructure/IAM_Service.Infrastructure.csproj --startup-project IAM_Service.API/IAM_Service.API.csproj
```

---

## 2. Deploy Backend Services

### 2.1. Repository Information

- **Repository:** `https://github.com/ThinhTran2412/OJT`
- **Branch:** `master`
- **Type:** Monorepo (tất cả services trong 1 repo)

### 2.2. Cấu hình cho từng Service

#### IAM_Service

**Settings:**
- **Name:** `iam-service`
- **Region:** Singapore (Asia Pacific)
- **Branch:** `master`
- **Root Directory:** `OJT_Laboratory_Project/IAM_Service`
- **Runtime:** `dotnet`
- **Build Command:**
  ```bash
  dotnet tool install --global dotnet-ef || true && dotnet restore IAM_Service.sln && dotnet build IAM_Service.sln -c Release && dotnet ef database update --project IAM_Service.Infrastructure/IAM_Service.Infrastructure.csproj --startup-project IAM_Service.API/IAM_Service.API.csproj && dotnet publish IAM_Service.API/IAM_Service.API.csproj -c Release -o ./publish
  ```
- **Start Command:**
  ```bash
  cd publish && dotnet IAM_Service.API.dll
  ```
- **Dockerfile Path:** `IAM_Service.API/Dockerfile` (hoặc để trống nếu dùng Build Command)

**Environment Variables:**
- `DATABASE_URL` - Tự động set nếu database và service trong cùng Project
- `ASPNETCORE_ENVIRONMENT=Production` - Tự động set bởi Render

#### Laboratory_Service

**Settings:**
- **Name:** `laboratory-service`
- **Region:** Singapore (Asia Pacific)
- **Branch:** `master`
- **Root Directory:** `OJT_Laboratory_Project/Laboratory_Service`
- **Runtime:** `dotnet`
- **Build Command:**
  ```bash
  dotnet tool install --global dotnet-ef || true && dotnet restore Laboratory_Service.sln && dotnet build Laboratory_Service.sln -c Release && dotnet ef database update --project Laboratory_Service.Infrastructure/Laboratory_Service.Infrastructure.csproj --startup-project Laboratory_Service.API/Laboratory_Service.API.csproj && dotnet publish Laboratory_Service.API/Laboratory_Service.API.csproj -c Release -o ./publish
  ```
- **Start Command:**
  ```bash
  cd publish && dotnet Laboratory_Service.API.dll
  ```
- **Dockerfile Path:** `Laboratory_Service.API/Dockerfile`

#### Monitoring_Service

**Settings:**
- **Name:** `monitoring-service`
- **Region:** Singapore (Asia Pacific)
- **Branch:** `master`
- **Root Directory:** `OJT_Laboratory_Project/Monitoring_Service`
- **Runtime:** `dotnet`
- **Build Command:**
  ```bash
  dotnet tool install --global dotnet-ef || true && dotnet restore Monitoring_Service.slnx && dotnet build Monitoring_Service.slnx -c Release && dotnet ef database update --project Monitoring_Service.Infastructure/Monitoring_Service.Infastructure.csproj --startup-project Monitoring_Service.API/Monitoring_Service.API.csproj && dotnet publish Monitoring_Service.API/Monitoring_Service.API.csproj -c Release -o ./publish
  ```
- **Start Command:**
  ```bash
  cd publish && dotnet Monitoring_Service.API.dll
  ```
- **Dockerfile Path:** `Monitoring_Service.API/Dockerfile`

#### Simulator_Service

**Settings:**
- **Name:** `simulator-service`
- **Region:** Singapore (Asia Pacific)
- **Branch:** `master`
- **Root Directory:** `OJT_Laboratory_Project/Simulator_Service`
- **Runtime:** `dotnet`
- **Build Command:**
  ```bash
  dotnet tool install --global dotnet-ef || true && dotnet restore Simulator_Service.sln && dotnet build Simulator_Service.sln -c Release && dotnet ef database update --project Simulator.Infastructure/Simulator.Infastructure.csproj --startup-project Simulator.API/Simulator.API.csproj && dotnet publish Simulator.API/Simulator.API.csproj -c Release -o ./publish
  ```
- **Start Command:**
  ```bash
  cd publish && dotnet Simulator.API.dll
  ```
- **Dockerfile Path:** `Simulator.API/Dockerfile`

### 2.3. Lưu ý Quan Trọng

1. **Root Directory:** Phải set đúng vì đây là monorepo
2. **Dockerfile Path:** Đường dẫn tương đối từ Root Directory
3. **DATABASE_URL:** Render tự động set nếu database và service trong cùng Project
4. **HTTPS:** Đã được tắt trong Production - Render handle HTTPS ở load balancer

---

## 3. Deploy Frontend React

### 3.1. Tạo Static Site trên Render

1. Vào Render Dashboard → Click **"New +"** → Chọn **"Static Site"** ⭐ **QUAN TRỌNG!**
   - **KHÔNG** chọn "Web Service"
   - Phải chọn **"Static Site"**

2. Connect repository: `https://github.com/ThinhTran2412/OJT`

3. Cấu hình:

**Settings:**
- **Name:** `ojt-frontend` hoặc `laboratory-frontend`
- **Branch:** `master`
- **Root Directory:** `OJT_Laboratory_Project/Front_End` ⭐ **QUAN TRỌNG!**
- **Build Command:**
  ```bash
  npm install && npm run build
  ```
- **Publish Directory:** `dist` ⭐ **QUAN TRỌNG!**
- **Start Command:** ⚠️ **KHÔNG SET - ĐỂ TRỐNG!** (Static Site không cần start command)

**⚠️ Lưu ý quan trọng:**
- **KHÔNG** tạo Web Service, phải tạo **Static Site**
- **KHÔNG** set Start Command (để trống hoàn toàn)
- Static Site chỉ cần Build Command và Publish Directory
- Render sẽ tự động serve files trong Publish Directory sau khi build
- Nếu thấy lỗi "Missing script: start", có nghĩa là đang cấu hình sai (chọn Web Service thay vì Static Site)

**❓ FAQ: "File `index.html` chỉ có `<div id="root"></div>` rỗng, có cần Web Service không?"**

**Trả lời: KHÔNG! Đây là cách React SPA hoạt động bình thường.**

- File `index.html` chỉ là **entry point** - không cần server-side rendering
- React sẽ **render toàn bộ UI** vào `<div id="root">` bằng JavaScript (chạy trong browser)
- JavaScript files trong `assets/` chứa toàn bộ code React (đã được compile)
- **Tất cả logic chạy client-side** → Vẫn là Static Site, không cần Web Service

Xem file `REACT_STATIC_SITE_EXPLAINED.md` để hiểu chi tiết cách React SPA hoạt động.

**🔧 Fix "Not Found" Error (React Router):**

File `public/_redirects` đã được tạo để redirect tất cả routes về `index.html`:
```
/*    /index.html   200
```

**⚠️ QUAN TRỌNG:** Nếu vẫn gặp lỗi "Not Found" sau khi có file `_redirects`, cần cấu hình **Redirects/Rewrites** trong Render Dashboard:

### Cách 1: File `_redirects` (Tự động - đã có)

File `public/_redirects` sẽ tự động được Vite copy vào `dist/` khi build. Render sẽ tự động nhận file này.

**Kiểm tra:**
1. File `_redirects` có trong `public/` folder ✅
2. Sau khi build, file có trong `dist/` folder
3. File đã được push lên Git

### Cách 2: Cấu hình trong Render Dashboard (Nếu Cách 1 không hoạt động)

Nếu file `_redirects` không hoạt động, cấu hình thủ công trong Render:

1. **Vào Render Dashboard:**
   - Đăng nhập Render
   - Chọn Static Site (Frontend service)

2. **Settings → Redirects/Rewrites:**
   - Click **"Add Redirect"** hoặc **"Add Rewrite"**
   - **Source:** `/*`
   - **Destination:** `/index.html`
   - **Action:** `Rewrite` (hoặc `Redirect` với status code `200`)

3. **Save và Redeploy:**
   - Click **"Save Changes"**
   - Render sẽ tự động rebuild và redeploy

### Kiểm tra sau khi fix:

1. **Test routes:**
   - `https://front-end-fnfs.onrender.com/` → Home ✅
   - `https://front-end-fnfs.onrender.com/login` → Login ✅
   - `https://front-end-fnfs.onrender.com/dashboard` → Dashboard ✅

2. **Test refresh:**
   - Vào bất kỳ route nào (ví dụ: `/login`)
   - Refresh trang (F5)
   - Không được báo "Not Found" ✅

3. **Test direct access:**
   - Mở tab mới
   - Gõ trực tiếp URL: `https://front-end-fnfs.onrender.com/login`
   - Trang load đúng ✅

**Environment Variables:** ⭐ **QUAN TRỌNG!**

Vì frontend dùng axios để gọi API, cần set các environment variables:

- **`VITE_API_BASE_URL`** - URL của IAM_Service API (chính)
  - Ví dụ: `https://iam-service.onrender.com`
  - Dùng cho: Auth, User, Role, EventLog, PatientInfo
  
- **`VITE_AUTH_API_URL`** - URL của IAM_Service (nếu cần override)
  - Ví dụ: `https://iam-service.onrender.com`
  
- **`VITE_PATIENT_API_URL`** - URL của Laboratory_Service
  - Ví dụ: `https://laboratory-service.onrender.com`
  - Dùng cho: Patient, TestOrder, TestResult, AI Review, MedicalRecord

**Lưu ý:**
- Proxy trong `vite.config.js` chỉ hoạt động khi chạy `npm run dev` (development)
- Khi deploy production, axios sẽ gọi trực tiếp đến backend URL từ environment variables
- Không cần proxy trên production

### 3.2. Cấu hình Environment Variables

Tạo file `.env.production` trong Front_End (nếu cần) hoặc set trong Render:

**Ví dụ:**
```
VITE_API_BASE_URL=https://iam-service.onrender.com
VITE_LABORATORY_API_URL=https://laboratory-service.onrender.com
```

**Lưu ý:** Vite chỉ expose các biến bắt đầu bằng `VITE_` trong frontend.

### 3.3. Kiểm tra API Configuration

File `src/services/api.js` đã được cấu hình để dùng environment variables:

```javascript
const api = axios.create({
  baseURL: import.meta.env.PROD
    ? import.meta.env.VITE_API_BASE_URL  // Production: dùng env var
    : "/api",  // Development: dùng proxy từ vite.config.js
  // ...
});
```

**Cách hoạt động:**
- **Development (`npm run dev`):** Dùng proxy từ `vite.config.js` → `/api` → proxy đến localhost
- **Production (deploy):** Dùng trực tiếp `VITE_API_BASE_URL` → gọi đến backend trên Render

**⚠️ Quan trọng:**
- Environment variables phải bắt đầu bằng `VITE_` để Vite expose cho frontend
- Sau khi set env vars trên Render, cần rebuild để áp dụng

### 3.4. Build và Deploy

1. Render sẽ tự động:
   - Run `npm install`
   - Run `npm run build`
   - Deploy files trong `dist` folder
2. Kiểm tra build logs để đảm bảo build thành công
3. Kiểm tra URL của static site

**Lưu ý về Chunk Size Warning:**
- Warning về chunk size lớn (>500KB) là bình thường
- Không phải lỗi - site vẫn hoạt động tốt
- Đã được optimize với manual chunks trong `vite.config.js`
- Nếu muốn optimize thêm, có thể dùng code splitting với React.lazy

**📝 Giải thích về React Static Site:**

React app **VẪN LÀ Static Site** sau khi build:
1. **JSX được compile:** Khi chạy `npm run build`, Vite compile JSX thành JavaScript
2. **Output là static files:** Tạo ra HTML, CSS, JS trong folder `dist/`
3. **Axios chạy client-side:** Axios là JavaScript library chạy trong browser, gọi API đến backend
4. **Không cần server runtime:** Static Site chỉ serve files, không cần Node.js server

**Development vs Production:**
- **Dev (`npm run dev`):** Proxy trong `vite.config.js` hoạt động để redirect `/api` → localhost
- **Production (deploy):** Axios gọi trực tiếp đến backend URL từ `VITE_API_BASE_URL` environment variable

### 3.5. Custom Domain (Tùy chọn)

1. Vào Static Site → **"Settings"** → **"Custom Domain"**
2. Thêm custom domain nếu cần
3. Follow DNS instructions

---

## 4. Kiểm Tra và Test

### 4.1. Kiểm tra Backend Services

1. **Health Check:**
   - IAM_Service: `https://iam-service.onrender.com/swagger`
   - Laboratory_Service: `https://laboratory-service.onrender.com/swagger`
   - Monitoring_Service: `https://monitoring-service.onrender.com/swagger`
   - Simulator_Service: `https://simulator-service.onrender.com/swagger`

2. **Test API:**
   - Dùng Swagger UI để test endpoints
   - Hoặc dùng Postman/Thunder Client

### 4.2. Kiểm tra Database Connection

1. Vào service → **"Logs"** tab
2. Kiểm tra logs để xem:
   - Database connection successful
   - Migrations đã chạy
   - Không có lỗi

### 4.3. Kiểm tra Frontend

1. Truy cập URL của static site
2. Test các chức năng:
   - Login/Register
   - API calls
   - Navigation

### 4.4. Test Integration

1. Test frontend gọi API từ backend
2. Test authentication flow
3. Test các chức năng chính

---

## 📝 Quick Reference

### Database Connection String

**Internal (Cho services trong cùng Render Project):**
```
postgresql://laboratory_service_user:geeqHh8B6xA8oQNkNHw0K0AoJKSZhji2@dpg-d4fcsm95pdvs73ader70-a/laboratory_service
```

**External (Cho kết nối từ bên ngoài):**
```
postgresql://laboratory_service_user:geeqHh8B6xA8oQNkNHw0K0AoJKSZhji2@dpg-d4fcsm95pdvs73ader70-a.singapore-postgres.render.com/laboratory_service
```

### Service URLs (Sau khi deploy)

- IAM_Service: `https://iam-service.onrender.com`
- Laboratory_Service: `https://laboratory-service.onrender.com`
- Monitoring_Service: `https://monitoring-service.onrender.com`
- Simulator_Service: `https://simulator-service.onrender.com`
- Frontend: `https://ojt-frontend.onrender.com`

### Environment Variables cần thiết

**Backend Services:**
- `DATABASE_URL` - Tự động set nếu trong cùng Project
- `ASPNETCORE_ENVIRONMENT=Production` - Tự động set

**Frontend:**
- `VITE_API_BASE_URL` - URL của backend API
- `VITE_LABORATORY_API_URL` - URL của Laboratory Service (nếu cần)

---

## 🔧 Troubleshooting

### Database Connection Failed

**Giải pháp:**
- Kiểm tra `DATABASE_URL` đã được set chưa
- Kiểm tra database và service trong cùng region
- Kiểm tra database đã ready chưa
- Kiểm tra Internal URL vs External URL

### Service không start

**Giải pháp:**
- Kiểm tra PORT environment variable
- Kiểm tra build logs để xem lỗi
- Kiểm tra HTTPS đã được tắt chưa

### Frontend build failed

**Giải pháp:**
- Kiểm tra Node.js version
- Kiểm tra dependencies
- Kiểm tra build command
- Kiểm tra Publish Directory

### API calls failed from frontend

**Giải pháp:**
- Kiểm tra CORS configuration
- Kiểm tra API URL trong environment variables
- Kiểm tra backend services đã running chưa

---

## 📞 Useful Links

- [Render Documentation](https://render.com/docs)
- [PostgreSQL on Render](https://render.com/docs/databases)
- [Static Sites on Render](https://render.com/docs/static-sites)
- [Environment Variables](https://render.com/docs/environment-variables)

