/**
 * Job Filter Utilities
 * Single Responsibility: Filter jobs based on business rules
 */

import { shouldShowCompletedJob } from './jobPersistence';

/**
 * Check if a job should be displayed in notifications
 * @param {object} job - Job object
 * @returns {boolean} - True if job should be shown
 */
export const shouldDisplayJob = (job) => {
  if (!job || !job.jobId) return false;
  
  // Show pending or processing jobs
  if (job.status === 'pending' || job.status === 'processing') {
    return true;
  }
  
  // Show recently completed jobs
  if (job.status === 'completed') {
    return shouldShowCompletedJob(job);
  }
  
  return false;
};

/**
 * Filter jobs that should be displayed
 * @param {Array} jobs - Array of job objects
 * @returns {Array} - Array of job IDs to display
 */
export const getDisplayableJobIds = (jobs) => {
  return jobs
    .filter(shouldDisplayJob)
    .map(job => job.jobId)
    .filter(Boolean);
};

