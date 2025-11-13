import { Modal, Progress, Button, Space, Tag } from 'antd';
import { CheckCircle, XCircle, X, Download, FileSpreadsheet } from 'lucide-react';
import { useEffect, useState } from 'react';
import backgroundJobManager from '../../utils/BackgroundJobManager';
import { getExportJobStatus, downloadExportedFile } from '../../services/TestOrderService';
import { downloadFile, generateExportFileName } from '../../utils/fileUtils';

/**
 * Export Progress Modal
 * AC03: Non-blocking - User can close modal and continue working
 * Shows progress for background export jobs
 */
export default function ExportProgressModal({
  visible,
  jobId,
  onClose,
  onComplete
}) {
  const [jobInfo, setJobInfo] = useState(null);

  useEffect(() => {
    if (!visible || !jobId) return;

    // Subscribe to job updates
    const unsubscribe = backgroundJobManager.subscribe(jobId, (updatedJob) => {
      setJobInfo(updatedJob);
    });

    // Get initial job info
    const initialJob = backgroundJobManager.getJob(jobId);
    if (initialJob) {
      setJobInfo(initialJob);
    }

    // Start polling if job is active
    if (initialJob && (initialJob.status === 'pending' || initialJob.status === 'processing')) {
      backgroundJobManager.startPolling(jobId, getExportJobStatus, 2000);
    }

    return () => {
      unsubscribe();
    };
  }, [visible, jobId]);

  const handleDownload = async () => {
    if (!jobId || !jobInfo?.downloadUrl) return;

    try {
      const blob = await downloadExportedFile(jobId);
      const filename = generateExportFileName();
      downloadFile(blob, filename);
      
      onComplete && onComplete();
      onClose && onClose();
    } catch (error) {
      console.error('Error downloading file:', error);
    }
  };

  const getStatusIcon = () => {
    if (!jobInfo) return null;
    
    switch (jobInfo.status) {
      case 'completed':
        return <CheckCircle className="w-5 h-5 text-green-600" />;
      case 'failed':
        return <XCircle className="w-5 h-5 text-red-600" />;
      default:
        return null;
    }
  };

  const getStatusText = () => {
    if (!jobInfo) return 'Initializing...';
    
    switch (jobInfo.status) {
      case 'pending':
        return 'Đang khởi tạo export job...';
      case 'processing':
        return `Đang tạo file Excel... (${jobInfo.progress || 0}%)`;
      case 'completed':
        return 'Hoàn thành! File đã sẵn sàng để tải xuống.';
      case 'failed':
        return `Có lỗi xảy ra: ${jobInfo.errorMessage || 'Unknown error'}`;
      default:
        return 'Đang xử lý...';
    }
  };

  const canDownload = jobInfo?.status === 'completed' && jobInfo?.downloadUrl;
  const isProcessing = jobInfo?.status === 'pending' || jobInfo?.status === 'processing';

  return (
    <Modal
      title={
        <div className="flex items-center gap-2">
          <FileSpreadsheet className="w-5 h-5 text-blue-600" />
          <span>Export Excel Progress</span>
          {jobInfo && (
            <Tag color={jobInfo.status === 'completed' ? 'green' : jobInfo.status === 'failed' ? 'red' : 'blue'}>
              {jobInfo.status}
            </Tag>
          )}
        </div>
      }
      open={visible}
      onCancel={onClose}
      footer={
        <Space>
          {/* AC03: User can close modal and continue working */}
          <Button onClick={onClose} icon={<X className="w-4 h-4" />}>
            {isProcessing ? 'Close (Continue Working)' : 'Close'}
          </Button>
          {canDownload && (
            <Button
              type="primary"
              onClick={handleDownload}
              icon={<Download className="w-4 h-4" />}
            >
              Download File
            </Button>
          )}
        </Space>
      }
      width={500}
      closable={true}
      maskClosable={false}
    >
      <div className="py-4">
        {/* Progress Bar */}
        <div className="mb-4">
          <Progress
            percent={jobInfo?.progress || 0}
            status={
              jobInfo?.status === 'failed'
                ? 'exception'
                : jobInfo?.status === 'completed'
                ? 'success'
                : 'active'
            }
            strokeColor={
              jobInfo?.status === 'completed'
                ? '#52c41a'
                : jobInfo?.status === 'failed'
                ? '#ff4d4f'
                : '#1890ff'
            }
          />
        </div>

        {/* Status Message */}
        <div className="flex items-center gap-2 mb-4">
          {getStatusIcon()}
          <p className="text-gray-700">{getStatusText()}</p>
        </div>

        {/* Job Info */}
        {jobInfo && (
          <div className="text-sm text-gray-500 space-y-1">
            <p>Job ID: <span className="font-mono text-xs">{jobId}</span></p>
            {jobInfo.createdAt && (
              <p>Started: {new Date(jobInfo.createdAt).toLocaleString()}</p>
            )}
          </div>
        )}

        {/* AC03: Non-blocking notice */}
        {isProcessing && (
          <div className="mt-4 p-3 bg-blue-50 border border-blue-200 rounded-lg">
            <p className="text-sm text-blue-800">
              <strong>Note:</strong> Bạn có thể đóng cửa sổ này và tiếp tục làm việc. 
              Hệ thống sẽ thông báo khi file đã sẵn sàng.
            </p>
          </div>
        )}
      </div>
    </Modal>
  );
}

