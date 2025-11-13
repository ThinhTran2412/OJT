import api from "./api";
import backgroundJobManager from '../utils/BackgroundJobManager';

// CREATE Test Order - Simple payload
export const createTestOrder = async (data) => {
  try {
    const payload = {
      identifyNumber: data.identifyNumber || '',
      servicePackageId: data.servicePackageId,
      priority: data.priority || 'Normal',
      note: data.note || ''
    };
    
    console.log('Creating test order with payload:', payload);
    const response = await api.post('/TestOrder', payload);
    return response.data;
  } catch (error) {
    console.error('Error creating test order:', error);
    console.error('Error details:', error.response?.data);
    throw error;
  }
};

// UPDATE Test Order - Full patient info
export const updateTestOrder = async (testOrderId, data) => {
  try {
    const payload = {
      testOrderId: testOrderId,
      identifyNumber: data.identifyNumber || '',
      patientName: data.patientName || '',
      dateOfBirth: data.dateOfBirth || null,
      age: parseInt(data.age) || 0,
      gender: data.gender || '',
      address: data.address || '',
      phoneNumber: data.phoneNumber || '',
      email: data.email || '',
      priority: data.priority || 'Normal',
      status: data.status || 'Created',
      note: data.note || ''
    };
    
    console.log('Updating test order with payload:', payload);
    const response = await api.patch(`/TestOrder/${testOrderId}`, payload);
    return response.data;
  } catch (error) {
    console.error('Error updating test order:', error);
    console.error('Error details:', error.response?.data);
    throw error;
  }
};

// Get Test Order by ID
export const getTestOrderById = async (testOrderId) => {
  try {
    const response = await api.get(`/TestOrder/${testOrderId}`);
    return response.data;
  } catch (error) {
    console.error('Error fetching test order:', error);
    throw error;
  }
};

// Delete Test Order
export const deleteTestOrder = async (testOrderId) => {
  try {
    const response = await api.delete(`/TestOrder/${testOrderId}`);
    return response.data;
  } catch (error) {
    console.error('Error deleting test order:', error);
    throw error;
  }
};

// Get Service Packages (mock data - replace with real API if available)
export const getServicePackages = async () => {
  return [
    { id: '3fa85f64-5717-4562-b3fc-2c963f66afa6', name: 'Complete Blood Count (CBC)' },
    { id: '4fa85f64-5717-4562-b3fc-2c963f66afa7', name: 'Lipid Panel' },
    { id: '5fa85f64-5717-4562-b3fc-2c963f66afa8', name: 'Comprehensive Metabolic Panel' },
    { id: '6fa85f64-5717-4562-b3fc-2c963f66afa9', name: 'Thyroid Function Test' },
    { id: '7fa85f64-5717-4562-b3fc-2c963f66afaa', name: 'Liver Function Test' },
    { id: '8fa85f64-5717-4562-b3fc-2c963f66afab', name: 'Renal Function Test' }
  ];
};

// ============================================
// REPORT FUNCTIONS - Mock implementation
// ============================================

/**
 * Get all test orders with filters, sort, and pagination
 * MOCK: Returns empty array for now (backend not ready)
 */
export const getAllTestOrders = async (params = {}) => {
  try {
    const {
      page = 1,
      pageSize = 10,
      filters = {},
      sortBy,
      sortOrder = 'asc'
    } = params;

    // TODO: Uncomment when backend API is ready
    /*
    const queryParams = new URLSearchParams();
    queryParams.append('page', page);
    queryParams.append('pageSize', pageSize);
    
    if (filters.status) queryParams.append('status', filters.status);
    if (filters.priority) queryParams.append('priority', filters.priority);
    if (filters.dateFrom) queryParams.append('dateFrom', filters.dateFrom);
    if (filters.dateTo) queryParams.append('dateTo', filters.dateTo);
    if (filters.patientName) queryParams.append('patientName', filters.patientName);
    if (filters.orderCode) queryParams.append('orderCode', filters.orderCode);
    
    if (sortBy) {
      queryParams.append('sortBy', sortBy);
      queryParams.append('sortOrder', sortOrder);
    }

    const response = await api.get(`/TestOrder?${queryParams.toString()}`);
    return response.data;
    */

    // MOCK: Return empty data for now
    console.log('MOCK: getAllTestOrders called with params:', params);
    return {
      data: [],
      totalCount: 0,
      pageNumber: page,
      pageSize: pageSize,
      totalPages: 0,
      hasNextPage: false,
      hasPreviousPage: false
    };
  } catch (error) {
    console.error('Error fetching test orders:', error);
    throw error;
  }
};

