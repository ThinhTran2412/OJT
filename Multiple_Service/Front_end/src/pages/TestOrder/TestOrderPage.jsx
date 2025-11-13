import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { Plus } from 'lucide-react';
import { message } from 'antd';
import DashboardLayout from '../../layouts/DashboardLayout';
import TestOrderFilters from '../../components/TestOrder_Management/TestOrderFilters';
import TestOrderTable from '../../components/TestOrder_Management/TestOrderTable';
import ExportExcelButton from '../../components/TestOrder_Management/ExportExcelButton';
import { getAllTestOrders, exportTestOrdersToExcel, getExportJobStatus } from '../../services/TestOrderService';
import backgroundJobManager from '../../utils/BackgroundJobManager';
import { getCurrentMonthRange } from '../../utils/fileUtils';

/**
 * Test Order Management Page
 * Single Responsibility: Page-level state and coordination
 */
export default function TestOrderPage() {
  const navigate = useNavigate();
  
  // Data state
  const [testOrders, setTestOrders] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  
  // Filter and sort state
  const [filters, setFilters] = useState({});
  const [sortBy, setSortBy] = useState(null);
  const [sortOrder, setSortOrder] = useState('asc');
  
  // Pagination state
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [totalCount, setTotalCount] = useState(0);
  
  // Selection state (AC02: one or multiple)
  const [selectedRowKeys, setSelectedRowKeys] = useState([]);
  
  // Export state (AC03: non-blocking)
  const [exportLoading, setExportLoading] = useState(false);

  useEffect(() => {
    fetchTestOrders();
  }, [currentPage, pageSize, filters, sortBy, sortOrder]);

  const fetchTestOrders = async () => {
    try {
      setLoading(true);
      setError(null);

      const appliedFilters = Object.keys(filters).length > 0 ? filters : {};

      const response = await getAllTestOrders({
        page: currentPage,
        pageSize: pageSize,
        filters: appliedFilters,
        sortBy: sortBy,
        sortOrder: sortOrder,
      });

      setTestOrders(response.data || []);
      setTotalCount(response.totalCount || 0);
    } catch (err) {
      console.error('Error fetching test orders:', err);
      setError('Failed to load test orders. Please try again.');
      setTestOrders([]);
    } finally {
      setLoading(false);
    }
  };

  const handleFilterChange = (newFilters) => {
    setFilters(newFilters);
    setCurrentPage(1);
    setSelectedRowKeys([]);
  };

  const handleClearFilters = () => {
    setFilters({});
    setCurrentPage(1);
    setSelectedRowKeys([]);
  };

  const handleSearch = () => {
    setCurrentPage(1);
    fetchTestOrders();
  };

  const handleSelectChange = (selectedKeys) => {
    setSelectedRowKeys(selectedKeys);
  };

  /**
   * Handle export (AC03: non-blocking background job)
   * Single Responsibility: Export job initiation
   */
  const handleExport = async (exportRequest) => {
    try {
      setExportLoading(true);
      setError(null);

      // Apply default filters if no selection and no filters
      if (!exportRequest.testOrderIds && !exportRequest.filters && exportRequest.exportAll) {
        exportRequest.filters = getCurrentMonthRange();
      }

      // Start export job (non-blocking)
      const response = await exportTestOrdersToExcel(exportRequest);
      const jobId = response.jobId;

      if (jobId) {
        const jobInfo = {
          status: 'pending',
          progress: 0,
          exportRequest,
          createdAt: new Date().toISOString(),
        };
        
        backgroundJobManager.registerJob(jobId, jobInfo);
        backgroundJobManager.startPolling(jobId, async (id) => {
          return await getExportJobStatus(id);
        }, 2000);
      }

      setExportLoading(false);
      message.success('Export job started. Check the notification in the bottom-right corner.');
    } catch (err) {
      console.error('Error starting export:', err);
      setError('Failed to start export. Please try again.');
      setExportLoading(false);
      message.error('Failed to start export job.');
    }
  };

  const handlePageChange = (page) => {
    setCurrentPage(page);
  };

  const handlePageSizeChange = (newPageSize) => {
    setPageSize(newPageSize);
    setCurrentPage(1);
  };

  const handleCreateNew = () => {
    navigate('/test-orders/create');
  };

  const handleRowClick = (record) => {
    navigate(`/test-orders/${record.testOrderId}`);
  };

  return (
    <DashboardLayout>
      <div className="min-h-screen bg-gray-50">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
          {/* Header */}
          <div className="mb-6">
            <h1 className="text-3xl font-bold text-gray-900 mb-2">Test Orders</h1>
            <p className="text-gray-600">Manage laboratory test orders and export reports</p>
          </div>

          {/* Error Message */}
          {error && (
            <div className="mb-6 p-4 bg-red-50 border border-red-200 rounded-lg">
              <p className="text-red-800">{error}</p>
            </div>
          )}

          {/* Filters */}
          <TestOrderFilters
            filters={filters}
            onFilterChange={handleFilterChange}
            onClear={handleClearFilters}
            onSearch={handleSearch}
          />

          {/* Action Bar */}
          <div className="flex justify-between items-center mb-6">
            <div className="text-sm text-gray-600">
              {selectedRowKeys.length > 0 && (
                <span>
                  {selectedRowKeys.length} test order(s) selected
                </span>
              )}
            </div>
            <div className="flex gap-3">
              <ExportExcelButton
                selectedRowKeys={selectedRowKeys}
                filters={filters}
                onExport={handleExport}
                loading={exportLoading}
              />
              <button
                onClick={handleCreateNew}
                className="flex items-center gap-2 px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
              >
                <Plus className="w-5 h-5" />
                Create New Order
              </button>
            </div>
          </div>

          {/* Table */}
          <TestOrderTable
            testOrders={testOrders}
            loading={loading}
            selectedRowKeys={selectedRowKeys}
            onSelectChange={handleSelectChange}
            onRowClick={handleRowClick}
          />

          {/* Pagination */}
          {!loading && testOrders.length > 0 && (
            <div className="mt-6 flex items-center justify-between">
              <div className="text-sm text-gray-700">
                Showing {(currentPage - 1) * pageSize + 1} to{' '}
                {Math.min(currentPage * pageSize, totalCount)} of {totalCount} results
              </div>
              <div className="flex items-center gap-2">
                <button
                  onClick={() => handlePageChange(currentPage - 1)}
                  disabled={currentPage === 1}
                  className="px-3 py-2 border rounded-lg disabled:opacity-50 disabled:cursor-not-allowed hover:bg-gray-50"
                >
                  Previous
                </button>
                <span className="px-3 py-2 text-sm text-gray-700">
                  Page {currentPage}
                </span>
                <button
                  onClick={() => handlePageChange(currentPage + 1)}
                  disabled={currentPage * pageSize >= totalCount}
                  className="px-3 py-2 border rounded-lg disabled:opacity-50 disabled:cursor-not-allowed hover:bg-gray-50"
                >
                  Next
                </button>
              </div>
            </div>
          )}
        </div>
      </div>
    </DashboardLayout>
  );
}
