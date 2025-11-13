using Laboratory_Service.Application.DTOs.Pagination;
using Laboratory_Service.Application.Interface;
using Laboratory_Service.Domain.Entity;
using Laboratory_Service.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Laboratory_Service.Infrastructure.Repositories
{
    /// <summary>
    /// Implement method form ITestOrderRepository
    /// </summary>
    /// <seealso cref="Laboratory_Service.Application.Interface.ITestOrderRepository" />
    public class TestOrderRepository : ITestOrderRepository
    {
        /// <summary>
        /// The database context
        /// </summary>
        private readonly AppDbContext _dbContext;
        /// <summary>
        /// Initializes a new instance of the <see cref="TestOrderRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public TestOrderRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Adds the asynchronous.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task AddAsync(TestOrder order, CancellationToken cancellationToken = default)
        {
            await _dbContext.TestOrders.AddAsync(order, cancellationToken);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes the asynchronous.
        /// </summary>
        /// <param name="testOrder">The test order.</param>
        public async Task DeleteAsync(TestOrder testOrder)
        {
            _dbContext.TestOrders.Remove(testOrder);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Gets all by patient identifier asynchronous.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<IEnumerable<TestOrder>> GetAllByPatientIdAsync(int patientId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.TestOrders
                .Where(o => o.PatientId == patientId)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Gets the by identifier asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<TestOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.TestOrders
                .Include(o => o.Patient)
                .FirstOrDefaultAsync(o => o.TestOrderId == id, cancellationToken);
        }

        /// <summary>
        /// Saves the changes asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Updates the asynchronous.
        /// </summary>
        /// <param name="testOrder">The test order.</param>
        public async Task UpdateAsync(TestOrder testOrder)
        {
             _dbContext.TestOrders.Update(testOrder);
             await _dbContext.SaveChangesAsync();
        }
        /// <summary>
        /// Query patient test orders with search, paging, sorting and status filtering.
        /// Returns a paged result for handlers.
        /// </summary>
        public async Task<PagedResult<TestOrder>> GetTestOrdersAsync(
            string? search,
            int page,
            int pageSize,
            string? sortBy,
            bool sortDesc,
            string? status,
            CancellationToken cancellationToken)
        {
            // Base query with eager loading for related patient, medical record, and test results. No tracking for read performance.
            IQueryable<TestOrder> query = _dbContext.TestOrders
                .Include(p => p.Patient)
                .Include(p => p.MedicalRecord)
                .AsNoTracking();

            // Apply status filter if provided
            if (!string.IsNullOrWhiteSpace(status))
            {
                string statusFilter = status.Trim();
                query = query.Where(p => p.Status == statusFilter);
            }

            // Apply search filter if provided
            if (!string.IsNullOrWhiteSpace(search))
            {
                string s = search.Trim().ToLower();
                query = query.Where(p =>
                    p.PatientName.ToLower().Contains(s)
                    || p.PhoneNumber.ToLower().Contains(s)
                    || p.Status.ToLower().Contains(s)
                );
            }

            // Apply sorting by requested column (default to createdDate descending for most recent)
            if (string.IsNullOrEmpty(sortBy))
            {
                sortBy = "createdDate";
                sortDesc = true; // Default to descending (most recent first)
            }
            string sort = sortBy.ToLower();

            switch (sort)
            {
                case "id":
                case "testorderid":
                    query = sortDesc ? query.OrderByDescending(p => p.TestOrderId) : query.OrderBy(p => p.TestOrderId);
                    break;
                case "patientname":
                case "patient":
                    query = sortDesc ? query.OrderByDescending(p => p.PatientName)
                                     : query.OrderBy(p => p.PatientName);
                    break;
                case "age":
                    query = sortDesc ? query.OrderByDescending(p => p.Age)
                                     : query.OrderBy(p => p.Age);
                    break;
                case "gender":
                    query = sortDesc ? query.OrderByDescending(p => p.Gender)
                                     : query.OrderBy(p => p.Gender);
                    break;
                case "phonenumber":
                case "phone":
                    query = sortDesc ? query.OrderByDescending(p => p.PhoneNumber)
                                     : query.OrderBy(p => p.PhoneNumber);
                    break;
                case "status":
                    query = sortDesc ? query.OrderByDescending(p => p.Status) : query.OrderBy(p => p.Status);
                    break;
                case "rundate":
                case "run":
                    query = sortDesc ? query.OrderByDescending(p => p.RunDate) : query.OrderBy(p => p.RunDate);
                    break;
                case "createddate":
                case "created":
                default:
                    query = sortDesc ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt);
                    break;
            }

            // Count BEFORE paging to get total records
            int total = await query.CountAsync(cancellationToken);

            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            // Apply paging at the database level
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<TestOrder>
            {
                Items = items,
                Total = total,
                Page = page,
                PageSize = pageSize
            };
        }
    }
}
