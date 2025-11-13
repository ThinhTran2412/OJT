import { useEffect, useMemo, useState } from 'react';
import { RefreshCcw, Search, FileText, Users, CalendarDays } from 'lucide-react';
import DashboardLayout from '../../layouts/DashboardLayout';
import { getAllMedicalRecords } from '../../services/MedicalRecordService';

const formatDate = (value) => {
  if (!value) return 'N/A';
  const date = new Date(value);
  if (Number.isNaN(date.getTime())) return 'N/A';
  return date.toLocaleDateString('en-US', {
    month: '2-digit',
    day: '2-digit',
    year: 'numeric',
  });
};

const formatDateTime = (value) => {
  if (!value) return 'N/A';
  const date = new Date(value);
  if (Number.isNaN(date.getTime())) return 'N/A';
  return date.toLocaleString('en-US', {
    month: '2-digit',
    day: '2-digit',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  });
};

const normalizeRecords = (data) => {
  if (!Array.isArray(data)) return [];
  return data.map((record) => ({
    ...record,
    patientName: record.patientName || 'Unknown patient',
    dateOfBirth:
      !record.dateOfBirth || record.dateOfBirth === '0001-01-01'
        ? null
        : record.dateOfBirth,
    age: typeof record.age === 'number' && record.age >= 0 ? record.age : null,
    gender: record.gender?.toLowerCase() === 'female' ? 'Female' : 'Male',
    email: record.email || 'N/A',
    phoneNumber: record.phoneNumber || 'N/A',
    address: record.address || 'N/A',
    createdBy: record.createdBy || 'System',
    testOrders: Array.isArray(record.testOrders) ? record.testOrders : [],
  }));
};

