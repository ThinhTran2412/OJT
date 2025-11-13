import { useState, useEffect } from "react";
import api from "../../services/api";
import UserFilters from "../../components/User_Management/UserFilters";
import UserTable from "../../components/User_Management/UserTable";
import { useAuthStore } from "../../store/authStore";
import DashboardLayout from "../../layouts/DashboardLayout";
// JWT Decode function
const decodeJWT = (token) => {
  try {
    const base64Url = token.split('.')[1];
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    const jsonPayload = decodeURIComponent(atob(base64).split('').map(function(c) {
      return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
    }).join(''));
    return JSON.parse(jsonPayload);
  } catch (error) {
    console.error('Error decoding JWT:', error);
    return null;
  }
};

export default function UserManagement() {
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(false);
  const [refreshKey, setRefreshKey] = useState(0);

  // Lấy access token từ store
  const { accessToken } = useAuthStore();

  // Decode JWT token để lấy privileges
  const tokenPayload = accessToken ? decodeJWT(accessToken) : null;
  
  // Try different possible field names for privileges
  const jwtPrivileges = tokenPayload?.privilege || 
                        tokenPayload?.privileges || 
                        tokenPayload?.Privilege || 
                        tokenPayload?.Privileges || 
                        tokenPayload?.claims?.privilege ||
                        tokenPayload?.claims?.privileges ||
                        tokenPayload?.claims?.Privilege ||
                        tokenPayload?.claims?.Privileges ||
                        tokenPayload?.permissions ||
                        tokenPayload?.Permissions ||
                        tokenPayload?.roles ||
                        tokenPayload?.Roles ||
                        [];
  
  // Fallback to localStorage user privileges
  const localStorageUser = localStorage.getItem('user');
  const parsedUser = localStorageUser ? JSON.parse(localStorageUser) : null;
  const localStoragePrivileges = parsedUser?.privileges || [];
  
  // Use JWT privileges first, fallback to localStorage
  let userPrivileges = jwtPrivileges.length > 0 ? jwtPrivileges : localStoragePrivileges;
  
  // If still empty, try to get from localStorage user object with different field names
  if (userPrivileges.length === 0 && parsedUser) {
    userPrivileges = parsedUser?.privileges || 
                    parsedUser?.Privileges || 
                    parsedUser?.permissions ||
                    parsedUser?.Permissions ||
                    parsedUser?.roles ||
                    parsedUser?.Roles ||
                    [];
  }

  // Debug privileges
  console.log('=== JWT DEBUG ===');
  console.log('Access token:', accessToken);
  console.log('Token payload:', tokenPayload);
  console.log('Token payload keys:', tokenPayload ? Object.keys(tokenPayload) : 'No payload');
  
  // Debug each possible privilege field
  console.log('Checking privilege fields:');
  console.log('  tokenPayload.privilege:', tokenPayload?.privilege);
  console.log('  tokenPayload.privileges:', tokenPayload?.privileges);
  console.log('  tokenPayload.Privilege:', tokenPayload?.Privilege);
  console.log('  tokenPayload.Privileges:', tokenPayload?.Privileges);
  console.log('  tokenPayload.claims:', tokenPayload?.claims);
  console.log('  tokenPayload.claims?.privilege:', tokenPayload?.claims?.privilege);
  console.log('  tokenPayload.claims?.privileges:', tokenPayload?.claims?.privileges);
  console.log('  tokenPayload.claims?.Privilege:', tokenPayload?.claims?.Privilege);
  console.log('  tokenPayload.claims?.Privileges:', tokenPayload?.claims?.Privileges);
  console.log('  tokenPayload.permissions:', tokenPayload?.permissions);
  console.log('  tokenPayload.Permissions:', tokenPayload?.Permissions);
  console.log('  tokenPayload.roles:', tokenPayload?.roles);
  console.log('  tokenPayload.Roles:', tokenPayload?.Roles);
  
  console.log('JWT privileges:', jwtPrivileges);
  console.log('localStorage privileges:', localStoragePrivileges);
  console.log('Final user privileges:', userPrivileges);
  console.log('Privileges type:', typeof userPrivileges);
  console.log('Privileges length:', userPrivileges?.length);
  console.log('================');

  // Modal state
  const [selectedUser, setSelectedUser] = useState(null);
  const [userDetails, setUserDetails] = useState(null);
  const [modalLoading, setModalLoading] = useState(false);
  const [showModal, setShowModal] = useState(false);

  // Filter & Sort State
  const [keyword, setKeyword] = useState("");
  const [filterField, setFilterField] = useState("");
  // store gender as an array to support multi-select filters from Ant Design Table
  const [gender, setGender] = useState([]);
  const [minAge, setMinAge] = useState("");
  const [maxAge, setMaxAge] = useState("");
  const [address, setAddress] = useState("");
  const [sortBy, setSortBy] = useState("");
  const [sortOrder, setSortOrder] = useState("asc");

  // Validation State
  const [validationErrors, setValidationErrors] = useState({});

  // Check if current user can view user details based on privileges
  const canViewUserDetails = () => {
    // Privilege cần thiết để xem user details
    const requiredPrivilege = 'VIEW_USER';
    
    // Kiểm tra xem user có privilege VIEW_USER không
    const hasViewPrivilege = userPrivileges.includes(requiredPrivilege);
    
    // Temporary fix: Force enable for testing (DISABLED for proper privilege check)
    const forceEnable = false;
    
    console.log('Privilege check:', {
      userPrivileges,
      requiredPrivilege,
      hasViewPrivilege,
      forceEnable,
      canView: hasViewPrivilege || forceEnable
    });
    
    return hasViewPrivilege || forceEnable;
  };

  // Fetch user details with role and privileges
  const fetchUserDetails = async (user) => {
    try {
      setModalLoading(true);
      // Backend expects email parameter, not userId
      const response = await api.get(`/User/detail?email=${user.email}`);
      setUserDetails(response.data);
    } catch (error) {
      console.error("Error fetching user details:", error);
      setUserDetails(null);
    } finally {
      setModalLoading(false);
    }
  };

  // Handle view button click
  const handleViewClick = async (u) => {
    if (!canViewUserDetails()) {
      alert("Bạn không có quyền xem chi tiết người dùng");
      return;
    }

    setSelectedUser(u);
    setShowModal(true);
    await fetchUserDetails(u);
  };

  // Close modal
  const closeModal = () => {
    setShowModal(false);
    setSelectedUser(null);
    setUserDetails(null);
  };

  // Validation functions
  const validateMinAge = (value) => {
    // Consider 0 as a valid provided value, so check for null/empty explicitly
    const provided = value !== '' && value !== null && value !== undefined;
    if (provided) {
      const num = Number(value);
      if (isNaN(num) || num < 0) {
        return "MinAge must be greater than or equal to 0";
      }
    }
    return null;
  };

  const validateMaxAge = (value) => {
    // Consider 0 as a valid provided value, so check for null/empty explicitly
    const provided = value !== '' && value !== null && value !== undefined;
    if (provided) {
      const num = Number(value);
      if (isNaN(num) || num < 0) {
        return "MaxAge must be greater than or equal to 0";
      }
    }
    return null;
  };

  const validateAgeRange = (minAge, maxAge) => {
    const minProvided = minAge !== '' && minAge !== null && minAge !== undefined;
    const maxProvided = maxAge !== '' && maxAge !== null && maxAge !== undefined;
    if (minProvided && maxProvided) {
      const minNum = Number(minAge);
      const maxNum = Number(maxAge);
      if (!isNaN(minNum) && !isNaN(maxNum) && minNum >= maxNum) {
        return "MinAge must be less than MaxAge";
      }
    }
    return null;
  };

  // Validate all fields
  const validateFields = () => {
    const errors = {};

    const minAgeError = validateMinAge(minAge);
    if (minAgeError) errors.minAge = minAgeError;

    const maxAgeError = validateMaxAge(maxAge);
    if (maxAgeError) errors.maxAge = maxAgeError;

    const ageRangeError = validateAgeRange(minAge, maxAge);
    if (ageRangeError) errors.ageRange = ageRangeError;

    setValidationErrors(errors);
    return Object.keys(errors).length === 0;
  };

  useEffect(() => {
    const fetchUsers = async () => {
      try {
        setLoading(true);

        // Validate fields before making API call
        if (!validateFields()) {
          setLoading(false);
          return;
        }

        // Normalize gender for API: backend usually expects a string or undefined.
        // If user selects multiple genders, send as comma-separated string (e.g. 'male,female').
        let genderParam;
        if (Array.isArray(gender)) {
          if (gender.length === 0) genderParam = undefined;
          else if (gender.length === 1) genderParam = gender[0];
          else genderParam = gender.join(',');
        } else {
          genderParam = gender || undefined;
        }

        const params = {
          Keyword: keyword,
          FilterField: filterField,
          Gender: genderParam,
          MinAge: minAge || undefined,
          MaxAge: maxAge || undefined,
          Address: address,
          SortBy: sortBy,
          SortOrder: sortOrder,
        };
        const res = await api.get("/User/getListOfUser", { params });
        setUsers(res.data);
      } catch (err) {
        console.error("Error fetching users:", err);
      } finally {
        setLoading(false);
      }
    };

    fetchUsers();
  }, [keyword, filterField, gender, minAge, maxAge, address, sortBy, sortOrder, refreshKey]);

  // Handlers for UserFilters
  const handleSearch = (searchValue, field = '') => {
    setKeyword(searchValue || '');
    setFilterField(field || '');
  };

  const handleAgeFilter = (min, max) => {
    setMinAge(min ?? '');
    setMaxAge(max ?? '');
  };

  const handleClearFilters = () => {
    setKeyword('');
    setFilterField('');
    setGender([]);
    setMinAge('');
    setMaxAge('');
    setAddress('');
    setSortBy('');
    setSortOrder('asc');
    setValidationErrors({});
  };

  // Handle table change (sorting) from Ant Design Table
  const handleTableChange = (_pagination, filters, sorter) => {
    // Handle sorting
    const s = Array.isArray(sorter) ? sorter[0] : sorter;
    if (s) {
      if (s.field) {
        setSortBy(s.field || '');
      }
      if (s.order) {
        setSortOrder(s.order === 'descend' ? 'desc' : 'asc');
      } else {
        setSortOrder('asc');
      }
    }
    // Handle gender filter coming from AntD Table filters (can be multi-select array)
    if (filters && Object.prototype.hasOwnProperty.call(filters, 'gender')) {
      if (Array.isArray(filters.gender) && filters.gender.length > 0) {
        // store full array so we preserve multi-select (male + female)
        setGender(filters.gender);
      } else {
        setGender([]);
      }
    }
  };

  // Handle delete user
  const handleDeleteUser = async (user) => {
    const name = user?.fullName || user?.email || 'this user';
    const confirmed = window.confirm(`Bạn có chắc muốn xóa ${name}?`);
    if (!confirmed) return;

    try {
      setLoading(true);
      // Delete by userId using DELETE method
      const userId = user?.userId || user?.id;
      if (!userId) throw new Error('Missing userId');
      await api.delete(`/User/delete/${userId}`);
      // refresh list
      setRefreshKey((k) => k + 1);
    } catch (error) {
      console.error('Error deleting user:', error);
      alert('Xóa người dùng thất bại. Vui lòng thử lại.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <DashboardLayout>
    <div className="flex flex-col min-h-screen bg-gray-50">
      {/* Header */}
      <div className="bg-white shadow-sm border-b border-gray-200 px-6 py-4">
        <h1 className="text-2xl font-bold text-gray-900">User Management</h1>
      </div>

      {/* Filter & Sort Controls (replaced by UserFilters) */}
      <div className="px-6 py-4">
        <UserFilters
          filters={{
            keyword,
            filterField,
            minAge,
            maxAge,
          }}
          onSearch={handleSearch}
          onAgeFilter={handleAgeFilter}
          onClear={handleClearFilters}
        />
      </div>

      {/* Validation Error Messages */}
      {Object.keys(validationErrors).length > 0 && (
        <div className="px-4 pb-2">
          <div className="bg-red-50 border border-red-200 rounded-lg p-3">
            <div className="flex">
              <div className="flex-shrink-0">
                <svg
                  className="h-5 w-5 text-red-400"
                  viewBox="0 0 20 20"
                  fill="currentColor"
                >
                  <path
                    fillRule="evenodd"
                    d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z"
                    clipRule="evenodd"
                  />
                </svg>
              </div>
              <div className="ml-3">
                <h3 className="text-sm font-medium text-red-800">
                  Validation Errors:
                </h3>
                <div className="mt-2 text-sm text-red-700">
                  <ul className="list-disc list-inside space-y-1">
                    {Object.values(validationErrors).map((error, index) => (
                      <li key={index}>{error}</li>
                    ))}
                  </ul>
                </div>
              </div>
            </div>
          </div>
        </div>
      )}

      {/* Table (replaced by UserTable) */}
      <div className="flex-1 p-6">
        <UserTable
          users={users}
          loading={loading}
          onView={handleViewClick}
          onDelete={handleDeleteUser}
          onChange={handleTableChange}
          genderFilter={gender}
        />
      </div>

      {/* User Details Modal */}
      {showModal && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-lg shadow-xl max-w-4xl w-full max-h-[90vh] overflow-hidden">
            {/* Modal Header */}
            <div className="bg-gradient-to-r from-blue-600 to-blue-700 text-white px-6 py-4">
              <div className="flex items-center justify-between">
                <div>
                  <h2 className="text-xl font-bold">User Details</h2>
                  <p className="text-blue-100">{selectedUser?.fullName}</p>
                </div>
                <button
                  onClick={closeModal}
                  className="text-white hover:text-gray-200 transition-colors"
                >
                  <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                  </svg>
                </button>
              </div>
            </div>

            {/* Modal Content */}
            <div className="p-6 overflow-y-auto max-h-[calc(90vh-120px)]">
              {modalLoading ? (
                <div className="flex items-center justify-center py-12">
                  <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
                  <span className="ml-3 text-gray-600">Loading user details...</span>
                </div>
              ) : userDetails ? (
                <div className="space-y-6">
                  {/* User Basic Info */}
                  <div className="bg-gray-50 rounded-lg p-4">
                    <h3 className="text-lg font-semibold text-gray-900 mb-4">Basic Information</h3>
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                      <div>
                        <label className="block text-sm font-medium text-gray-600">Email</label>
                        <p className="text-gray-900">{userDetails.email}</p>
                      </div>
                      <div>
                        <label className="block text-sm font-medium text-gray-600">Phone</label>
                        <p className="text-gray-900">{userDetails.phoneNumber || "-"}</p>
                      </div>
                      <div>
                        <label className="block text-sm font-medium text-gray-600">Age</label>
                        <p className="text-gray-900">{userDetails.age}</p>
                      </div>
                      <div>
                        <label className="block text-sm font-medium text-gray-600">Gender</label>
                        <p className="text-gray-900">{userDetails.gender || "-"}</p>
                      </div>
                    </div>
                  </div>

                  {/* Role Information */}
                  <div className="bg-blue-50 rounded-lg p-4">
                    <h3 className="text-lg font-semibold text-gray-900 mb-4">Role Information</h3>
                    <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                      <div>
                        <label className="block text-sm font-medium text-gray-600">Role ID</label>
                        <div className="mt-1">
                          <span className="inline-flex items-center px-3 py-1 rounded-full text-sm font-medium bg-blue-100 text-blue-800">
                            {userDetails.roleId}
                          </span>
                        </div>
                      </div>
                      <div>
                        <label className="block text-sm font-medium text-gray-600">Role Name</label>
                        <div className="mt-1">
                          <span className="inline-flex items-center px-3 py-1 rounded-full text-sm font-medium bg-green-100 text-green-800">
                            {userDetails.roleName}
                          </span>
                        </div>
                      </div>
                      <div>
                        <label className="block text-sm font-medium text-gray-600">Role Code</label>
                        <p className="text-gray-900">{userDetails.roleCode || "-"}</p>
                      </div>
                    </div>
                  </div>

                  {/* Privileges */}
                  <div className="bg-purple-50 rounded-lg p-4">
                    <h3 className="text-lg font-semibold text-gray-900 mb-4">Privileges</h3>
                    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-2">
                      {userDetails.privileges && userDetails.privileges.length > 0 ? (
                        userDetails.privileges.map((privilege, index) => (
                          <div
                            key={index}
                            className="bg-white border border-purple-200 rounded-lg px-3 py-2 text-sm font-medium text-purple-800 hover:bg-purple-100 transition-colors"
                          >
                            {privilege}
                          </div>
                        ))
                      ) : (
                        <p className="text-gray-500 col-span-full">No privileges assigned</p>
                      )}
                    </div>
                  </div>
                </div>
              ) : (
                <div className="text-center py-12">
                  <svg className="w-12 h-12 text-red-400 mx-auto mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                  </svg>
                  <h3 className="text-lg font-medium text-gray-900 mb-2">Error Loading User Details</h3>
                  <p className="text-gray-500">Unable to fetch user information. Please try again.</p>
                </div>
              )}
            </div>

            {/* Modal Footer */}
            <div className="bg-gray-50 px-6 py-4 flex justify-end">
              <button
                onClick={closeModal}
                className="px-4 py-2 bg-gray-600 text-white rounded-lg hover:bg-gray-700 transition-colors"
              >
                Close
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
    </DashboardLayout>
  );
}
