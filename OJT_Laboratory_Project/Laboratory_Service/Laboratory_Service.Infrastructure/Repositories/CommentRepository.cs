using Laboratory_Service.Application.Interface;
using Laboratory_Service.Domain.Entity;
using Laboratory_Service.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Laboratory_Service.Application.Interface.ICommentRepository" />
    public class CommentRepository : ICommentRepository
    {
        /// <summary>
        /// The context
        /// </summary>
        private readonly AppDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommentRepository"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public CommentRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets the by identifier asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public async Task<Comment?> GetByIdAsync(long id)
        {
            return await _context.Comment
                .FirstOrDefaultAsync(x => x.CommentId == id);
        }


        /// <summary>
        /// Adds the asynchronous.
        /// </summary>
        /// <param name="comment">The comment.</param>
        public async Task AddAsync(Comment comment)
        {
            await _context.Comment.AddAsync(comment);
        }

        /// <summary>
        /// Saves the changes asynchronous.
        /// </summary>
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
