import { Select, DatePicker, Input, Button, Space } from 'antd';
import { Search, X } from 'lucide-react';
import { useState } from 'react';

const { Option } = Select;
const { RangePicker } = DatePicker;

export default function TestOrderFilters({
  filters = {},
  onFilterChange,
  onClear,
  onSearch
}) {
  const [localFilters, setLocalFilters] = useState({
    status: filters.status || '',
    priority: filters.priority || '',
    dateRange: filters.dateRange || null,
    patientName: filters.patientName || '',
    orderCode: filters.orderCode || '',
    ...filters
  });

  const handleFilterChange = (key, value) => {
    const newFilters = { ...localFilters, [key]: value };
    setLocalFilters(newFilters);
  };

  const handleApply = () => {
    const appliedFilters = { ...localFilters };
    
    // Format date range if exists (Ant Design DatePicker returns dayjs objects)
    if (localFilters.dateRange && localFilters.dateRange.length === 2) {
      const [startDate, endDate] = localFilters.dateRange;
      // dayjs format method (Ant Design 5 uses dayjs by default)
      if (startDate && typeof startDate.format === 'function') {
        appliedFilters.dateFrom = startDate.format('YYYY-MM-DD');
      }
      if (endDate && typeof endDate.format === 'function') {
        appliedFilters.dateTo = endDate.format('YYYY-MM-DD');
      }
    }
    
    delete appliedFilters.dateRange;
    onFilterChange && onFilterChange(appliedFilters);
    onSearch && onSearch();
  };

  const handleClear = () => {
    const clearedFilters = {
      status: '',
      priority: '',
      dateRange: null,
      patientName: '',
      orderCode: '',
    };
    setLocalFilters(clearedFilters);
    onClear && onClear();
  };

  return (
    <div className="mb-6 p-4 bg-white rounded-lg shadow">
      <div className="flex flex-col md:flex-row md:items-center gap-4 w-full">
        {/* Status Filter */}
        <Select
          value={localFilters.status}
          onChange={(v) => handleFilterChange('status', v)}
          placeholder="Status"
          allowClear
          style={{ width: 150, height: 40 }}
          size="large"
        >
          <Option value="Created">Created</Option>
          <Option value="InProgress">In Progress</Option>
          <Option value="Completed">Completed</Option>
          <Option value="Cancelled">Cancelled</Option>
        </Select>

        {/* Priority Filter */}
        <Select
          value={localFilters.priority}
          onChange={(v) => handleFilterChange('priority', v)}
          placeholder="Priority"
          allowClear
          style={{ width: 150, height: 40 }}
          size="large"
        >
          <Option value="Normal">Normal</Option>
          <Option value="Urgent">Urgent</Option>
          <Option value="High">High</Option>
          <Option value="Low">Low</Option>
        </Select>

        {/* Date Range */}
        <RangePicker
          value={localFilters.dateRange}
          onChange={(dates) => handleFilterChange('dateRange', dates)}
          format="DD/MM/YYYY"
          style={{ height: 40, width: 300 }}
          size="large"
        />

        {/* Patient Name Search */}
        <Input
          value={localFilters.patientName}
          onChange={(e) => handleFilterChange('patientName', e.target.value)}
          placeholder="Patient Name"
          prefix={<Search className="w-4 h-4 text-gray-400" />}
          style={{ height: 40 }}
          size="large"
          allowClear
        />

        {/* Order Code Search */}
        <Input
          value={localFilters.orderCode}
          onChange={(e) => handleFilterChange('orderCode', e.target.value)}
          placeholder="Order Code"
          prefix={<Search className="w-4 h-4 text-gray-400" />}
          style={{ height: 40 }}
          size="large"
          allowClear
        />

        {/* Action Buttons */}
        <Space>
          <Button
            type="primary"
            onClick={handleApply}
            icon={<Search className="w-4 h-4" />}
            size="large"
            style={{ height: 40 }}
          >
            Apply Filters
          </Button>
          <Button
            onClick={handleClear}
            icon={<X className="w-4 h-4" />}
            size="large"
            style={{ height: 40 }}
          >
            Clear
          </Button>
        </Space>
      </div>
    </div>
  );
}

