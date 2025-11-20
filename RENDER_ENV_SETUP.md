# ⚙️ Cấu Hình Environment Variables trên Render

## 🔧 IAM_Service Environment Variables

### Vào Render Dashboard → IAM_Service → **"Environment"** tab

#### 1. DATABASE_URL

- **Key:** `DATABASE_URL`
- **Value:** `postgresql://laboratory_service_user:geeqHh8B6xA8oQNkNHw0K0AoJKSZhji2@dpg-d4fcsm95pdvs73ader70-a/laboratory_service`
- **Usage:** Internal connection string (nếu cùng Project)

**Hoặc nếu khác Project/Region:**
- **Value:** `postgresql://laboratory_service_user:geeqHh8B6xA8oQNkNHw0K0AoJKSZhji2@dpg-d4fcsm95pdvs73ader70-a.singapore-postgres.render.com/laboratory_service`

**Lưu ý:** 
- Nếu Database và IAM_Service trong cùng Project → Render sẽ tự động set `DATABASE_URL`
- Nếu không → Cần set thủ công như trên

#### 2. PORT (Tự động)

Render tự động set `PORT` environment variable → Không cần set thủ công.

---

## 🔧 Frontend Static Site Environment Variables

### Vào Render Dashboard → Frontend Static Site → **"Environment"** tab

#### 1. VITE_API_BASE_URL

- **Key:** `VITE_API_BASE_URL`
- **Value:** `https://iam-service-fz3h.onrender.com`
- **Usage:** Base URL cho axios calls (Auth, User, Role, etc.)

#### 2. VITE_AUTH_API_URL

- **Key:** `VITE_AUTH_API_URL`
- **Value:** `https://iam-service-fz3h.onrender.com`
- **Usage:** Override URL cho Auth endpoints (nếu cần)

#### 3. VITE_PATIENT_API_URL

- **Key:** `VITE_PATIENT_API_URL`
- **Value:** `https://laboratory-service.onrender.com` (cập nhật khi deploy Laboratory_Service)
- **Usage:** Base URL cho Laboratory_Service (Patient, TestOrder, etc.)

---

## ✅ Checklist

### IAM_Service

- [ ] `DATABASE_URL` đã được set (hoặc Render tự động set nếu cùng Project)
- [ ] Service đã deploy và running
- [ ] Logs không có database connection errors
- [ ] API endpoints hoạt động: `https://iam-service-fz3h.onrender.com/api/...`

### Frontend Static Site

- [ ] `VITE_API_BASE_URL` = `https://iam-service-fz3h.onrender.com`
- [ ] `VITE_AUTH_API_URL` = `https://iam-service-fz3h.onrender.com` (nếu cần)
- [ ] `VITE_PATIENT_API_URL` = `https://laboratory-service.onrender.com` (khi deploy Laboratory_Service)
- [ ] Frontend đã rebuild sau khi set env vars
- [ ] Frontend có thể call API từ IAM_Service

---

## 🚀 Các Bước Thực Hiện

### 1. Cấu hình IAM_Service Database

1. Vào IAM_Service → **"Environment"** tab
2. Kiểm tra có `DATABASE_URL` chưa?
   - ✅ Có: Skip bước này
   - ❌ Không có: Add `DATABASE_URL` với Internal URL
3. Save và Redeploy (nếu thêm mới)

### 2. Cấu hình Frontend API URLs

1. Vào Frontend Static Site → **"Environment"** tab
2. Add:
   - `VITE_API_BASE_URL` = `https://iam-service-fz3h.onrender.com`
   - `VITE_AUTH_API_URL` = `https://iam-service-fz3h.onrender.com`
3. Save → Render sẽ tự động rebuild

### 3. Test Connection

1. **Test IAM_Service:**
   - Vào `https://iam-service-fz3h.onrender.com/api/Auth/...`
   - Xem có response không? (có thể 404 nhưng không được 500/connection error)

2. **Test Frontend:**
   - Vào `https://front-end-fnfs.onrender.com`
   - Mở Browser DevTools → Network tab
   - Test login → Xem API calls có đến `https://iam-service-fz3h.onrender.com` không?

---

## 🔍 Troubleshooting

### Lỗi: "Network Error" hoặc "CORS Error"

**Nguyên nhân:** Backend không cho phép CORS từ frontend domain

**Giải pháp:**
- Kiểm tra CORS configuration trên IAM_Service
- Đảm bảo IAM_Service cho phép origin: `https://front-end-fnfs.onrender.com`

### Lỗi: "404 Not Found" khi call API

**Nguyên nhân:** API URL không đúng hoặc endpoint không tồn tại

**Giải pháp:**
1. Kiểm tra `VITE_API_BASE_URL` có đúng không?
2. Kiểm tra API endpoint có tồn tại không?
3. Kiểm tra IAM_Service đã deploy và running chưa?

### Lỗi: Environment variable không hoạt động

**Nguyên nhân:** 
- Thiếu prefix `VITE_` cho frontend env vars
- Chưa rebuild sau khi set env vars

**Giải pháp:**
1. Đảm bảo env var có prefix `VITE_` (frontend)
2. Trigger rebuild trên Render
3. Clear browser cache và hard refresh

