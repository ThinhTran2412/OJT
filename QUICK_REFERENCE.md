# ⚡ Quick Reference - Deploy OJT_Laboratory_Project

## 🗄️ Database Connection

### Internal URL (Cho services trong Render)
```
postgresql://laboratory_service_user:geeqHh8B6xA8oQNkNHw0K0AoJKSZhji2@dpg-d4fcsm95pdvs73ader70-a/laboratory_service
```

### External URL (Cho local dev)
```
postgresql://laboratory_service_user:geeqHh8B6xA8oQNkNHw0K0AoJKSZhji2@dpg-d4fcsm95pdvs73ader70-a.singapore-postgres.render.com/laboratory_service
```

**Lưu ý:** Render tự động set `DATABASE_URL` nếu database và services trong cùng Project.

---

## 🚀 Backend Services Config

**Repository:** `https://github.com/ThinhTran2412/OJT`  
**Branch:** `master`  
**Type:** Monorepo

### IAM_Service
- **Root Directory:** `OJT_Laboratory_Project/IAM_Service`
- **Dockerfile:** `IAM_Service.API/Dockerfile`

### Laboratory_Service
- **Root Directory:** `OJT_Laboratory_Project/Laboratory_Service`
- **Dockerfile:** `Laboratory_Service.API/Dockerfile`

### Monitoring_Service
- **Root Directory:** `OJT_Laboratory_Project/Monitoring_Service`
- **Dockerfile:** `Monitoring_Service.API/Dockerfile`

### Simulator_Service
- **Root Directory:** `OJT_Laboratory_Project/Simulator_Service`
- **Dockerfile:** `Simulator.API/Dockerfile`

---

## 🎨 Frontend Config

**Repository:** `https://github.com/ThinhTran2412/OJT`  
**Branch:** `master`  
**Root Directory:** `OJT_Laboratory_Project/Front_End`

**Build Command:**
```bash
npm install && npm run build
```

**Publish Directory:** `dist`

**Environment Variables:**
- `VITE_API_BASE_URL` - URL của backend API

---

## ✅ Checklist

### Database
- [ ] Database đã được tạo trên Render
- [ ] DATABASE_URL đã được set (tự động hoặc manual)
- [ ] Migrations đã chạy cho tất cả services

### Backend Services
- [ ] IAM_Service deployed
- [ ] Laboratory_Service deployed
- [ ] Monitoring_Service deployed
- [ ] Simulator_Service deployed
- [ ] Tất cả services đã start thành công
- [ ] Test API endpoints

### Frontend
- [ ] Frontend deployed
- [ ] Environment variables đã set
- [ ] API URL đã cấu hình đúng
- [ ] Test frontend hoạt động

---

Xem chi tiết trong: **DEPLOYMENT_GUIDE.md**

