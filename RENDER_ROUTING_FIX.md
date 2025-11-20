# 🔧 Fix "Not Found" Error - React Router trên Render

## 🐛 Vấn đề

Khi truy cập trực tiếp vào các routes như `/login`, `/dashboard`, hoặc refresh trang, bị lỗi **"Not Found"**.

**Nguyên nhân:**
- React Router sử dụng **client-side routing** (Browser History API)
- Khi truy cập trực tiếp `/login`, server (Render) tìm file `/login/index.html` → không tồn tại → 404
- Khi refresh trang, server không biết route `/login` → 404

## ✅ Giải pháp

### Cách 1: File `_redirects` (Khuyến nghị - Tự động)

File `public/_redirects` sẽ tự động được Vite copy vào `dist/` khi build.

**Bước 1: Kiểm tra file `_redirects`**

File `OJT_Laboratory_Project/Front_End/public/_redirects`:
```
/*    /index.html   200
```

**Bước 2: Đảm bảo file được build**

Vite tự động copy files từ `public/` vào root của `dist/` khi build:
```bash
npm run build
```

Sau khi build, kiểm tra folder `dist/`:
```
dist/
  ├── index.html
  ├── _redirects    ← File này phải có
  ├── assets/
  └── ...
```

**Bước 3: Push lên Git và deploy**

```bash
git add public/_redirects
git commit -m "Add _redirects file for React Router"
git push
```

Render sẽ tự động rebuild và nhận file `_redirects`.

### Cách 2: Cấu hình trong Render Dashboard (Nếu Cách 1 không hoạt động)

**Bước 1: Vào Render Dashboard**

1. Đăng nhập Render: https://dashboard.render.com
2. Chọn project chứa Frontend Static Site
3. Click vào **Static Site** (Frontend service)

**Bước 2: Settings → Redirects/Rewrites**

1. Click tab **"Settings"** (hoặc **"Redirects/Rewrites"**)
2. Scroll xuống phần **"Redirects/Rewrites"**
3. Click **"Add Redirect"** hoặc **"Add Rewrite"**

**Bước 3: Cấu hình Rewrite Rule**

- **Source (Path):** `/*`
- **Destination:** `/index.html`
- **Action:** `Rewrite` (hoặc `Redirect` với status code `200`)

**Ví dụ cấu hình:**
```
Source: /*
Destination: /index.html
Action: Rewrite
```

**Hoặc:**
```
Source: /*
Destination: /index.html
Type: Rewrite
Status: 200
```

**Bước 4: Save và Redeploy**

1. Click **"Save Changes"**
2. Render sẽ tự động rebuild và redeploy
3. Chờ deployment hoàn tất

### Cách 3: Sử dụng Hash Router (Không khuyến nghị)

Nếu cả 2 cách trên không hoạt động, có thể đổi sang Hash Router:

**File `src/routes/App_Route.jsx`:**
```javascript
// Thay đổi từ:
import { createBrowserRouter } from "react-router-dom";

// Thành:
import { createHashRouter } from "react-router-dom";

// Và đổi:
const router = createBrowserRouter([...]);

// Thành:
const router = createHashRouter([...]);
```

**Lưu ý:** Hash Router sẽ thay đổi URLs thành `#/login`, `#/dashboard` (không đẹp).

## ✅ Kiểm tra sau khi fix

### 1. Test Direct Access

Mở trình duyệt, gõ trực tiếp URL:
- ✅ `https://front-end-fnfs.onrender.com/` → Home
- ✅ `https://front-end-fnfs.onrender.com/login` → Login page
- ✅ `https://front-end-fnfs.onrender.com/dashboard` → Dashboard

### 2. Test Refresh

1. Vào bất kỳ route nào (ví dụ: `/login`)
2. Refresh trang (F5 hoặc Ctrl+R)
3. ✅ Không được báo "Not Found"

### 3. Test Navigation

1. Vào trang Home
2. Click link đến `/login`
3. ✅ Trang chuyển đúng
4. Refresh trang
5. ✅ Vẫn ở trang Login

### 4. Test Browser DevTools

1. Mở Browser DevTools (F12)
2. Tab **Network**
3. Vào route `/login`
4. Kiểm tra request:
   - ✅ Request đến `/login` → Response `200` (từ `index.html`)
   - ❌ Không được `404 Not Found`

## 🔍 Troubleshooting

### Vấn đề: File `_redirects` không hoạt động

**Kiểm tra:**
1. File `_redirects` có trong `public/` folder?
2. File có được push lên Git?
3. Sau khi build, file có trong `dist/` folder?
4. Render logs có báo lỗi gì không?

**Giải pháp:**
- Sử dụng **Cách 2** (cấu hình trong Render Dashboard)
- Kiểm tra format file `_redirects` (không có dòng trống thừa)

### Vấn đề: Redirects/Rewrites không hoạt động

**Kiểm tra:**
1. Cấu hình trong Render Dashboard đúng chưa?
2. Source path là `/*`?
3. Destination là `/index.html`?
4. Action là `Rewrite`?

**Giải pháp:**
- Thử đổi Action thành `Redirect` với status code `200`
- Clear browser cache
- Hard refresh (Ctrl+Shift+R)

### Vấn đề: Vẫn báo "Not Found"

**Kiểm tra:**
1. Build có thành công không?
2. Deploy có thành công không?
3. Browser DevTools Network tab có lỗi gì không?

**Giải pháp:**
- Kiểm tra Render logs
- Thử clear browser cache và hard refresh
- Kiểm tra CORS settings trên backend (nếu có lỗi network)

## 📝 Lưu ý

1. **File `_redirects`:** Render tự động nhận file này (nếu có trong `dist/`)
2. **Redirects/Rewrites trong Dashboard:** Có thể override file `_redirects`
3. **Browser History:** `createBrowserRouter` cần server support cho client-side routing
4. **Hash Router:** Fallback solution nếu server không support rewrite rules

## 🔗 Tài liệu tham khảo

- [Render Docs: Deploy Create React App](https://render.com/docs/deploy-create-react-app#using-client-side-routing)
- [React Router: Browser Router](https://reactrouter.com/en/main/routers/create-browser-router)
- [Vite: Public Directory](https://vitejs.dev/guide/assets.html#the-public-directory)

