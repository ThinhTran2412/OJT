import React from "react";
import ReactDOM from "react-dom/client";
import App from "./App.jsx"; // Giữ nguyên để không ảnh hưởng logic cũ
import AppRouter from "./routes/App_Route.jsx"; // Thêm router vào
import "./index.css";
import "antd/dist/reset.css";
import { useAuthStore } from "./store/authStore";

// ✅ Khởi tạo auth store khi app start
useAuthStore.getState().initializeAuth();

ReactDOM.createRoot(document.getElementById("root")).render(
  <React.StrictMode>
    {/* App vẫn bao ngoài để không ảnh hưởng Context, Layout, Theme... */}
    <App>
      <AppRouter />
    </App>
  </React.StrictMode>
);
