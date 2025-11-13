import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { ChevronLeft, AlertCircle } from 'lucide-react';
import { message } from 'antd';
import DashboardLayout from '../../layouts/DashboardLayout';
import PrintTestResultsButton from '../../components/TestOrder_Management/PrintTestResultsButton';
import { getTestOrderById, printTestResultsPDF } from '../../services/TestOrderService';
import { openFileInNewTab, generatePrintFileName } from '../../utils/fileUtils';

export default function TestOrderDetailPage() {
  const { orderId } = useParams();
  const navigate = useNavigate();
  
  const [testOrder, setTestOrder] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [printing, setPrinting] = useState(false);

  useEffect(() => {
    if (orderId) {
      fetchTestOrderDetail();
    }
  }, [orderId]);

  const fetchTestOrderDetail = async () => {
    try {
      setLoading(true);
      setError(null);
      
      // TODO: Replace with real API call when backend is ready
      // For now, use mock data
      const response = await getTestOrderById(orderId);
      setTestOrder(response);
    } catch (err) {
      console.error('Error fetching test order detail:', err);
      setError('Failed to load test order details. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  // AC02: Print one test order at a time
  // AC03: Only when status = "Completed"
  const handlePrint = async (orderId) => {
    if (!testOrder || testOrder.status !== 'Completed') {
      message.warning('Only completed test orders can be printed.');
      return;
    }

    try {
      setPrinting(true);
      
      const blob = await printTestResultsPDF(orderId);
      const filename = generatePrintFileName(testOrder.patientName);
      
      // Open PDF in new tab
      openFileInNewTab(blob, 'application/pdf');
      
      message.success('PDF opened in new tab. You can print it from there.');
    } catch (err) {
      console.error('Error printing test results:', err);
      message.error('Failed to generate PDF. Please try again.');
    } finally {
      setPrinting(false);
    }
  };

  const formatDate = (dateString) => {
    if (!dateString) return 'N/A';
    try {
      const date = new Date(dateString);
      return date.toLocaleDateString('en-GB', {
        day: '2-digit',
        month: '2-digit',
        year: 'numeric'
      });
    } catch {
      return 'N/A';
    }
  };

  const formatDateTime = (dateString) => {
    if (!dateString) return 'N/A';
    try {
      const date = new Date(dateString);
      return date.toLocaleString('en-GB', {
        day: '2-digit',
        month: '2-digit',
        year: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
      });
    } catch {
      return 'N/A';
    }
  };

  if (loading) {
    return (
      <DashboardLayout>
        <div className="min-h-screen bg-gray-50 flex items-center justify-center">
          <div className="text-center">
            <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto mb-4"></div>
            <p className="text-gray-600">Loading test order details...</p>
          </div>
        </div>
      </DashboardLayout>
    );
  }

  if (error || !testOrder) {
    return (
      <DashboardLayout>
        <div className="min-h-screen bg-gray-50 flex items-center justify-center">
          <div className="text-center">
            <AlertCircle className="w-12 h-12 text-red-600 mx-auto mb-4" />
            <p className="text-red-600 mb-4">{error || 'Test order not found'}</p>
            <button
              onClick={() => navigate('/test-orders')}
              className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700"
            >
              Back to Test Orders
            </button>
          </div>
        </div>
      </DashboardLayout>
    );
  }

  return (
    <DashboardLayout>
      <div className="min-h-screen bg-gray-50">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
          {/* Header */}
          <div className="mb-6">
            <button
              onClick={() => navigate('/test-orders')}
              className="flex items-center gap-2 text-gray-600 hover:text-gray-900 mb-4"
            >
              <ChevronLeft className="w-5 h-5" />
              <span>Back to Test Orders</span>
            </button>
            <div className="flex justify-between items-start">
              <div>
                <h1 className="text-3xl font-bold text-gray-900 mb-2">Test Order Details</h1>
                <p className="text-gray-600">View and print test order results</p>
              </div>
              {/* AC03: Print button only when status = "Completed" */}
              <PrintTestResultsButton
                testOrder={testOrder}
                onPrint={handlePrint}
                loading={printing}
              />
            </div>
          </div>

          {/* Table 1: Test Order Information */}
          <div className="bg-white rounded-lg shadow-sm mb-6 overflow-hidden">
            <div className="px-6 py-4 border-b border-gray-200">
              <h2 className="text-lg font-semibold text-gray-900">Test Order Information</h2>
            </div>
            <div className="px-6 py-4">
              <table className="w-full">
                <tbody className="divide-y divide-gray-200">
                  <tr>
                    <td className="py-3 px-4 font-medium text-gray-700 w-1/3">Test Order ID</td>
                    <td className="py-3 px-4 text-gray-900 font-mono text-sm">{testOrder.testOrderId || 'N/A'}</td>
                  </tr>
                  <tr>
                    <td className="py-3 px-4 font-medium text-gray-700">Order Code</td>
                    <td className="py-3 px-4 text-gray-900">{testOrder.orderCode || 'N/A'}</td>
                  </tr>
                  <tr>
                    <td className="py-3 px-4 font-medium text-gray-700">Patient Name</td>
                    <td className="py-3 px-4 text-gray-900">{testOrder.patientName || 'N/A'}</td>
                  </tr>
                  <tr>
                    <td className="py-3 px-4 font-medium text-gray-700">Gender</td>
                    <td className="py-3 px-4 text-gray-900">{testOrder.gender || 'N/A'}</td>
                  </tr>
                  <tr>
                    <td className="py-3 px-4 font-medium text-gray-700">Date of Birth</td>
                    <td className="py-3 px-4 text-gray-900">{formatDate(testOrder.dateOfBirth)}</td>
                  </tr>
                  <tr>
                    <td className="py-3 px-4 font-medium text-gray-700">Phone Number</td>
                    <td className="py-3 px-4 text-gray-900">{testOrder.phoneNumber || 'N/A'}</td>
                  </tr>
                  <tr>
                    <td className="py-3 px-4 font-medium text-gray-700">Status</td>
                    <td className="py-3 px-4">
                      <span className={`px-2 py-1 text-xs rounded-full ${
                        testOrder.status === 'Completed' ? 'bg-green-100 text-green-800' :
                        testOrder.status === 'InProgress' ? 'bg-blue-100 text-blue-800' :
                        testOrder.status === 'Created' ? 'bg-gray-100 text-gray-800' :
                        'bg-red-100 text-red-800'
                      }`}>
                        {testOrder.status || 'N/A'}
                      </span>
                    </td>
                  </tr>
                  <tr>
                    <td className="py-3 px-4 font-medium text-gray-700">Created By</td>
                    <td className="py-3 px-4 text-gray-900">{testOrder.createdBy || 'N/A'}</td>
                  </tr>
                  <tr>
                    <td className="py-3 px-4 font-medium text-gray-700">Created At</td>
                    <td className="py-3 px-4 text-gray-900">{formatDateTime(testOrder.createdAt)}</td>
                  </tr>
                  {testOrder.status === 'Completed' && (
                    <>
                      <tr>
                        <td className="py-3 px-4 font-medium text-gray-700">Run By</td>
                        <td className="py-3 px-4 text-gray-900">{testOrder.runBy || 'N/A'}</td>
                      </tr>
                      <tr>
                        <td className="py-3 px-4 font-medium text-gray-700">Run On</td>
                        <td className="py-3 px-4 text-gray-900">{formatDateTime(testOrder.runDate)}</td>
                      </tr>
                    </>
                  )}
                </tbody>
              </table>
            </div>
          </div>

          {/* Table 2: Test Results */}
          <div className="bg-white rounded-lg shadow-sm overflow-hidden">
            <div className="px-6 py-4 border-b border-gray-200">
              <h2 className="text-lg font-semibold text-gray-900">Test Results</h2>
            </div>
            <div className="px-6 py-4">
              <table className="w-full">
                <tbody className="divide-y divide-gray-200">
                  <tr>
                    <td className="py-3 px-4 font-medium text-gray-700 w-1/3">Test Type</td>
                    <td className="py-3 px-4 text-gray-900">{testOrder.testType || 'N/A'}</td>
                  </tr>
                  <tr>
                    <td className="py-3 px-4 font-medium text-gray-700">Test Results</td>
                    <td className="py-3 px-4 text-gray-900 whitespace-pre-wrap">{testOrder.testResults || 'N/A'}</td>
                  </tr>
                  {testOrder.note && (
                    <tr>
                      <td className="py-3 px-4 font-medium text-gray-700">Comments</td>
                      <td className="py-3 px-4 text-gray-900 whitespace-pre-wrap">{testOrder.note}</td>
                    </tr>
                  )}
                </tbody>
              </table>
            </div>
          </div>
        </div>
      </div>
    </DashboardLayout>
  );
}