/**
 * Export Test Orders to Excel
 * AC03: Non-blocking - returns jobId immediately
 * 
 * ============================================
 * BACKEND API INTEGRATION (When Entity Ready)
 * ============================================
 * 
 * API Endpoint: POST /api/v1/test-orders/report/export-excel
 * 
 * Request Body:
 * {
 *   testOrderIds?: string[],        // Optional: specific order IDs to export
 *   filters?: {                      // Optional: filter criteria
 *     status?: string,
 *     priority?: string,
 *     dateFrom?: string,
 *     dateTo?: string,
 *     patientName?: string,
 *     orderCode?: string
 *   },
 *   exportAll?: boolean             // Default: export current month if true
 * }
 * 
 * Response:
 * {
 *   jobId: string                   // Job ID for status tracking
 * }
 * 
 * ============================================
 * CURRENT IMPLEMENTATION (Mock - Front-end Only)
 * ============================================
 * This is a MOCK implementation for front-end development.
 * Replace with actual API call when backend entity is ready.
 */
export const exportTestOrdersToExcel = async (exportRequest) => {
  try {
    // ============================================
    // BACKEND API CALL (Uncomment when ready)
    // ============================================
    /*
    const response = await api.post('/api/v1/test-orders/report/export-excel', exportRequest);
    return response.data; // { jobId }
    */

    // ============================================
    // MOCK IMPLEMENTATION (Remove when API ready)
    // ============================================
    const jobId = `job_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`;
    
    // Store job info in localStorage for mock polling
    const jobInfo = {
      jobId,
      status: 'pending',
      progress: 0,
      createdAt: new Date().toISOString(),
      exportRequest
    };
    localStorage.setItem(`export_job_${jobId}`, JSON.stringify(jobInfo));
    
    return { jobId };
  } catch (error) {
    console.error('Error exporting test orders:', error);
    throw error;
  }
};

/**
 * Get Export Job Status
 * AC03: Polling endpoint for background job status
 * 
 * ============================================
 * BACKEND API INTEGRATION (When Entity Ready)
 * ============================================
 * 
 * API Endpoint: GET /api/v1/test-orders/report/export-status/{jobId}
 * 
 * Response:
 * {
 *   jobId: string,
 *   status: 'pending' | 'processing' | 'completed' | 'failed',
 *   progress: number,              // 0-100
 *   downloadUrl?: string,          // Available when status = 'completed'
 *   errorMessage?: string,          // Available when status = 'failed'
 *   createdAt: string,
 *   updatedAt: string
 * }
 * 
 * ============================================
 * CURRENT IMPLEMENTATION (Mock - Front-end Only)
 * ============================================
 * This is a MOCK implementation for front-end development.
 * Replace with actual API call when backend entity is ready.
 */
export const getExportJobStatus = async (jobId) => {
  try {
    // ============================================
    // BACKEND API CALL (Uncomment when ready)
    // ============================================
    /*
    const response = await api.get(`/api/v1/test-orders/report/export-status/${jobId}`);
    return response.data;
    */

    // ============================================
    // MOCK IMPLEMENTATION (Remove when API ready)
    // ============================================
    const storedJob = localStorage.getItem(`export_job_${jobId}`);
    if (!storedJob) {
      return { status: 'failed', errorMessage: 'Job not found' };
    }

    const jobInfo = JSON.parse(storedJob);
    const now = Date.now();
    const createdAt = new Date(jobInfo.createdAt).getTime();
    const elapsed = now - createdAt;

    // Simulate progress: 0-100% over 5 seconds
    if (elapsed < 5000) {
      const progress = Math.min(100, Math.floor((elapsed / 5000) * 100));
      jobInfo.progress = progress;
      jobInfo.status = progress < 100 ? 'processing' : 'completed';
      
      if (jobInfo.status === 'completed') {
        jobInfo.downloadUrl = `mock://download/${jobId}`;
      }
      
      localStorage.setItem(`export_job_${jobId}`, JSON.stringify(jobInfo));
    } else if (jobInfo.status !== 'completed') {
      jobInfo.status = 'completed';
      jobInfo.progress = 100;
      jobInfo.downloadUrl = `mock://download/${jobId}`;
      localStorage.setItem(`export_job_${jobId}`, JSON.stringify(jobInfo));
    }

    // Update job in background manager
    backgroundJobManager.updateJob(jobId, {
      status: jobInfo.status,
      progress: jobInfo.progress,
      downloadUrl: jobInfo.downloadUrl,
    });

    return {
      status: jobInfo.status,
      progress: jobInfo.progress,
      downloadUrl: jobInfo.downloadUrl,
      errorMessage: jobInfo.errorMessage
    };
  } catch (error) {
    console.error('Error getting export status:', error);
    throw error;
  }
};

