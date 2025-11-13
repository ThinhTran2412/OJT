using IAM_Service.Application.Interface.IRole;
using IAM_Service.Application.Interface.IUser;
using MediatR;

namespace IAM_Service.Application.Users.Command
{
    /// <summary>
    /// Handles the update operation for an existing user.
    /// Applies partial update logic — only updates fields provided in the request.
    /// </summary>
    /// <seealso cref="MediatR.IRequestHandler&lt;IAM_Service.Application.Users.Command.UpdateUserCommand, MediatR.Unit&gt;" />
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Unit>
    {
        /// <summary>
        /// The user repository
        /// </summary>
        private readonly IUsersRepository _userRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateUserCommandHandler" /> class.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        public UpdateUserCommandHandler(IUsersRepository userRepository, IRoleRepository roleRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// Handles the update user command request.
        /// Validates user existence, applies updates only for fields provided, and saves changes.
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Response from the request
        /// </returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">User not found.</exception>
        public async Task<Unit> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByUserIdAsync(request.UserId);
            if (user == null)
                throw new KeyNotFoundException("User not found.");

            if (!string.IsNullOrWhiteSpace(request.FullName))
                user.FullName = request.FullName;

            if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
                user.PhoneNumber = request.PhoneNumber;

            if (!string.IsNullOrWhiteSpace(request.Gender))
                user.Gender = request.Gender;

            if (request.Age.HasValue && request.Age.Value > 0)
                user.Age = request.Age.Value;

            if (!string.IsNullOrWhiteSpace(request.Address))
                user.Address = request.Address;

            if (request.DateOfBirth.HasValue)
                user.DateOfBirth = request.DateOfBirth.Value;

            await _userRepository.UpdateUserAsync(user);
            await _userRepository.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
