// hooks/useProtectedRoute.js
// EXTENSION POINTS:
// 1. Thêm convenience methods cho specific roles (isAdmin, isManager, etc.)
// 2. Thêm permission checking (checkPermission, hasPermission)
// 3. Thêm role hierarchy support (isHigherThan, canAccess)
// 4. Thêm department/team based access control

import { useAuthStore } from '../store/authStore';

export const useProtectedRoute = () => {
  const { isAuthenticated, role } = useAuthStore();

  const checkRole = (requiredRole) => {
    if (!isAuthenticated || !role) return false;
    return role === requiredRole;
  };

  const checkAnyRole = (requiredRoles) => {
    if (!isAuthenticated || !role) return false;
    return requiredRoles.includes(role);
  };

  // TODO: Thêm convenience methods khi có roles cụ thể
  // const isAdmin = checkRole('admin');
  // const isManager = checkRole('manager');
  // const isStaff = checkRole('staff');

  // TODO: Thêm permission checking
  // const checkPermission = (permission) => { ... };
  // const hasPermission = (permission) => { ... };

  return {
    isAuthenticated,
    role,
    checkRole,
    checkAnyRole
    // TODO: Thêm các methods mới ở đây
  };
};
