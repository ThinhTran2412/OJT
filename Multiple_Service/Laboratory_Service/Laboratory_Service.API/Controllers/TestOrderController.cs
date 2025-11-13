using Laboratory_Service.Application.TestOrders.Commands;
using Laboratory_Service.Application.TestOrders.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;


namespace Laboratory_Service.API.Controllers
{
    /// <summary>
    /// Create many endpoint for api CRUD TestOrder
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    [Route("api/[controller]")]
    [ApiController]
    public class TestOrderController : ControllerBase
    {
        /// <summary>
        /// The mediator
        /// </summary>
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestOrderController"/> class.
        /// </summary>
        /// <param name="mediator">The mediator.</param>
        public TestOrderController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Create a new test order for a patient.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Policy = "CanCreateTestOrder")]
        [SwaggerOperation(Summary = "Create a new test order", Description = "Creates a test order; handles patient/medical record per business rules.")]
        public async Task<IActionResult> Create([FromBody] CreateTestOrderCommand command)
        {
            // Source CreatedBy from claims if available
            var userIdClaim = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out var createdBy))
            {
                command.CreatedBy = createdBy;
            }

            var orderId = await _mediator.Send(command);
            return Ok(new { orderId = orderId });
        }

        /// <summary>
        /// Gets the by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            return Ok(new { Message = $"Details for Test Order {id}" });
        }

        /// <summary>
        /// Modify an existing Test Order for a patient.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        [HttpPatch("{id}")]
        [Authorize(Policy = "CanModifyTestOrder")]
        public async Task<IActionResult> ModifyTestOrder(string id, [FromBody] ModifyTestOrderCommand command)
        {
            if (!Guid.TryParse(id, out var parsedId))
            {
                return BadRequest(new { Message = $"Invalid TestOrderId: {id}" });
            }

            var userId = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var userName = User?.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;

            if (command.TestOrderId != parsedId)
                return BadRequest("TestOrderId in URL and body do not match.");

            if (!int.TryParse(userId, out var parsedUserId))
            {
                return BadRequest(new { Message = $"Invalid UserId: {userId}" });
            }

            command.UpdatedBy = parsedUserId;
            command.UpdatedByName = userName!;

            var result = await _mediator.Send(command);

            return Ok(new
            {
                message = "TestOrder modified successfully.",
                testOrderId = parsedId
            });
        }



        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(Policy = "CanDeleteTestOrder")]
        public async Task<IActionResult> Delete(string id)
        {
            if (!Guid.TryParse(id, out var parsedId))
            {
                return BadRequest(new { Message = $"Invalid TestOrderId: {id}" });
            }
            var userId = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var userName = User?.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;

            if (!int.TryParse(userId, out var parsedUserId))
            {
                return BadRequest(new { Message = $"Invalid UserId: {userId}" });
            }

            var command = new DeleteTestOrderCommand(parsedId, parsedUserId!, userName!);
            var result = await _mediator.Send(command);

            return result
                ? Ok(new { Message = "Test order deleted successfully." })
                : BadRequest(new { Message = "Failed to delete test order." });
        }
        /// <summary>
        /// Get paged list of patient test orders with optional search, filtering, and sorting.
        /// Query params:
        /// - search: keyword across patient name, phone number, status, or user names
        /// - page, pageSize: paging
        /// - sortBy: id|patientName|age|gender|phoneNumber|status|createdDate|runDate; sortDesc: true|false
        /// - status: filter by status (Pending, Cancelled, Completed)
        /// Default sorting is by createdDate descending (most recent first).
        /// Returns "No Data" message when no records are found.
        /// </summary>
        [HttpGet("getList")]
        [SwaggerOperation(
            Summary = "Get list of patient test orders",
            Description = "Get paginated list of patient test orders with search, filter, and sort capabilities. Default sort is by createdDate descending."
        )]
        // [Authorize(Policy = "CanViewTestOrders")]
        public async Task<IActionResult> Get([FromQuery] GetTestOrdersQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Get detailed information of a specific patient test order by ID.
        /// Returns patient information (from MedicalRecord) and test order details.
        /// If the test order is not found, returns 404 Not Found.
        /// </summary>
        [HttpGet("detail/{id:guid}")]
        [SwaggerOperation(
            Summary = "Get patient test order detail",
            Description = "Get detailed information of a patient test order including patient info and test order details."
        )]
        //[Authorize(Policy = "CanViewTestOrders")]
        public async Task<IActionResult> GetDetail(Guid id)
        {
            var query = new GetTestOrderDetailQuery(id);
            var result = await _mediator.Send(query);

            if (result == null)
            {
                return NotFound(new { message = "Test order not found or has been deleted." });
            }

            return Ok(result);
        }
    }
}
