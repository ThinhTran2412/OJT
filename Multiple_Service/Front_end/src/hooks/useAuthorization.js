import { useCallback, useState, useEffect } from "react";

export function useAuthorization() {
  const [privileges, setPrivileges] = useState([]);

  useEffect(() => {
    // Lấy user từ sessionStorage mỗi khi load lại trang
    const storedUser = sessionStorage.getItem("user");
    if (storedUser) {
      const user = JSON.parse(storedUser);
      setPrivileges(user.privileges || []);
    }
  }, []);

  // Kiểm tra 1 quyền cụ thể
  const can = useCallback(
    (privilegeName) => privileges.includes(privilegeName),
    [privileges]
  );

  // Kiểm tra nếu có ít nhất 1 trong các quyền (OR)
  const canAny = useCallback(
    (list) => Array.isArray(list) && list.some((p) => privileges.includes(p)),
    [privileges]
  );

  // Kiểm tra nếu có đủ tất cả các quyền (AND)
  const canAll = useCallback(
    (list) => Array.isArray(list) && list.every((p) => privileges.includes(p)),
    [privileges]
  );

  return { can, canAny, canAll, privileges };
}
