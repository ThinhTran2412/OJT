import { Table, Tag, Button } from 'antd';
import { Eye } from 'lucide-react';
import { useNavigate } from 'react-router-dom';

export default function TestOrderTable({
  testOrders = [],
  loading = false,
  selectedRowKeys = [],
  onSelectChange,
  onRowClick
}) {
  const navigate = useNavigate();

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

  const getStatusColor = (status) => {
    switch (status) {
      case 'Completed':
        return 'green';
      case 'InProgress':
      case 'In Progress':
        return 'blue';
      case 'Created':
        return 'default';
      case 'Cancelled':
        return 'red';
      default:
        return 'default';
    }
  };

  const columns = [
    {
      title: 'Test Order ID',
      dataIndex: 'testOrderId',
      key: 'testOrderId',
      width: 200,
      ellipsis: true,
      render: (text) => <span className="font-mono text-xs">{text || 'N/A'}</span>,
    },
    {
      title: 'Order Code',
      dataIndex: 'orderCode',
      key: 'orderCode',
      width: 150,
      ellipsis: true,
    },
    {
      title: 'Patient Name',
      dataIndex: 'patientName',
      key: 'patientName',
      width: 200,
      ellipsis: true,
    },
    {
      title: 'Gender',
      dataIndex: 'gender',
      key: 'gender',
      width: 100,
    },
    {
      title: 'Date of Birth',
      dataIndex: 'dateOfBirth',
      key: 'dateOfBirth',
      width: 120,
      render: formatDate,
    },
    {
      title: 'Phone Number',
      dataIndex: 'phoneNumber',
      key: 'phoneNumber',
      width: 140,
      ellipsis: true,
    },
    {
      title: 'Status',
      dataIndex: 'status',
      key: 'status',
      width: 120,
      render: (status) => (
        <Tag color={getStatusColor(status)}>
          {status || 'N/A'}
        </Tag>
      ),
    },
    {
      title: 'Created By',
      dataIndex: 'createdBy',
      key: 'createdBy',
      width: 120,
      ellipsis: true,
    },
    {
      title: 'Created At',
      dataIndex: 'createdAt',
      key: 'createdAt',
      width: 160,
      render: formatDateTime,
    },
    {
      title: 'Run By',
      dataIndex: 'runBy',
      key: 'runBy',
      width: 120,
      render: (text, record) => {
        // AC03: Only show if status is "Completed"
        return record.status === 'Completed' ? (text || 'N/A') : '-';
      },
    },
    {
      title: 'Run On',
      dataIndex: 'runDate',
      key: 'runDate',
      width: 160,
      render: (text, record) => {
        // AC03: Only show if status is "Completed"
        return record.status === 'Completed' ? formatDateTime(text) : '-';
      },
    },
    {
      title: 'Actions',
      key: 'actions',
      width: 120,
      fixed: 'right',
      render: (_text, record) => (
        <Button
          type="link"
          size="small"
          icon={<Eye className="w-4 h-4" />}
          onClick={() => {
            if (onRowClick) {
              onRowClick(record);
            } else {
              navigate(`/test-orders/${record.testOrderId}`);
            }
          }}
        >
          View
        </Button>
      ),
    },
  ];

  // AC02: Row selection for export (one or multiple)
  const rowSelection = {
    selectedRowKeys,
    onChange: onSelectChange,
    getCheckboxProps: (record) => ({
      disabled: false,
      name: record.testOrderId,
    }),
  };

  return (
    <div className="bg-white rounded-lg shadow-sm overflow-hidden">
      <Table
        columns={columns}
        dataSource={testOrders}
        loading={loading}
        rowSelection={rowSelection}
        rowKey={(record) => record.testOrderId || record.id || Math.random()}
        pagination={false}
        size="middle"
        scroll={{ x: 1500 }}
        locale={{ emptyText: 'No test orders found' }}
        onRow={(record) => ({
          onClick: () => {
            if (onRowClick) {
              onRowClick(record);
            } else {
              navigate(`/test-orders/${record.testOrderId}`);
            }
          },
          style: { cursor: 'pointer' },
        })}
      />
    </div>
  );
}