export default function MedicalRecordListPage() {
  const [records, setRecords] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [searchTerm, setSearchTerm] = useState('');

  useEffect(() => {
    fetchRecords();
  }, []);

  const fetchRecords = async () => {
    try {
      setLoading(true);
      setError(null);

      const data = await getAllMedicalRecords();
      setRecords(normalizeRecords(data));
    } catch (err) {
      console.error('Error fetching medical records:', err);
      setError(
        err.response?.data?.message ||
          err.message ||
          'Failed to load medical records. Please try again.'
      );
      setRecords([]);
    } finally {
      setLoading(false);
    }
  };

  const filteredRecords = useMemo(() => {
    if (!searchTerm.trim()) return records;

    const normalizedTerm = searchTerm.trim().toLowerCase();
    return records.filter((record) => {
      return [
        record.patientName,
        record.email,
        record.phoneNumber,
        record.address,
        record.createdBy,
        String(record.medicalRecordId),
      ]
        .filter(Boolean)
        .some((field) => field.toLowerCase().includes(normalizedTerm));
    });
  }, [records, searchTerm]);

  const summary = useMemo(() => {
    const totalRecords = records.length;
    const uniquePatients = new Set(records.map((record) => record.patientName)).size;
    const today = new Date();
    const createdToday = records.filter((record) => {
      const createdAt = new Date(record.createdAt);
      return (
        !Number.isNaN(createdAt.getTime()) &&
        createdAt.getUTCFullYear() === today.getUTCFullYear() &&
        createdAt.getUTCMonth() === today.getUTCMonth() &&
        createdAt.getUTCDate() === today.getUTCDate()
      );
    }).length;

    return {
      totalRecords,
      uniquePatients,
      createdToday,
    };
  }, [records]);

  return (
    <DashboardLayout>
      <div className="min-h-screen bg-gray-50">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
          <div className="flex flex-col gap-6">
            {/* Page header */}
            <div className="flex flex-wrap items-center justify-between gap-4">
              <div>
                <h1 className="text-3xl font-bold text-gray-900 flex items-center gap-2">
                  <FileText className="w-8 h-8 text-blue-600" />
                  Medical Records
                </h1>
                <p className="text-gray-600 mt-1">
                  View and manage all patient medical records in the laboratory system.
                </p>
              </div>
              <button
                onClick={fetchRecords}
                disabled={loading}
                className="inline-flex items-center gap-2 bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors disabled:opacity-60 disabled:cursor-not-allowed"
              >
                <RefreshCcw className={`w-4 h-4 ${loading ? 'animate-spin' : ''}`} />
                Refresh
              </button>
            </div>

            {/* Summary cards */}
            <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
              <div className="bg-white rounded-xl shadow-sm border border-gray-100 p-5">
                <div className="flex items-center gap-3">
                  <div className="bg-blue-100 text-blue-700 w-10 h-10 rounded-lg flex items-center justify-center">
                    <FileText className="w-6 h-6" />
                  </div>
                  <div>
                    <p className="text-sm text-gray-500">Total Records</p>
                    <p className="text-2xl font-semibold text-gray-900">{summary.totalRecords}</p>
                  </div>
                </div>
              </div>
              <div className="bg-white rounded-xl shadow-sm border border-gray-100 p-5">
                <div className="flex items-center gap-3">
                  <div className="bg-emerald-100 text-emerald-700 w-10 h-10 rounded-lg flex items-center justify-center">
                    <Users className="w-6 h-6" />
                  </div>
                  <div>
                    <p className="text-sm text-gray-500">Unique Patients</p>
                    <p className="text-2xl font-semibold text-gray-900">{summary.uniquePatients}</p>
                  </div>
                </div>
              </div>
              <div className="bg-white rounded-xl shadow-sm border border-gray-100 p-5">
                <div className="flex items-center gap-3">
                  <div className="bg-purple-100 text-purple-700 w-10 h-10 rounded-lg flex items-center justify-center">
                    <CalendarDays className="w-6 h-6" />
                  </div>
                  <div>
                    <p className="text-sm text-gray-500">Created Today</p>
                    <p className="text-2xl font-semibold text-gray-900">{summary.createdToday}</p>
                  </div>
                </div>
              </div>
            </div>

            {/* Search bar */}
            <div className="bg-white rounded-lg shadow-sm border border-gray-100 p-5">
              <div className="flex flex-col sm:flex-row gap-4 sm:items-center sm:justify-between">
                <div className="relative flex-1">
                  <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 w-5 h-5" />
                  <input
                    type="text"
                    value={searchTerm}
                    onChange={(event) => setSearchTerm(event.target.value)}
                    placeholder="Search by patient name, contact, creator, or record ID"
                    className="w-full pl-10 pr-4 py-2 border border-gray-200 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                  />
                </div>
                <div className="text-sm text-gray-500">
                  Showing <span className="font-medium text-gray-900">{filteredRecords.length}</span> of{' '}
                  <span className="font-medium text-gray-900">{records.length}</span> records
                </div>
              </div>
            </div>

            {/* Error state */}
            {error && (
              <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-lg flex items-start gap-3">
                <span className="font-medium">Error:</span>
                <span>{error}</span>
              </div>
            )}

            {/* Table */}
            <div className="bg-white rounded-lg shadow-sm border border-gray-100 overflow-hidden">
              {loading ? (
                <div className="flex justify-center items-center py-16">
                  <div className="text-center">
                    <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto mb-4" />
                    <p className="text-gray-600">Loading medical records...</p>
                  </div>
                </div>
              ) : filteredRecords.length === 0 ? (
                <div className="text-center py-16 px-6">
                  <p className="text-gray-700 text-lg font-medium mb-1">No medical records found.</p>
                  <p className="text-gray-500 text-sm">
                    Try adjusting your search keywords or refresh the list to load the latest data.
                  </p>
                </div>
              ) : (
                <div className="overflow-x-auto">
                  <table className="min-w-full divide-y divide-gray-200">
                    <thead className="bg-gray-50">
                      <tr>
                        <th className="px-6 py-3 text-left text-xs font-semibold text-gray-500 uppercase tracking-wider">
                          Record ID
                        </th>
                        <th className="px-6 py-3 text-left text-xs font-semibold text-gray-500 uppercase tracking-wider">
                          Patient
                        </th>
                        <th className="px-6 py-3 text-left text-xs font-semibold text-gray-500 uppercase tracking-wider">
                          Contact
                        </th>
                        <th className="px-6 py-3 text-left text-xs font-semibold text-gray-500 uppercase tracking-wider">
                          Address
                        </th>
                        <th className="px-6 py-3 text-left text-xs font-semibold text-gray-500 uppercase tracking-wider">
                          Created
                        </th>
                        <th className="px-6 py-3 text-left text-xs font-semibold text-gray-500 uppercase tracking-wider">
                          Created By
                        </th>
                        <th className="px-6 py-3 text-left text-xs font-semibold text-gray-500 uppercase tracking-wider text-center">
                          Test Orders
                        </th>
                      </tr>
                    </thead>
                    <tbody className="bg-white divide-y divide-gray-200">
                      {filteredRecords.map((record) => (
                        <tr key={record.medicalRecordId} className="hover:bg-gray-50 transition-colors">
                          <td className="px-6 py-4 whitespace-nowrap text-sm font-semibold text-gray-900">
                            MR-{String(record.medicalRecordId).padStart(4, '0')}
                          </td>
                          <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                            <div>
                              <p className="font-medium">{record.patientName}</p>
                              <p className="text-xs text-gray-500">
                                DOB: {record.dateOfBirth ? formatDate(record.dateOfBirth) : 'N/A'}
                                {record.age ? ` · ${record.age} yrs` : ''}
                              </p>
                              <p className="text-xs text-gray-500 capitalize">Gender: {record.gender}</p>
                            </div>
                          </td>
                          <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                            <div className="flex flex-col gap-1">
                              <span>{record.phoneNumber}</span>
                              <span className="text-xs text-blue-600 break-all">{record.email}</span>
                            </div>
                          </td>
                          <td className="px-6 py-4 text-sm text-gray-900 max-w-xs">
                            <span className="line-clamp-2" title={record.address}>
                              {record.address}
                            </span>
                          </td>
                          <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                            <div className="flex flex-col">
                              <span>{formatDateTime(record.createdAt)}</span>
                              {record.updatedAt && (
                                <span className="text-xs text-gray-500">
                                  Updated {formatDateTime(record.updatedAt)}
                                </span>
                              )}
                            </div>
                          </td>
                          <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                            {record.createdBy}
                          </td>
                          <td className="px-6 py-4 whitespace-nowrap text-sm text-center text-gray-900">
                            <span className="inline-flex items-center justify-center px-3 py-1 rounded-full bg-blue-100 text-blue-700 text-xs font-semibold">
                              {record.testOrders.length}
                            </span>
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              )}
            </div>
          </div>
        </div>
      </div>
    </DashboardLayout>
  );
}