/**
 * Download Exported Excel File
 * AC03: Download completed export file
 * 
 * ============================================
 * BACKEND API INTEGRATION (When Entity Ready)
 * ============================================
 * 
 * API Endpoint: GET /api/v1/test-orders/report/download/{jobId}
 * Response Type: blob (application/vnd.openxmlformats-officedocument.spreadsheetml.sheet)
 * 
 * Headers:
 * - Content-Type: application/vnd.openxmlformats-officedocument.spreadsheetml.sheet
 * - Content-Disposition: attachment; filename="Test Orders-{Patient Name}-{Export Date}.xlsx"
 * 
 * File Format: Excel (.xlsx)
 * Required Columns:
 * - Id Test Orders
 * - Patient Name
 * - Gender
 * - Date of Birth
 * - Phone Number
 * - Status
 * - Created By
 * - Created On Date
 * - Run By (empty if status != "Completed")
 * - Run On (empty if status != "Completed")
 * 
 * ============================================
 * CURRENT IMPLEMENTATION (Mock - Front-end Only)
 * ============================================
 * This is a MOCK implementation for front-end development.
 * Replace with actual API call when backend entity is ready.
 */
export const downloadExportedFile = async (jobId) => {
  try {
    // ============================================
    // BACKEND API CALL (Uncomment when ready)
    // ============================================
    /*
    const response = await api.get(`/api/v1/test-orders/report/download/${jobId}`, {
      responseType: 'blob',
    });
    return response.data;
    */

    // ============================================
    // MOCK IMPLEMENTATION (Remove when API ready)
    // ============================================
    const csvContent = 'Test Order ID,Patient Name,Gender,Date of Birth,Phone Number,Status,Created By,Created At,Run By,Run On\n';
    const blob = new Blob([csvContent], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
    
    return blob;
  } catch (error) {
    console.error('Error downloading file:', error);
    throw error;
  }
};

/**
 * Print Test Results PDF
 * AC02: Only one test order at a time
 * AC03: Only when status = "Completed"
 * 
 * ============================================
 * BACKEND API INTEGRATION (When Entity Ready)
 * ============================================
 * 
 * API Endpoint: GET /api/v1/test-orders/report/print-results/{orderId}
 * Response Type: blob (application/pdf)
 * 
 * Validation:
 * - Order must exist and not be deleted
 * - Order status must be "Completed" (AC03)
 * 
 * Response:
 * - PDF file with two tables:
 *   1. Order Details: Id, Patient Name, Gender, DOB, Phone, Status, Created By, Created On, Run By, Run On
 *   2. Test Results: Specific test results and related comments
 * 
 * File Naming: "Details-{Patient Name}-{Print Date}.pdf"
 * 
 * ============================================
 * CURRENT IMPLEMENTATION (Mock - Front-end Only)
 * ============================================
 * This is a MOCK implementation for front-end development.
 * Replace with actual API call when backend entity is ready.
 */
export const printTestResultsPDF = async (orderId) => {
  try {
    // ============================================
    // BACKEND API CALL (Uncomment when ready)
    // ============================================
    /*
    const response = await api.get(`/api/v1/test-orders/report/print-results/${orderId}`, {
      responseType: 'blob',
    });
    return response.data;
    */

    // ============================================
    // MOCK IMPLEMENTATION (Remove when API ready)
    // ============================================
    const pdfContent = `Test Order Results\nOrder ID: ${orderId}\n\n[PDF content will be generated by backend]`;
    const blob = new Blob([pdfContent], { type: 'application/pdf' });
    
    return blob;
  } catch (error) {
    console.error('Error printing test results:', error);
    throw error;
  }
};
