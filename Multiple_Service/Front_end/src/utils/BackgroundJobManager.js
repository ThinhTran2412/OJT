/**
 * Background Job Manager
 * Manages multiple background export jobs (AC03: Non-blocking)
 * Allows users to continue working while exports run in background
 * 
 * Single Responsibility: Job lifecycle management only
 */

class BackgroundJobManager {
  constructor() {
    this.jobs = new Map(); // jobId -> jobInfo
    this.listeners = new Map(); // jobId -> Set of callbacks
    this.pollingIntervals = new Map(); // jobId -> intervalId
  }

  /**
   * Register a new export job
   * @param {string} jobId - Unique job identifier
   * @param {object} jobInfo - Job information
   */
  registerJob(jobId, jobInfo) {
    const existingJob = this.jobs.get(jobId);
    const createdAt = existingJob?.createdAt || jobInfo?.createdAt || new Date().toISOString();
    
    const jobData = {
      ...jobInfo,
      jobId,
      status: jobInfo.status || 'pending',
      progress: jobInfo.progress || 0,
      createdAt,
    };
    
    this.jobs.set(jobId, jobData);
    this.notifyListeners(jobId);
    this.dispatchJobRegisteredEvent(jobId);
  }

  /**
   * Update job status
   * @param {string} jobId - Job identifier
   * @param {object} updates - Status updates
   */
  updateJob(jobId, updates) {
    const job = this.jobs.get(jobId);
    if (job) {
      Object.assign(job, updates);
      job.jobId = jobId;
      this.jobs.set(jobId, job);
      this.notifyListeners(jobId);
    }
  }

  /**
   * Get job information
   * @param {string} jobId - Job identifier
   * @returns {object|null} - Job information or null
   */
  getJob(jobId) {
    const job = this.jobs.get(jobId);
    return job ? { ...job, jobId } : null;
  }

  /**
   * Get all jobs
   * @returns {Array} - Array of job information with jobId included
   */
  getAllJobs() {
    return Array.from(this.jobs.entries()).map(([jobId, jobData]) => ({
      ...jobData,
      jobId,
    }));
  }

  /**
   * Get active jobs (pending or processing)
   * @returns {Array} - Array of active job information
   */
  getActiveJobs() {
    return this.getAllJobs().filter(
      job => job.status === 'pending' || job.status === 'processing'
    );
  }

  /**
   * Subscribe to job updates
   * @param {string} jobId - Job identifier
   * @param {function} callback - Callback function
   * @returns {function} - Unsubscribe function
   */
  subscribe(jobId, callback) {
    if (!this.listeners.has(jobId)) {
      this.listeners.set(jobId, new Set());
    }
    this.listeners.get(jobId).add(callback);

    return () => {
      const callbacks = this.listeners.get(jobId);
      if (callbacks) {
        callbacks.delete(callback);
        if (callbacks.size === 0) {
          this.listeners.delete(jobId);
        }
      }
    };
  }

  /**
   * Notify all listeners of a job update
   * @param {string} jobId - Job identifier
   */
  notifyListeners(jobId) {
    const callbacks = this.listeners.get(jobId);
    if (callbacks) {
      const job = this.jobs.get(jobId);
      if (job) {
        const jobWithId = { ...job, jobId };
        callbacks.forEach(callback => {
          try {
            callback(jobWithId);
          } catch (error) {
            console.error('Error in job listener callback:', error);
          }
        });
      }
    }
  }

  /**
   * Start polling for a job
   * @param {string} jobId - Job identifier
   * @param {function} statusChecker - Function that returns job status
   * @param {number} interval - Polling interval in milliseconds (default: 2000)
   */
  startPolling(jobId, statusChecker, interval = 2000) {
    this.stopPolling(jobId);

    const poll = async () => {
      try {
        const status = await statusChecker(jobId);
        this.updateJob(jobId, status);

        if (status.status === 'completed' || status.status === 'failed') {
          this.stopPolling(jobId);
        }
      } catch (error) {
        console.error(`Error polling job ${jobId}:`, error);
        this.updateJob(jobId, {
          status: 'failed',
          errorMessage: error.message || 'Failed to check job status',
        });
        this.stopPolling(jobId);
      }
    };

    poll();
    const intervalId = setInterval(poll, interval);
    this.pollingIntervals.set(jobId, intervalId);
  }

  /**
   * Stop polling for a job
   * @param {string} jobId - Job identifier
   */
  stopPolling(jobId) {
    const intervalId = this.pollingIntervals.get(jobId);
    if (intervalId) {
      clearInterval(intervalId);
      this.pollingIntervals.delete(jobId);
    }
  }

  /**
   * Remove a job (cleanup)
   * @param {string} jobId - Job identifier
   */
  removeJob(jobId) {
    this.stopPolling(jobId);
    this.jobs.delete(jobId);
    this.listeners.delete(jobId);
  }

  /**
   * Cleanup completed jobs older than specified time
   * @param {number} maxAge - Maximum age in milliseconds (default: 1 hour)
   */
  cleanupOldJobs(maxAge = 60 * 60 * 1000) {
    const now = Date.now();
    const jobsToRemove = [];

    this.jobs.forEach((job, jobId) => {
      if (job.status === 'completed' || job.status === 'failed') {
        const createdAt = new Date(job.createdAt).getTime();
        if (now - createdAt > maxAge) {
          jobsToRemove.push(jobId);
        }
      }
    });

    jobsToRemove.forEach(jobId => this.removeJob(jobId));
  }

  /**
   * Dispatch custom event for new job registration
   * @private
   */
  dispatchJobRegisteredEvent(jobId) {
    const event = new CustomEvent('exportJobRegistered', { detail: { jobId } });
    window.dispatchEvent(event);
  }
}

// Singleton instance
const backgroundJobManager = new BackgroundJobManager();

// Cleanup old jobs every 5 minutes
setInterval(() => {
  backgroundJobManager.cleanupOldJobs();
}, 5 * 60 * 1000);

export default backgroundJobManager;
