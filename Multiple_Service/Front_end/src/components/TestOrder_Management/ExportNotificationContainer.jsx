import { useEffect, useState, useCallback } from 'react';
import ExportProgressNotification from './ExportProgressNotification';
import backgroundJobManager from '../../utils/BackgroundJobManager';
import { saveJobsToStorage, loadJobsFromStorage, clearJobsFromStorage } from '../../utils/jobPersistence';
import { getDisplayableJobIds } from '../../utils/jobFilters';

/**
 * Export Notification Container
 * Manages multiple export notifications at bottom-right
 * Persists across page navigation (until logout)
 * 
 * Single Responsibility: UI container for export notifications
 */
export default function ExportNotificationContainer() {
  const [activeJobs, setActiveJobs] = useState([]);

  const updateActiveJobs = useCallback(() => {
    const allJobs = backgroundJobManager.getAllJobs();
    const displayableIds = getDisplayableJobIds(allJobs);
    
    setActiveJobs(prev => {
      // Only update if changed
      if (prev.length !== displayableIds.length) {
        return displayableIds;
      }
      const prevSet = new Set(prev);
      const displayableSet = new Set(displayableIds);
      if (prevSet.size !== displayableSet.size) {
        return displayableIds;
      }
      for (const id of prevSet) {
        if (!displayableSet.has(id)) {
          return displayableIds;
        }
      }
      return prev;
    });

    saveJobsToStorage(allJobs);
  }, []);

  useEffect(() => {
    // Load persisted jobs from localStorage on mount
    const loadPersistedJobs = () => {
      const jobs = loadJobsFromStorage();
      jobs.forEach(job => {
        if (job.jobId) {
          backgroundJobManager.registerJob(job.jobId, job);
          
          if (job.status === 'pending' || job.status === 'processing') {
            backgroundJobManager.startPolling(job.jobId, async (id) => {
              const { getExportJobStatus } = await import('../../services/TestOrderService');
              return await getExportJobStatus(id);
            }, 2000);
          }
        }
      });
    };

    loadPersistedJobs();

    // Subscribe to individual job updates
    const unsubscribes = new Map();
    const subscribeToJob = (jobId) => {
      if (unsubscribes.has(jobId)) return;
      
      const unsubscribe = backgroundJobManager.subscribe(jobId, () => {
        updateActiveJobs();
      });
      unsubscribes.set(jobId, unsubscribe);
    };

    // Subscribe to existing jobs
    backgroundJobManager.getAllJobs().forEach(job => {
      if (job.jobId) {
        subscribeToJob(job.jobId);
      }
    });

    // Watch for new jobs and update
    const checkInterval = setInterval(() => {
      const allJobs = backgroundJobManager.getAllJobs();
      allJobs.forEach(job => {
        if (job.jobId && !unsubscribes.has(job.jobId)) {
          subscribeToJob(job.jobId);
        }
      });
      updateActiveJobs();
    }, 1000);

    // Listen for new job registration events
    const handleNewJob = (event) => {
      const { jobId } = event.detail;
      if (jobId) {
        subscribeToJob(jobId);
        updateActiveJobs();
      }
    };

    window.addEventListener('exportJobRegistered', handleNewJob);
    updateActiveJobs();

    return () => {
      clearInterval(checkInterval);
      window.removeEventListener('exportJobRegistered', handleNewJob);
      unsubscribes.forEach((unsubscribe) => unsubscribe());
    };
  }, [updateActiveJobs]);

  const handleJobComplete = (jobId) => {
    setTimeout(() => {
      setActiveJobs(prev => prev.filter(id => id !== jobId));
      backgroundJobManager.removeJob(jobId);
      const allJobs = backgroundJobManager.getAllJobs();
      saveJobsToStorage(allJobs);
    }, 5000);
  };

  const handleJobClose = (jobId) => {
    setActiveJobs(prev => prev.filter(id => id !== jobId));
  };

  // Clear jobs on logout
  useEffect(() => {
    const handleLogout = () => {
      activeJobs.forEach(jobId => {
        backgroundJobManager.removeJob(jobId);
      });
      clearJobsFromStorage();
      setActiveJobs([]);
    };

    window.addEventListener('logout', handleLogout);
    return () => {
      window.removeEventListener('logout', handleLogout);
    };
  }, [activeJobs]);

  const validJobs = activeJobs.filter(jobId => jobId && typeof jobId === 'string');

  if (validJobs.length === 0) return null;

  return (
    <div className="fixed bottom-4 right-4 z-50 flex flex-col gap-3" style={{ maxWidth: '320px' }}>
      {validJobs.map((jobId) => (
        <div
          key={jobId}
          style={{
            animation: 'slideInRight 0.3s ease-out',
          }}
        >
          <ExportProgressNotification
            jobId={jobId}
            onClose={() => handleJobClose(jobId)}
            onComplete={() => handleJobComplete(jobId)}
          />
        </div>
      ))}
      <style>{`
        @keyframes slideInRight {
          from {
            transform: translateX(100%);
            opacity: 0;
          }
          to {
            transform: translateX(0);
            opacity: 1;
          }
        }
      `}</style>
    </div>
  );
}
