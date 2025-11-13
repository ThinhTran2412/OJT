import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { ArrowLeft } from 'lucide-react';
import TestOrderModal from '../../components/modals/TestOrderModal';

export default function CreateTestOrderPage() {
  const navigate = useNavigate();

  const handleSubmit = async (formData) => {
    try {
      // TODO: Implement test order creation API call
      // This will be implemented when the API is ready
      navigate('/test-orders');
    } catch (error) {
      console.error('Error creating test order:', error);
    }
  };

  return (
    <div className="p-6">
      <div className="max-w-4xl mx-auto">
        <button
          onClick={() => navigate('/test-orders')}
          className="flex items-center gap-2 text-gray-600 hover:text-gray-900 mb-6"
        >
          <ArrowLeft className="w-5 h-5" />
          Back to Test Orders
        </button>

        <div className="mb-6">
          <h1 className="text-2xl font-semibold text-gray-900">Create New Test Order</h1>
          <p className="mt-2 text-sm text-gray-700">
            Create a new laboratory test order for a patient
          </p>
        </div>

        <div className="bg-white rounded-lg shadow">
          <TestOrderModal
            isOpen={true}
            mode="create"
            onClose={() => navigate('/test-orders')}
            onSubmit={handleSubmit}
          />
        </div>
      </div>
    </div>
  );
}