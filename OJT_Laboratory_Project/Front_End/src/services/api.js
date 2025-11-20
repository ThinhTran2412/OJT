import axios from "axios";

// Current Axios configuration 
const api = axios.create({
  baseURL: import.meta.env.PROD
    ? import.meta.env.VITE_API_BASE_URL
    : "/api",
  headers: {
    "Content-Type": "application/json",
  },
  timeout: 15000, 
  withCredentials: false,
});

api.interceptors.request.use(
  (config) => {
    // Ensure /api prefix in production
    if (import.meta.env.PROD) {
      const baseURL = api.defaults.baseURL || '';
      if (baseURL && !baseURL.endsWith('/api')) {
        // Production: baseURL is full domain (https://iam-service-fz3h.onrender.com)
        // Add /api prefix to all routes
        if (config.url && !config.url.startsWith('/api/') && !config.url.startsWith('http')) {
          config.url = `/api${config.url.startsWith('/') ? '' : '/'}${config.url}`;
        }
      }
    }
    
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

    // Suppress console errors for expected 404s on AI review endpoints
    if (status === 404 && originalRequest?.url?.includes('/ai-review/')) {
      // This is expected - test orders without AI review enabled will return 404
      // Don't log to console to avoid noise
      return Promise.reject(error);
    }

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
        
        // Add /api prefix interceptor for refreshClient too
        refreshClient.interceptors.request.use((config) => {
          if (import.meta.env.PROD) {
            const baseURL = refreshClient.defaults.baseURL || '';
            if (baseURL && !baseURL.endsWith('/api')) {
              if (config.url && !config.url.startsWith('/api/') && !config.url.startsWith('http')) {
                config.url = `/api${config.url.startsWith('/') ? '' : '/'}${config.url}`;
              }
            }
          }
          return config;
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
