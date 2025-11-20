using Laboratory_Service.Domain.Entity;

namespace Laboratory_Service.Application.Interface
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICommentRepository
    {
        /// <summary>
        /// Gets the by identifier asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        Task<Domain.Entity.Comment?> GetByIdAsync(long id);
        /// <summary>
        /// Adds the asynchronous.
        /// </summary>
        /// <param name="comment">The comment.</param>
        /// <returns></returns>
        Task AddAsync(Domain.Entity.Comment comment);
        /// <summary>
        /// Saves the changes asynchronous.
        /// </summary>
        /// <returns></returns>
        Task SaveChangesAsync();
    }
}
