# 🔧 Cấu Hình API cho Frontend React trên Render

## 📋 Tổng Quan

Frontend React **VẪN LÀ Static Site** sau khi build. JSX được compile thành JavaScript, và axios chạy ở client-side (trong browser) để gọi API đến backend services.

## 🔍 Cách React App Hoạt Động

### 1. Build Process

```
JSX Files → npm run build → Static Files (HTML, CSS, JS)
                                ↓
                          Folder dist/
                                ↓
                          Deploy lên Render Static Site
```

- **JSX** được compile thành **JavaScript** khi build
- **Output:** Static files (HTML, CSS, JS) - không cần server runtime
- **Axios** được bundle vào JavaScript, chạy trong browser

### 2. API Calls Flow

```
Browser (Frontend)
    ↓ axios request
Environment Variable (VITE_API_BASE_URL)
    ↓ HTTP request
Backend Service trên Render (IAM_Service, Laboratory_Service, etc.)
    ↓ Response
Browser (Frontend)
```

## ⚙️ Cấu Hình Environment Variables

### Trên Render Static Site Settings

Vào Frontend Static Site → **"Environment"** tab → Add:

#### 1. IAM_Service API (Chính)

**Key:** `VITE_API_BASE_URL`  
**Value:** `https://iam-service.onrender.com`

#### 2. Laboratory_Service API

**Key:** `VITE_PATIENT_API_URL`  
**Value:** `https://laboratory-service.onrender.com`

#### 3. IAM_Service API (Nếu cần override)

**Key:** `VITE_AUTH_API_URL`  
**Value:** `https://iam-service.onrender.com`

### Environment Variables Cần Set

```
VITE_API_BASE_URL=https://iam-service.onrender.com
VITE_PATIENT_API_URL=https://laboratory-service.onrender.com
VITE_AUTH_API_URL=https://iam-service.onrender.com
```

**⚠️ Lưu ý:**
- Phải có prefix `VITE_` để Vite expose cho frontend
- Sau khi set, cần rebuild static site để áp dụng

## 📝 Code Configuration

### File `src/services/api.js`

Đã được cấu hình để dùng environment variables:

```javascript
const api = axios.create({
  baseURL: import.meta.env.PROD
    ? import.meta.env.VITE_API_BASE_URL  // Production: dùng env var
    : "/api",  // Development: dùng proxy
  // ...
});
```

### File `vite.config.js`

Proxy chỉ hoạt động trong development mode:

```javascript
server: {
  proxy: {
    "/api": {
      target: env.VITE_AUTH_API_URL || "https://localhost:7155",
      // ...
    }
  }
}
```

**Lưu ý:**
- Proxy **KHÔNG** hoạt động trên production (static site)
- Chỉ dùng khi chạy `npm run dev` local
- Trên production, axios gọi trực tiếp đến backend URL

## 🚀 Deployment Flow

### 1. Development (Local)

```bash
npm run dev
```

- Vite dev server chạy với proxy
- Request `/api/*` → Proxy đến `localhost:7155` hoặc `localhost:7157`
- JSX được compile on-the-fly

### 2. Production (Render)

```bash
npm run build
```

1. **Build:**
   - Compile JSX → JavaScript
   - Bundle dependencies (react, axios, etc.)
   - Output: Static files trong `dist/`

2. **Deploy:**
   - Render serve files trong `dist/`
   - Browser download và chạy JavaScript

3. **Runtime:**
   - React Router xử lý routing (client-side)
   - Axios gọi API đến backend từ `VITE_API_BASE_URL`
   - Tất cả chạy trong browser

## ✅ Checklist

### Environment Variables trên Render

- [ ] `VITE_API_BASE_URL` - URL của IAM_Service
- [ ] `VITE_PATIENT_API_URL` - URL của Laboratory_Service  
- [ ] `VITE_AUTH_API_URL` - URL của IAM_Service (nếu cần)

### Kiểm tra API Calls

1. **Mở Browser DevTools** (F12)
2. **Tab Network**
3. **Test login hoặc API call**
4. **Kiểm tra:**
   - Request URL có đúng `https://iam-service.onrender.com` không?
   - Response status code?
   - CORS errors?

### Common Issues

#### Lỗi: "Network Error" hoặc "CORS Error"

**Nguyên nhân:** Backend không cho phép CORS từ frontend domain

**Giải pháp:**
- Kiểm tra CORS configuration trên backend
- Đảm bảo backend cho phép origin của frontend

#### Lỗi: "404 Not Found" khi gọi API

**Nguyên nhân:** API URL không đúng

**Giải pháp:**
- Kiểm tra `VITE_API_BASE_URL` đã set chưa
- Kiểm tra backend service đã deploy và running chưa
- Kiểm tra URL trong browser DevTools Network tab

#### Lỗi: Environment variable không hoạt động

**Nguyên nhân:** 
- Thiếu prefix `VITE_`
- Chưa rebuild sau khi set env var

**Giải pháp:**
- Đảm bảo env var có prefix `VITE_`
- Trigger rebuild trên Render
- Clear browser cache

---

## 📞 Troubleshooting

### Kiểm tra Environment Variables

1. Vào Static Site → **"Environment"** tab
2. Xác nhận các biến đã được set
3. Trigger rebuild nếu vừa thêm/sửa

### Kiểm tra API Calls trong Browser

1. Mở Browser DevTools (F12)
2. Tab **Network**
3. Test một API call (ví dụ: login)
4. Xem request URL và response

### Kiểm tra Build Output

1. Vào Static Site → **"Logs"** tab
2. Xem build logs
3. Đảm bảo build thành công
4. Kiểm tra files trong `dist/` folder

