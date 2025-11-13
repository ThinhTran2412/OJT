using Laboratory_Service.Application.DTOs.Pagination;
using Laboratory_Service.Domain.Entity;

namespace Laboratory_Service.Application.Interface
{
    /// <summary>
    /// Create methods for interface TestOrderRepository
    /// </summary>
    public interface ITestOrderRepository
    {
        /// <summary>
        /// Adds the asynchronous.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task AddAsync(TestOrder order, CancellationToken cancellationToken = default);
        /// <summary>
        /// Gets the by identifier asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<TestOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        /// <summary>
        /// Gets all by patient identifier asynchronous.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<IEnumerable<TestOrder>> GetAllByPatientIdAsync(int patientId, CancellationToken cancellationToken = default);
        /// <summary>
        /// Saves the changes asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
        /// <summary>
        /// Updates the asynchronous.
        /// </summary>
        /// <param name="testOrder">The test order.</param>
        /// <returns></returns>
        Task UpdateAsync(TestOrder testOrder);
        /// <summary>
        /// Deletes the asynchronous.
        /// </summary>
        /// <param name="testOrder">The test order.</param>
        /// <returns></returns>
        Task DeleteAsync(TestOrder testOrder);

        Task<PagedResult<TestOrder>> GetTestOrdersAsync(
            string? search,
            int page,
            int pageSize,
            string? sortBy,
            bool sortDesc,
            string? status,
            CancellationToken cancellationToken);


    }
}
