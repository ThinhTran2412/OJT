import api from '../services/api';

export const getAllPatients = async (params = {}) => {
  try {
    const accessToken = localStorage.getItem('accessToken');
    console.log('Access Token:', accessToken ? 'Present' : 'Missing');
    
    const response = await api.get('/Patient/all', {
      headers: {
        'Accept': 'application/json',
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${accessToken}`
      }
    });
    
    console.log('Raw API Response:', response.data);
    
    const patients = Array.isArray(response.data) ? response.data.map(patient => ({
      patientId: patient.patientId,
      identifyNumber: patient.identifyNumber,
      fullName: patient.fullName,
      dateOfBirth: patient.dateOfBirth !== "0001-01-01" ? patient.dateOfBirth : null,
      gender: patient.gender || "Not specified",
      phoneNumber: patient.phoneNumber || "Not specified",
      email: patient.email || "Not specified",
      address: patient.address || "Not specified",
      age: patient.age,
      lastTestDate: patient.lastTestDate !== "0001-01-01T00:00:00" ? patient.lastTestDate : null,
      createdAt: patient.createdAt,
      createdBy: patient.createdBy || "System",
      isDeleted: patient.isDeleted
    })) : [];

    console.log('Formatted Patients:', patients);

    return {
      patients: patients,
      totalCount: patients.length,
      pageNumber: 1,
      pageSize: patients.length,
      totalPages: 1,
      hasNextPage: false,
      hasPreviousPage: false
    };
  } catch (error) {
    console.error('Error fetching patients:', error.response || error);
    throw error;
  }
};

export const getPatientById = async (patientId) => {
  try {
    const response = await api.get(`/Patient/${patientId}`);
    return response.data;
  } catch (error) {
    console.error('Error fetching patient details:', error);
    throw error;
  }
};

// NEW: Create patient by identity number only
export const createPatientByIdentity = async (identifyNumber) => {
  try {
    const payload = { identifyNumber };
    console.log('Creating patient with identify number:', payload);
    
    const response = await api.post('/Patient/create/by-identity', payload);
    console.log('Patient created successfully:', response.data);
    
    return response.data;
  } catch (error) {
    console.error('Error creating patient by identity:', error);
    console.error('Error details:', error.response?.data);
    throw error;
  }
};

// Original create patient (full data)
export const createPatient = async (patientData) => {
  try {
    const response = await api.post('/Patient', patientData);
    return response.data;
  } catch (error) {
    console.error('Error creating patient:', error);
    throw error;
  }
};

export const updatePatient = async (patientId, patientData) => {
  try {
    const response = await api.put(`/Patient/${patientId}`, patientData);
    return response.data;
  } catch (error) {
    console.error('Error updating patient:', error);
    throw error;
  }
};

export const deletePatient = async (patientId) => {
  try {
    const response = await api.delete(`/Patient/${patientId}`);
    return response.data;
  } catch (error) {
    console.error('Error deleting patient:', error);
    throw error;
  }
};

export const searchPatients = async (searchTerm) => {
  try {
    const response = await api.get(`/Patient/search?query=${encodeURIComponent(searchTerm)}`);
    return response.data;
  } catch (error) {
    console.error('Error searching patients:', error);
    throw error;
  }
};

export const getPatientMedicalRecords = async (patientId) => {
  try {
    const response = await api.get('/PatientInfo/my-medical-records', {
      params: { patientId }
    });
    return response.data;
  } catch (error) {
    console.error('Error fetching patient medical records:', error);
    throw error;
  }
};

const PatientService = {
  getAllPatients,
  getPatientById,
  createPatient,
  createPatientByIdentity, // NEW
  updatePatient,
  deletePatient,
  searchPatients,
  getPatientMedicalRecords,
};

export default PatientService;