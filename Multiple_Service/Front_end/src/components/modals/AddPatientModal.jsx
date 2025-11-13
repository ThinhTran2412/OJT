import { useState } from 'react';
import { X, Plus, AlertCircle } from 'lucide-react';

export default function AddPatientModal({ isOpen, onClose, onSuccess }) {
  const [identifyNumber, setIdentifyNumber] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const handleSubmit = async () => {
    if (!identifyNumber.trim()) {
      setError('Identify Number is required');
      return;
    }

    setLoading(true);
    setError('');

    try {
      const result = await onSuccess({ identifyNumber: identifyNumber.trim() });
      
      // Only reset and close if successful
      if (result) {
        setIdentifyNumber('');
        onClose();
      }
    } catch (err) {
      console.error('Error adding patient:', err);
      
      // Better error handling
      const errorMessage = err.response?.data?.message || 
                          err.response?.data?.title ||
                          err.message || 
                          'Failed to add patient. Please try again.';
      
      setError(errorMessage);
      // Don't close modal on error - let user retry
    } finally {
      setLoading(false);
    }
  };

  const handleClose = () => {
    setIdentifyNumber('');
    setError('');
    onClose();
  };

  const handleKeyPress = (e) => {
    if (e.key === 'Enter' && identifyNumber.trim() && !loading) {
      handleSubmit();
    }
  };

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 z-50 overflow-y-auto">
      <div className="flex min-h-screen items-center justify-center p-4">
        <div 
          className="fixed inset-0 bg-black bg-opacity-50 transition-opacity"
          onClick={handleClose}
        />
        
        <div className="relative bg-white rounded-lg shadow-xl max-w-md w-full">
          <div className="flex items-center justify-between p-6 border-b border-gray-200">
            <div className="flex items-center gap-3">
              <div className="w-10 h-10 bg-blue-100 rounded-full flex items-center justify-center">
                <Plus className="w-5 h-5 text-blue-600" />
              </div>
              <div>
                <h2 className="text-xl font-semibold text-gray-900">Add New Patient</h2>
                <p className="text-sm text-gray-500">Enter patient's identification number</p>
              </div>
            </div>
            <button
              onClick={handleClose}
              className="text-gray-400 hover:text-gray-600 transition-colors"
              disabled={loading}
            >
              <X className="w-6 h-6" />
            </button>
          </div>

          <div className="p-6">
            {error && (
              <div className="mb-4 bg-red-50 border border-red-200 rounded-lg p-4">
                <div className="flex items-start gap-2">
                  <AlertCircle className="w-5 h-5 text-red-600 mt-0.5 flex-shrink-0" />
                  <p className="text-sm text-red-700">{error}</p>
                </div>
              </div>
            )}

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Identify Number <span className="text-red-500">*</span>
              </label>
              <input
                type="text"
                value={identifyNumber}
                onChange={(e) => {
                  setIdentifyNumber(e.target.value);
                  setError('');
                }}
                onKeyPress={handleKeyPress}
                placeholder="e.g., 123456789003"
                className={`w-full px-4 py-3 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors ${
                  error ? 'border-red-500' : 'border-gray-300'
                }`}
                disabled={loading}
                autoFocus
              />
              <p className="text-xs text-gray-500 mt-2">
                Enter the patient's national identification number or passport number
              </p>
            </div>
          </div>

          <div className="bg-gray-50 px-6 py-4 border-t border-gray-200 flex items-center justify-end gap-3">
            <button
              type="button"
              onClick={handleClose}
              className="px-4 py-2 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition-colors"
              disabled={loading}
            >
              Cancel
            </button>
            <button
              type="button"
              onClick={handleSubmit}
              className="flex items-center gap-2 px-6 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
              disabled={loading || !identifyNumber.trim()}
            >
              <Plus className="w-4 h-4" />
              {loading ? 'Adding...' : 'Add Patient'}
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}