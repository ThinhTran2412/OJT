using AutoMapper;
using IAM_Service.Application.Interface.IEmailSender;
using IAM_Service.Application.Interface.IPasswordHasher;
using IAM_Service.Application.Interface.IUser;
using IAM_Service.Application.Interface.IRole;
using IAM_Service.Application.Users.Command;
using IAM_Service.Domain.Entity;
using MediatR;
using Moq;

namespace IAM_Service.Application.Tests.Users.Command
{
    /// <summary>
    /// create usecase test for  CreateUserCommandHandler
    /// </summary>
    public class CreateUserCommandHandlerTests
    {
        /// <summary>
        /// The mock user repository
        /// </summary>
        private readonly Mock<IUsersRepository> _mockUserRepository;
        /// <summary>
        /// The mock role repository
        /// </summary>
        private readonly Mock<IRoleRepository> _mockRoleRepository;
        /// <summary>
        /// The mock mapper
        /// </summary>
        private readonly Mock<IMapper> _mockMapper;
        /// <summary>
        /// The mock password hasher
        /// </summary>
        private readonly Mock<IPasswordHasher> _mockPasswordHasher;
        /// <summary>
        /// The mock email sender
        /// </summary>
        private readonly Mock<IEmailSender> _mockEmailSender;
        /// <summary>
        /// The handler
        /// </summary>
        private readonly CreateUserCommandHandler _handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateUserCommandHandlerTests"/> class.
        /// </summary>
        public CreateUserCommandHandlerTests()
        {
            _mockUserRepository = new Mock<IUsersRepository>();
            _mockRoleRepository = new Mock<IRoleRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockPasswordHasher = new Mock<IPasswordHasher>();
            _mockEmailSender = new Mock<IEmailSender>();

            _handler = new CreateUserCommandHandler(
                _mockUserRepository.Object,
                _mockRoleRepository.Object,
                _mockMapper.Object,
                _mockPasswordHasher.Object,
                _mockEmailSender.Object
            );
        }

        /// <summary>
        /// Handles the valid command should call all dependencies correctly.
        /// </summary>
        [Fact]
        public async Task Handle_ValidCommand_ShouldCallAllDependenciesCorrectly()
        {
            // ARRANGE
            var command = new CreateUserCommand
            {
                Email = "Tanloc123@gmail.com",
                Address = "Hồ Chí Minh",
                Age = 20,
                DateOfBirth = new DateOnly(2004, 3, 31),
                FullName = "Pham Tan Loc",
                Gender = "Male",
                IdentifyNumber = "080204000454",
                PhoneNumber = "0373097930"
            };

            var newUser = new User { Email = command.Email };
            var fakeHashedPassword = "hashed_password_123";

            _mockUserRepository.Setup(r => r.GetByEmailAsync(command.Email))
                               .ReturnsAsync((User?)null);

            // Mock role repository to return null (no specific role, will use default)
            _mockRoleRepository.Setup(r => r.GetByCodeAsync("READ_ONLY", It.IsAny<CancellationToken>()))
                               .ReturnsAsync((Role?)null);

            _mockMapper.Setup(m => m.Map<User>(command))
                       .Returns(newUser);

            _mockPasswordHasher.Setup(p => p.Hash(It.IsAny<string>()))
                               .Returns(fakeHashedPassword);

            _mockUserRepository.Setup(r => r.CreateUser(It.IsAny<User>()))
                               .Returns(Task.CompletedTask);

            // ACT
            var result = await _handler.Handle(command, CancellationToken.None);

            // ASSERT
            Assert.Equal(Unit.Value, result);

            // Verify:
            _mockMapper.Verify(m => m.Map<User>(command), Times.Once);

            _mockPasswordHasher.Verify(p => p.Hash(It.IsAny<string>()), Times.Once);

            _mockUserRepository.Verify(r => r.CreateUser(It.Is<User>(u => u.Password == fakeHashedPassword)), Times.Once);

            _mockEmailSender.Verify(e => e.SendEmail(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                command.Email,
                It.IsAny<string>(),
                It.Is<string>(body => body.Contains("Your temporary password"))
            ), Times.Once);
        }

        /// <summary>
        /// Handles the duplicate email should throw invalid operation exception.
        /// </summary>
        [Fact]
        public async Task Handle_DuplicateEmail_ShouldThrowInvalidOperationException()
        {
            // ARRANGE
            var command = new CreateUserCommand { Email = "congloc313@gmail.com" };
            var existingUser = new User { Email = command.Email };

            _mockUserRepository.Setup(r => r.GetByEmailAsync(command.Email))
                               .ReturnsAsync(existingUser);

            // ACT & ASSERT
            await Assert.ThrowsAsync<InvalidOperationException>(
                async () => await _handler.Handle(command, CancellationToken.None)
            );

            // Verify:
            _mockUserRepository.Verify(r => r.CreateUser(It.IsAny<User>()), Times.Never);

            _mockEmailSender.Verify(e => e.SendEmail(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()
            ), Times.Never);
        }
    }
}
