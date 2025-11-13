import { Button } from 'antd';
import { Printer } from 'lucide-react';

/**
 * Print Test Results Button
 * AC02: Only print one test order at a time
 * AC03: Only show when status = "Completed"
 */
export default function PrintTestResultsButton({
  testOrder,
  onPrint,
  loading = false,
  disabled = false
}) {
  // AC03: Only show when status is "Completed"
  const isCompleted = testOrder?.status === 'Completed';
  
  if (!isCompleted) {
    return null; // Don't render if not completed
  }

  const handleClick = () => {
    if (disabled || loading || !testOrder?.testOrderId) return;
    onPrint && onPrint(testOrder.testOrderId);
  };

  return (
    <Button
      type="primary"
      icon={<Printer className="w-4 h-4" />}
      onClick={handleClick}
      loading={loading}
      disabled={disabled}
      size="large"
    >
      Print Test Results
    </Button>
  );
}

