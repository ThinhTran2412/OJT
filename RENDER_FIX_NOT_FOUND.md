# 🔧 Fix "Not Found" Error - Cấu Hình Render Dashboard

## 🐛 Vấn Đề

Truy cập trực tiếp `https://front-end-fnfs.onrender.com/login` → **"Not Found"**

File `_redirects` đã có nhưng Render có thể không tự động nhận file này.

## ✅ Giải Pháp: Cấu Hình Redirects/Rewrites Trong Render Dashboard

### Bước 1: Vào Render Dashboard

1. Đăng nhập: https://dashboard.render.com
2. Chọn project chứa Frontend Static Site
3. Click vào **Static Site** (Frontend service)

### Bước 2: Mở Settings → Redirects/Rewrites

1. Click tab **"Settings"** (hoặc tìm phần **"Redirects/Rewrites"**)
2. Scroll xuống phần **"Redirects/Rewrites"** hoặc **"Rewrites"**
3. Click **"Add Redirect"** hoặc **"Add Rewrite"**

### Bước 3: Thêm Rewrite Rule

**Cấu hình như sau:**

```
Source (Path): /*
Destination: /index.html
Action: Rewrite
Status: 200
```

**Hoặc nếu chỉ có các field đơn giản:**

- **From:** `/*`
- **To:** `/index.html`
- **Type:** `Rewrite` (hoặc `Redirect` với status `200`)

### Bước 4: Save và Redeploy

1. Click **"Save Changes"** hoặc **"Update"**
2. Render sẽ tự động rebuild và redeploy
3. Chờ deployment hoàn tất (thường 1-2 phút)

### Bước 5: Test Lại

Sau khi deploy xong, test:

1. **Direct Access:**
   - ✅ `https://front-end-fnfs.onrender.com/login` → Login page
   - ✅ `https://front-end-fnfs.onrender.com/dashboard` → Dashboard
   - ✅ `https://front-end-fnfs.onrender.com/home` → Home

2. **Refresh:**
   - Vào `/login`
   - Refresh (F5)
   - ✅ Không báo "Not Found"

## 📝 Screenshot Hướng Dẫn

### Tìm Redirects/Rewrites trong Settings:

```
Static Site Settings
├── General
│   ├── Name
│   ├── Branch
│   └── Root Directory
├── Build & Deploy
│   ├── Build Command
│   └── Publish Directory
├── Redirects/Rewrites  ← TÌM Ở ĐÂY!
│   └── Add Redirect/Rewrite
└── Environment
```

### Cấu hình Rewrite Rule:

```
┌─────────────────────────────────────┐
│ Add Rewrite                         │
├─────────────────────────────────────┤
│ Source (Path):                      │
│ /*                                  │
├─────────────────────────────────────┤
│ Destination:                        │
│ /index.html                         │
├─────────────────────────────────────┤
│ Action: [Rewrite ▼]                 │
│ Status: [200 ▼]                     │
├─────────────────────────────────────┤
│ [Cancel]  [Save]                    │
└─────────────────────────────────────┘
```

## 🔍 Troubleshooting

### Vấn đề: Không tìm thấy "Redirects/Rewrites" trong Settings

**Giải pháp:**
1. Kiểm tra xem đây có phải Static Site không (không phải Web Service)
2. Một số Render plans có thể không có feature này
3. Thử tìm trong tab khác: "Environment", "Advanced"

### Vấn đề: Vẫn báo "Not Found" sau khi cấu hình

**Kiểm tra:**
1. Cấu hình đã save chưa?
2. Deployment đã hoàn tất chưa? (kiểm tra logs)
3. Clear browser cache và hard refresh (Ctrl+Shift+R)
4. Thử incognito/private window

**Giải pháp:**
1. Xóa rule cũ và tạo lại
2. Đảm bảo Source là `/*` (wildcard)
3. Đảm bảo Destination là `/index.html` (không có trailing slash)

### Vấn đề: Lỗi khi save Rewrite Rule

**Kiểm tra:**
1. Format có đúng không? (Source: `/*`, Destination: `/index.html`)
2. Có conflict với rule khác không?
3. Render logs có báo lỗi gì không?

**Giải pháp:**
1. Xóa tất cả rules và tạo lại từ đầu
2. Chỉ tạo 1 rule duy nhất: `/* → /index.html`

## 🆘 Nếu Vẫn Không Hoạt Động

### Cách 2: Kiểm Tra File `_redirects` Trong Build

1. Vào Render Dashboard → Static Site → **Logs**
2. Xem build logs
3. Tìm dòng "Copying _redirects" hoặc tương tự
4. Đảm bảo file `_redirects` được copy vào `dist/`

### Cách 3: Manual Deploy với File `_redirects`

Nếu Render không tự động nhận `_redirects`, thử:

1. **Kiểm tra file `_redirects` trong dist/:**
   ```bash
   cd OJT_Laboratory_Project/Front_End
   npm run build
   # Kiểm tra dist/_redirects có tồn tại không
   ```

2. **Đảm bảo file có format đúng:**
   ```
   /*    /index.html   200
   ```
   (Không có dòng trống thừa)

3. **Commit và push:**
   ```bash
   git add public/_redirects
   git commit -m "Add _redirects for React Router"
   git push
   ```

### Cách 4: Contact Render Support

Nếu tất cả các cách trên không hoạt động:
1. Vào Render Dashboard → **Help** → **Contact Support**
2. Gửi message: "Static Site redirects not working for React Router SPA"
3. Include screenshot của Rewrite Rule configuration

## ✅ Checklist

### Trước khi test:
- [ ] File `_redirects` có trong `public/` folder
- [ ] File `_redirects` có trong `dist/` sau khi build
- [ ] Rewrite Rule đã được cấu hình trong Render Dashboard
- [ ] Deployment đã hoàn tất (không còn "Building..." hoặc "Deploying...")

### Sau khi test:
- [ ] Direct access `/login` → Login page ✅
- [ ] Direct access `/dashboard` → Dashboard ✅
- [ ] Refresh page không báo "Not Found" ✅
- [ ] Navigation trong app hoạt động bình thường ✅

## 📞 Liên Hệ

Nếu vẫn gặp vấn đề, kiểm tra:
- Render Docs: https://render.com/docs/static-sites#redirects-and-rewrites
- Render Support: https://render.com/support

