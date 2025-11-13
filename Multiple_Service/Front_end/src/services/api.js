import axios from "axios";

// Cấu hình Axios Chuẩn hiện tại 
// In production, use VITE_API_BASE_URL environment variable
// If not set, try to construct from current origin (for same-domain deployment)
const getBaseURL = () => {
  if (!import.meta.env.PROD) {
    return "/api"; // Development: use proxy
  }
  
  // Production: use environment variable or construct from origin
  const envUrl = import.meta.env.VITE_API_BASE_URL;
  if (envUrl) {
    return envUrl;
  }
  
  // Fallback: try to construct API URL from current origin
  // This assumes API is on a subdomain or different path
  const origin = window.location.origin;
  // If frontend is on ojt-front-end.onrender.com, API might be on iam-service.onrender.com
  // You should set VITE_API_BASE_URL environment variable on Render instead
  console.error('⚠️ VITE_API_BASE_URL not set! Frontend cannot connect to API.');
  console.error('Please set VITE_API_BASE_URL environment variable on Render:');
  console.error('  Key: VITE_API_BASE_URL');
  console.error('  Value: https://your-iam-service-url.onrender.com/api');
  console.error('Current origin:', origin);
  console.error('Using fallback URL (may not work):', `${origin}/api`);
  return `${origin}/api`; // Fallback - may not work
};

const api = axios.create({
  baseURL: getBaseURL(),
  headers: {
    "Content-Type": "application/json",
  },
  timeout: 15000, 
  withCredentials: false,
});


api.interceptors.request.use(
  (config) => {
    const accessToken = localStorage.getItem('accessToken');
    if (accessToken) {
      config.headers.Authorization = `Bearer ${accessToken}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);


api.interceptors.response.use(
  (response) => {
    return response;
  },
  async (error) => {
    const status = error.response?.status;
    const originalRequest = error.config;

    if (
      status === 401 &&
      originalRequest &&
      !originalRequest._retry &&
      !window.location.pathname.includes('/login') &&
      !originalRequest?.url?.includes('/Auth/refresh')
    ) {
      originalRequest._retry = true;
      try {
        const refreshToken = localStorage.getItem('refreshToken');
        if (!refreshToken) {
          throw new Error('Missing refresh token');
        }

        const refreshClient = axios.create({
          baseURL: api.defaults.baseURL,
          headers: { 'Content-Type': 'application/json' },
          timeout: 15000,
          withCredentials: false,
        });

        const refreshResponse = await refreshClient.post('/Auth/refresh', { refreshToken });
        if (refreshResponse.status === 200) {
          const { accessToken: newAccessToken, refreshToken: newRefreshToken } = refreshResponse.data || {};
          if (!newAccessToken || !newRefreshToken) {
            throw new Error('Invalid refresh response');
          }

          localStorage.setItem('accessToken', newAccessToken);
          localStorage.setItem('refreshToken', newRefreshToken);

          originalRequest.headers = originalRequest.headers || {};
          originalRequest.headers.Authorization = `Bearer ${newAccessToken}`;

          return api(originalRequest);
        }
      } catch (refreshError) {
        // fallthrough
      }

      try {
        localStorage.clear();
        sessionStorage.clear();
      } catch (e) {
        // no-op
      }
      window.location.href = '/login';
      return Promise.reject(error);
    }

    return Promise.reject(error);
  }
);

export default api;
