using IAM_Service.Application.Interface.IUser;
using IAM_Service.Application.Interface.IRole;
using IAM_Service.Application.Users.Command;
using IAM_Service.Domain.Entity;
using MediatR;
using Moq;

namespace IAM_Service.Application.Tests.Users.Command
{
    /// <summary>
    /// 
    /// </summary>
    public class UpdateUserCommandHandlerTests
    {
        /// <summary>
        /// The user repo mock
        /// </summary>
        private readonly Mock<IUsersRepository> _userRepoMock;
        /// <summary>
        /// The role repo mock
        /// </summary>
        private readonly Mock<IRoleRepository> _roleRepoMock;
        /// <summary>
        /// The handler
        /// </summary>
        private readonly UpdateUserCommandHandler _handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateUserCommandHandlerTests"/> class.
        /// </summary>
        public UpdateUserCommandHandlerTests()
        {
            _userRepoMock = new Mock<IUsersRepository>();
            _roleRepoMock = new Mock<IRoleRepository>();

            _handler = new UpdateUserCommandHandler(
                _userRepoMock.Object,
                _roleRepoMock.Object
            );
        }

        /// <summary>
        /// Handles the should update fields when provided.
        /// </summary>
        [Fact]
        public async Task Handle_Should_Update_Fields_When_Provided()
        {
            // Arrange
            var existingUser = new User
            {
                UserId = 1,
                FullName = "Old Name",
            };

            var command = new UpdateUserCommand
            {
                UserId = 1,
                FullName = "New Name",
            };

            _userRepoMock.Setup(r => r.GetByUserIdAsync(1)).ReturnsAsync(existingUser);
            _userRepoMock.Setup(r => r.UpdateUserAsync(existingUser)).Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(Unit.Value, result);
            Assert.Equal("New Name", existingUser.FullName);

            _userRepoMock.Verify(r => r.UpdateUserAsync(existingUser), Times.Once);
            _userRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        /// <summary>
        /// Handles the should not update empty fields.
        /// </summary>
        [Fact]
        public async Task Handle_Should_Not_Update_Empty_Fields()
        {
            // Arrange
            var existingUser = new User
            {
                UserId = 1,
                FullName = "Old Name",
                Email = "old@gmail.com"
            };

            var command = new UpdateUserCommand
            {
                UserId = 1,
                FullName = "",  // empty -> should NOT update
            };

            _userRepoMock.Setup(r => r.GetByUserIdAsync(1)).ReturnsAsync(existingUser);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal("Old Name", existingUser.FullName);
        }

        /// <summary>
        /// Handles the should throw when user not found.
        /// </summary>
        [Fact]
        public async Task Handle_Should_Throw_When_User_Not_Found()
        {
            var command = new UpdateUserCommand { UserId = 100 };

            _userRepoMock.Setup(r => r.GetByUserIdAsync(100))
                         .ReturnsAsync((User?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _handler.Handle(command, CancellationToken.None)
            );
        }


    }
}
