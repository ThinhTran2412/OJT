# Hướng dẫn cấu hình Frontend trên Render

## URL Frontend
- **Production URL**: `https://ojt-invc.onrender.com`

## Cấu hình Render Service Settings

### Bước 0: Cấu hình Build và Start Commands
1. Vào Render Dashboard → Chọn Frontend service (`ojt-invc`)
2. Vào tab **"Settings"**
3. Cấu hình các settings sau:

   **Build Command**: `npm install && npm run build`
   
   **Start Command**: `npm start`
   
   **Root Directory**: `Multiple_Service/Front_end` (nếu repo root là Deploy)
   
   ⚠️ **Lưu ý**: 
   - Nếu repo root là `Multiple_Service`, thì Root Directory là `Front_end`
   - Nếu repo root là `Deploy`, thì Root Directory là `Multiple_Service/Front_end`

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

