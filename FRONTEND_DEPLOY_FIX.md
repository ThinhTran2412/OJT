# 🔧 Fix Lỗi: "Missing script: start" khi Deploy Frontend

## ❌ Lỗi

```
npm error Missing script: "start"
```

## 🔍 Nguyên nhân

Render đang chạy `npm start` sau khi build, nhưng **Static Site không cần start command**.

**Có thể do:**
1. Tạo nhầm **Web Service** thay vì **Static Site**
2. Static Site có set Start Command (không nên)

## ✅ Giải pháp

### Bước 1: Kiểm tra loại Service

1. Vào Render Dashboard
2. Vào Frontend service
3. Kiểm tra loại service:
   - ✅ **Static Site** - Đúng
   - ❌ **Web Service** - Sai, cần tạo lại

### Bước 2: Nếu là Web Service (SAI)

**Option 1: Tạo lại Static Site (Khuyến nghị)**

1. Delete Web Service hiện tại
2. Tạo mới:
   - Click **"New +"** → Chọn **"Static Site"** ⭐
   - Connect repository: `https://github.com/ThinhTran2412/OJT`
   - Branch: `master`
   - Root Directory: `OJT_Laboratory_Project/Front_End`
   - Build Command: `npm install && npm run build`
   - Publish Directory: `dist`
   - **Start Command:** *(Để trống - không set gì cả)*

**Option 2: Fix Web Service (Không khuyến nghị)**

Nếu muốn giữ Web Service, cần:
1. Add script "start" vào package.json (không tốt cho static site)
2. Hoặc set Start Command để serve static files

### Bước 3: Nếu là Static Site nhưng có lỗi

1. Vào Settings
2. Kiểm tra **Start Command:**
   - ❌ Nếu có gì đó → Xóa hết, để trống
   - ✅ Phải để trống hoàn toàn

3. Kiểm tra **Build Command:**
   ```bash
   npm install && npm run build
   ```

4. Kiểm tra **Publish Directory:**
   ```
   dist
   ```

### Bước 4: Re-deploy

1. Vào service → **"Manual Deploy"**
2. Chọn commit mới nhất
3. Click **"Deploy latest commit"**

---

## 📋 Checklist Đúng

### Static Site Configuration

- [ ] Type: **Static Site** (không phải Web Service)
- [ ] Repository: `https://github.com/ThinhTran2412/OJT`
- [ ] Branch: `master`
- [ ] Root Directory: `OJT_Laboratory_Project/Front_End`
- [ ] Build Command: `npm install && npm run build`
- [ ] Publish Directory: `dist`
- [ ] **Start Command:** *(Trống - không có gì)* ⭐
- [ ] Environment Variables: `VITE_API_BASE_URL=https://iam-service.onrender.com`

---

## 🎯 Cấu hình Đúng

```
Render Dashboard
  └── New + → Static Site (không phải Web Service)
       ├── Name: ojt-frontend
       ├── Repository: https://github.com/ThinhTran2412/OJT
       ├── Branch: master
       ├── Root Directory: OJT_Laboratory_Project/Front_End
       ├── Build Command: npm install && npm run build
       ├── Publish Directory: dist
       └── Start Command: (TRỐNG - không set gì)
```

---

## ✅ Sau khi fix

1. Build sẽ chạy: `npm install && npm run build`
2. Build thành công → Tạo folder `dist`
3. Render serve files trong `dist` folder
4. Không chạy `npm start` (vì là Static Site)

---

## 📝 Lưu ý

**Static Site vs Web Service:**

| | Static Site | Web Service |
|---|---|---|
| **Type** | Static Site | Web Service |
| **Build Command** | ✅ Cần | ✅ Cần |
| **Publish Directory** | ✅ Cần | ❌ Không |
| **Start Command** | ❌ Không cần | ✅ Cần |
| **Dùng cho** | React, Vue, HTML | Node.js, .NET, etc. |

**Frontend React = Static Site** (không phải Web Service)

