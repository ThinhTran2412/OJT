using IAM_Service.Application.Registers.Command;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace IAM_Service.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RegistersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Register a new user account (Public registration for patients)
        /// </summary>
        /// <param name="command">Registration request containing FullName, Email, IdentifyNumber, Password</param>
        /// <returns>201 Created if successful</returns>
        [HttpPost("registers")]
        public async Task<IActionResult> Register([FromBody] RegistersAccountCommand command)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _mediator.Send(command);
                return StatusCode(201, new
                {
                    message = "Registration successful. Please log in to continue."
                });
            }
            catch (Exception ex)
            {
                // Log error for debugging
                Console.WriteLine($"Registration error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw; // Re-throw to let global exception handler handle it
            }
        }
    }
}
