import { Button } from 'antd';
import { FileSpreadsheet, Download } from 'lucide-react';

export default function ExportExcelButton({
  selectedRowKeys = [],
  filters = {},
  onExport,
  disabled = false,
  loading = false
}) {
  const hasSelection = selectedRowKeys && selectedRowKeys.length > 0;
  const hasFilters = filters && Object.keys(filters).some(key => {
    const value = filters[key];
    return value !== null && value !== undefined && value !== '';
  });

  // Button is enabled if:
  // 1. Has selected rows (AC02: export one or multiple)
  // 2. Has filters applied (AC01: filter before exporting)
  // 3. Or export all (current month) - for testing background job
  // Always enabled for testing background export functionality
  const isEnabled = !disabled; //  && (hasSelection || hasFilters);

  const handleClick = () => {
    if (!isEnabled || loading) return;

    const exportRequest = {
      testOrderIds: hasSelection ? selectedRowKeys : undefined,
      filters: hasFilters ? filters : undefined,
      // Default: export current month if no selection and no filters
      exportAll: !hasSelection && !hasFilters,
    };

    onExport && onExport(exportRequest);
  };

  return (
    <Button
      type="primary"
      icon={hasSelection ? <Download className="w-4 h-4" /> : <FileSpreadsheet className="w-4 h-4" />}
      onClick={handleClick}
      disabled={!isEnabled}
      loading={loading}
      size="large"
    >
      {hasSelection 
        ? `Export Selected (${selectedRowKeys.length})`
        : hasFilters
        ? 'Export Filtered'
        : 'Export Excel (Current Month)'
      }
    </Button>
  );
}

