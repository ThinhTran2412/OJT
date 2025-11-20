using Laboratory_Service.Application.Comments.Commands;
using Laboratory_Service.Application.DTOs.Comment;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


namespace Laboratory_Service.API.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : ControllerBase
    {
        /// <summary>
        /// The mediator
        /// </summary>
        private readonly IMediator _mediator;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<CommentController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommentController"/> class.
        /// </summary>
        /// <param name="mediator">The mediator.</param>
        /// <param name="logger">The logger.</param>
        public CommentController(IMediator mediator, ILogger<CommentController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Add new comment for TestOrder or TestResult
        /// </summary>
        /// <param name="dto">The dto.</param>
        /// <returns></returns>
        [HttpPost("add")]
        [Authorize]
        public async Task<IActionResult> Add([FromBody] CreateCommentDto dto)
        {
            _logger.LogInformation("Starting Add Comment request...");

            // Lấy UserId từ JWT
            var userIdClaim = User.FindFirst("sub");

            if (userIdClaim == null)
                return Unauthorized("User ID not found in JWT token.");

            long userId = long.Parse(userIdClaim.Value);

            var command = new AddCommentCommand(dto, userId);
            var result = await _mediator.Send(command);

            _logger.LogInformation("Comment created successfully with ID = {CommentId}", result);

            return Ok(new
            {
                CommentId = result,
                Message = "Comment added successfully"
            });
        }


        /// <summary>
        /// Soft delete a comment
        /// </summary>
        /// <param name="commentId">The comment identifier.</param>
        /// <returns></returns>
        [HttpDelete("delete/{commentId}")]
        [Authorize]
        public async Task<IActionResult> Delete(long commentId)
        {
            // Lấy userId từ JWT (thử "userId" trước, fallback sang ClaimTypes.NameIdentifier)
            var userIdClaim = User.FindFirst("sub") ?? User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || string.IsNullOrWhiteSpace(userIdClaim.Value))
            {
                return Unauthorized(new { Message = "Invalid token or userId not found in token." });
            }

            if (!long.TryParse(userIdClaim.Value, out var deletedBy))
            {
                return Unauthorized(new { Message = "Invalid userId value in token." });
            }

            _logger.LogInformation("Delete Comment request for ID = {CommentId} by user {DeletedBy}",
                commentId, deletedBy);

            // DTO chỉ chứa CommentId (DeletedBy không còn trong DTO)
            var dto = new DeleteCommentDto
            {
                CommentId = commentId
            };

            // Tạo command với dto và deletedBy (lấy từ JWT)
            var command = new DeleteCommentCommand(dto, deletedBy);
            var success = await _mediator.Send(command);

            if (!success)
            {
                _logger.LogWarning("Delete failed: Comment {CommentId} not found or already deleted", commentId);

                return NotFound(new
                {
                    Message = "Comment not found or already deleted"
                });
            }

            _logger.LogInformation("Comment {CommentId} deleted successfully by {DeletedBy}",
                commentId, deletedBy);

            return Ok(new
            {
                Message = "Comment deleted successfully"
            });
        }
    }
}
