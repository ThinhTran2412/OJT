# Hướng dẫn cấu hình Frontend trên Render

## URL Frontend
- **Production URLs**: 
  - `https://ojt-front-end.onrender.com` (main)
  - `https://ojt-invc.onrender.com` (alternative)

## Cấu hình Render Service Settings

### Bước 0: Cấu hình Build và Start Commands (QUAN TRỌNG!)
1. Vào Render Dashboard → Chọn Frontend service (`ojt-invc`)
2. Vào tab **"Settings"**
3. Cấu hình các settings sau:

   **Build Command**: `npm install && npm run build`
   
   **Start Command**: `npm start` ⚠️ **PHẢI là `npm start` (KHÔNG phải `npm run dev`)**
   
   **Root Directory**: `Multiple_Service/Front_end` (nếu repo root là Deploy)
   
   ⚠️ **Lưu ý QUAN TRỌNG**: 
   - **Start Command PHẢI là `npm start`** - Render đang chạy `npm run dev` (sai!)
   - Nếu repo root là `Multiple_Service`, thì Root Directory là `Front_end`
   - Nếu repo root là `Deploy`, thì Root Directory là `Multiple_Service/Front_end`
   - `npm start` sẽ chạy `serve` để serve static files từ `dist` folder (production build)
   - `npm run dev` là development server, không dùng cho production!

## Cấu hình Environment Variables trên Render

### Bước 1: Xác định URL của IAM Service
1. Vào Render Dashboard
2. Tìm IAM Service (service name có thể là `iam-service` hoặc tương tự)
3. Copy URL của service (ví dụ: `https://iam-service-xxx.onrender.com`)

### Bước 2: Thêm Environment Variable cho Frontend
1. Vào Render Dashboard → Chọn Frontend service (`ojt-invc`)
2. Vào tab **"Environment"**
3. Click **"Add Environment Variable"**
4. Thêm biến sau:

   **Key**: `VITE_API_BASE_URL`
   
   **Value**: `https://your-iam-service-url.onrender.com/api`
   
   ⚠️ **Lưu ý**: 
   - Thay `your-iam-service-url.onrender.com` bằng URL thực tế của IAM Service
   - Phải có `/api` ở cuối URL

### Bước 3: Redeploy Frontend
- Render sẽ tự động rebuild khi có thay đổi environment variable
- Hoặc click "Manual Deploy" → "Deploy latest commit"

## Kiểm tra sau khi deploy

1. Mở `https://ojt-invc.onrender.com`
2. Mở Developer Console (F12)
3. Kiểm tra:
   - **Console tab**: Không có warning "VITE_API_BASE_URL not set!"
   - **Network tab**: API calls đang gọi đúng URL (không phải `/api` relative)

## Ví dụ cấu hình

Nếu IAM Service URL là `https://iam-service-abc123.onrender.com`, thì:

```
VITE_API_BASE_URL=https://iam-service-abc123.onrender.com/api
```

## Troubleshooting

### Lỗi: "An HTTP/1.x request was sent to an HTTP/2 only endpoint"
- **Nguyên nhân**: Frontend đang gọi sai URL hoặc thiếu biến môi trường
- **Giải pháp**: Đảm bảo `VITE_API_BASE_URL` đã được set đúng trên Render

### Lỗi: CORS error
- **Nguyên nhân**: IAM Service chưa cho phép origin của frontend
- **Giải pháp**: Đã được cấu hình trong `Program.cs` với `https://ojt-invc.onrender.com`

### API calls trả về 404
- **Nguyên nhân**: URL không đúng hoặc thiếu `/api`
- **Giải pháp**: Kiểm tra `VITE_API_BASE_URL` có đúng format: `https://service-url.onrender.com/api`

### Lỗi: "Running 'npm run dev'" hoặc "Out of memory"
- **Nguyên nhân**: Start Command trên Render đang là `npm run dev` thay vì `npm start`
- **Giải pháp**: 
  1. Vào Render Dashboard → Frontend service → Settings
  2. Đổi **Start Command** từ `npm run dev` thành `npm start`
  3. Save và redeploy
  4. `npm start` sẽ serve static files (nhẹ hơn), `npm run dev` là dev server (nặng, không dùng cho production)

### Lỗi: "No open ports detected"
- **Nguyên nhân**: Port không được bind đúng
- **Giải pháp**: Đảm bảo Start Command là `npm start` (đã cấu hình dùng `$PORT` từ Render)

