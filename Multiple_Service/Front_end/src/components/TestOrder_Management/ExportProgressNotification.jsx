import { Progress, Button, Tag } from 'antd';
import { CheckCircle, XCircle, X, Download, FileSpreadsheet } from 'lucide-react';
import { useEffect, useState } from 'react';
import backgroundJobManager from '../../utils/BackgroundJobManager';
import { getExportJobStatus, downloadExportedFile } from '../../services/TestOrderService';
import { downloadFile, generateExportFileName } from '../../utils/fileUtils';

/**
 * Export Progress Notification
 * AC03: Small popup at bottom-right corner
 * Shows file name and progress bar
 * Auto-downloads when complete
 * Persists across page navigation (until logout)
 */
export default function ExportProgressNotification({
  jobId,
  onClose,
  onComplete
}) {
  const [jobInfo, setJobInfo] = useState(null);
  const [autoDownloaded, setAutoDownloaded] = useState(false);

  const handleAutoDownload = async () => {
    if (autoDownloaded || !jobId) return;

    try {
      setAutoDownloaded(true);
      const blob = await downloadExportedFile(jobId);
      const filename = generateExportFileName();
      downloadFile(blob, filename);
      
      // Keep notification visible for 5 seconds after download
      setTimeout(() => {
        onComplete && onComplete();
      }, 5000);
    } catch (error) {
      console.error('Error auto-downloading file:', error);
      setAutoDownloaded(false);
    }
  };

  useEffect(() => {
    if (!jobId) return;

    let unsubscribe = null;

    // Subscribe to job updates
    unsubscribe = backgroundJobManager.subscribe(jobId, (updatedJob) => {
      if (updatedJob) {
        setJobInfo(updatedJob);
      }
    });

    // Get initial job info
    const initialJob = backgroundJobManager.getJob(jobId);
    
    if (initialJob) {
      setJobInfo(initialJob);
      
      // Start polling if job is active
      if (initialJob.status === 'pending' || initialJob.status === 'processing') {
        backgroundJobManager.startPolling(jobId, getExportJobStatus, 2000);
      }
    } else {
      // If job doesn't exist, start polling to check status
      backgroundJobManager.startPolling(jobId, getExportJobStatus, 2000);
    }

    return () => {
      if (unsubscribe) {
        unsubscribe();
      }
    };
  }, [jobId]);
  
  // Separate effect for auto-download trigger
  useEffect(() => {
    if (jobInfo?.status === 'completed' && jobInfo?.downloadUrl && !autoDownloaded && jobId) {
      handleAutoDownload();
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [jobInfo?.status, jobInfo?.downloadUrl, autoDownloaded, jobId]);

  const handleManualDownload = async () => {
    if (!jobId || !jobInfo?.downloadUrl) return;

    try {
      const blob = await downloadExportedFile(jobId);
      const filename = generateExportFileName();
      downloadFile(blob, filename);
    } catch (error) {
      console.error('Error downloading file:', error);
    }
  };

  const handleClose = () => {
    // AC03: Close notification but job continues in background
    onClose && onClose();
  };

  const getStatusIcon = () => {
    if (!jobInfo) return null;
    
    switch (jobInfo.status) {
      case 'completed':
        return <CheckCircle className="w-4 h-4 text-green-600" />;
      case 'failed':
        return <XCircle className="w-4 h-4 text-red-600" />;
      default:
        return <FileSpreadsheet className="w-4 h-4 text-blue-600 animate-pulse" />;
    }
  };

  const getStatusText = () => {
    if (!jobInfo) return 'Initializing...';
    
    switch (jobInfo.status) {
      case 'pending':
        return 'Đang khởi tạo...';
      case 'processing':
        return `Đang tạo file... ${jobInfo.progress || 0}%`;
      case 'completed':
        return autoDownloaded ? 'Đã tải xuống!' : 'Hoàn thành!';
      case 'failed':
        return 'Có lỗi xảy ra';
      default:
        return 'Đang xử lý...';
    }
  };

  const getFileName = () => {
    // Generate filename based on export request
    if (jobInfo?.exportRequest) {
      const { testOrderIds, filters } = jobInfo.exportRequest;
      if (testOrderIds && testOrderIds.length > 0) {
        return `Test Orders (${testOrderIds.length} orders).xlsx`;
      }
      if (filters) {
        return 'Test Orders (Filtered).xlsx';
      }
    }
    return 'Test Orders (Current Month).xlsx';
  };

  const canDownload = jobInfo?.status === 'completed' && jobInfo?.downloadUrl;
  const isProcessing = jobInfo?.status === 'pending' || jobInfo?.status === 'processing';
  const isCompleted = jobInfo?.status === 'completed';
  const isFailed = jobInfo?.status === 'failed';

  if (!jobInfo) return null;

  return (
    <div 
      className="fixed bottom-4 right-4 w-80 bg-white rounded-lg shadow-lg border border-gray-200 p-4 z-50"
      style={{ borderRadius: '10px' }}
    >
      {/* Header */}
      <div className="flex items-center justify-between mb-3">
        <div className="flex items-center gap-2">
          {getStatusIcon()}
          <span className="font-semibold text-sm text-gray-900">Export Excel</span>
          {jobInfo.status && (
            <Tag 
              color={
                isCompleted ? 'green' : 
                isFailed ? 'red' : 
                'blue'
              }
              className="text-xs"
            >
              {jobInfo.status}
            </Tag>
          )}
        </div>
        <button
          onClick={handleClose}
          className="text-gray-400 hover:text-gray-600 transition-colors"
        >
          <X className="w-4 h-4" />
        </button>
      </div>

      {/* File Name */}
      <div className="mb-2">
        <p className="text-xs text-gray-600 truncate" title={getFileName()}>
          {getFileName()}
        </p>
      </div>

      {/* Progress Bar */}
      <div className="mb-3">
        <Progress
          percent={jobInfo.progress || 0}
          status={
            isFailed
              ? 'exception'
              : isCompleted
              ? 'success'
              : 'active'
          }
          strokeColor={
            isCompleted
              ? '#52c41a'
              : isFailed
              ? '#ff4d4f'
              : '#1890ff'
          }
          size="small"
          showInfo={isProcessing}
        />
      </div>

      {/* Status Message */}
      <div className="mb-3">
        <p className="text-xs text-gray-700">{getStatusText()}</p>
      </div>

      {/* Actions */}
      {canDownload && !autoDownloaded && (
        <Button
          type="primary"
          size="small"
          icon={<Download className="w-3 h-3" />}
          onClick={handleManualDownload}
          block
          className="text-xs"
        >
          Download File
        </Button>
      )}

      {isCompleted && autoDownloaded && (
        <div className="flex items-center gap-2 text-xs text-green-600">
          <CheckCircle className="w-3 h-3" />
          <span>File đã được tải xuống tự động</span>
        </div>
      )}

      {isFailed && (
        <p className="text-xs text-red-600">{jobInfo.errorMessage || 'Unknown error'}</p>
      )}
    </div>
  );
}

