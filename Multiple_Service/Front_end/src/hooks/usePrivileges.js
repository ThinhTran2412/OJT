import { useEffect } from 'react';
import api from '../services/api';
import { usePrivilegeStore } from '../store/privilegeStore';
import { useAuthStore } from '../store/authStore';

export const usePrivileges = () => {
  const { privileges, loading, error, setPrivileges, setLoading, setError } = usePrivilegeStore();
  const { isAuthenticated } = useAuthStore();

  const fetchPrivileges = async () => {
    // Kiểm tra authentication trước khi fetch
    if (!isAuthenticated) {
      console.warn('User not authenticated, skipping privileges fetch');
      return;
    }

    try {
      setLoading(true);
      setError(null);
      
      const response = await api.get('/Privileges');
      
      
      if (response.status === 200) {
        setPrivileges(response.data);
      }
    } catch (error) {
      console.error('Error fetching privileges:', error);
      console.error('Error details:', {
        message: error.message,
        code: error.code,
        response: error.response?.data,
        status: error.response?.status,
        statusText: error.response?.statusText
      });
      
      // Xử lý các loại lỗi khác nhau
      if (error.response?.status === 401) {
        setError('Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.');
      } else if (error.response?.status === 403) {
        setError('Bạn không có quyền truy cập thông tin quyền hạn.');
      } else if (error.response?.status >= 500) {
        setError('Lỗi máy chủ. Vui lòng thử lại sau.');
      } else {
        setError(error.message || 'Không thể tải danh sách quyền hạn');
      }
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    // Chỉ fetch nếu đã authenticated, chưa có data và không đang loading
    if (isAuthenticated && privileges.length === 0 && !loading) {
      fetchPrivileges();
    }
  }, [isAuthenticated]);

  return {
    privileges,
    loading,
    error,
    refetch: fetchPrivileges
  };
};
