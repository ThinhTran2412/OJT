using FluentAssertions;
using IAM_Service.Application.DTOs.Roles;
using IAM_Service.Application.Interface.IPrivilege;
using IAM_Service.Application.Interface.IRole;
using IAM_Service.Application.Roles.Command;
using IAM_Service.Domain.Entity;
using Moq;

namespace IAM_Service.Application.UnitTests.Roles.Command
{
    /// <summary>
    /// create usecase test for CreateRoleCommand
    /// </summary>
    public class CreateRoleCommandHandlerTests
    {
        /// <summary>
        /// The role repo mock
        /// </summary>
        private readonly Mock<IRoleCommandRepository> _roleRepoMock;
        /// <summary>
        /// The priv repo mock
        /// </summary>
        private readonly Mock<IPrivilegeRepository> _privRepoMock;
        /// <summary>
        /// The handler
        /// </summary>
        private readonly CreateRoleCommandHandler _handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateRoleCommandHandlerTests"/> class.
        /// </summary>
        public CreateRoleCommandHandlerTests()
        {
            _roleRepoMock = new Mock<IRoleCommandRepository>();
            _privRepoMock = new Mock<IPrivilegeRepository>();
            _handler = new CreateRoleCommandHandler(_roleRepoMock.Object, _privRepoMock.Object);
        }

        /// <summary>
        /// Handles the should throw when role already exists.
        /// </summary>
        [Fact]
        public async Task Handle_ShouldThrow_WhenRoleAlreadyExists()
        {
            var dto = new RoleCreateDto
            {
                Name = "Admin",
                Code = "ADMIN",
                Description = "Administrator role",
                PrivilegeIds = new List<int> { 1, 2 }
            };
            var command = new CreateRoleCommand(dto);

            _roleRepoMock.Setup(r => r.ExistsByCodeAsync("ADMIN", It.IsAny<CancellationToken>()))
                         .ReturnsAsync(true);

            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("Role with code 'ADMIN' already exists.");

            _roleRepoMock.Verify(r => r.AddAsync(It.IsAny<Role>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        /// <summary>
        /// Handles the should assign default read only privilege when no privilege ids provided.
        /// </summary>
        [Fact]
        public async Task Handle_ShouldAssignDefaultReadOnlyPrivilege_WhenNoPrivilegeIdsProvided()
        {
            var dto = new RoleCreateDto
            {
                Name = "Viewer",
                Code = "VIEWER",
                PrivilegeIds = new List<int>()
            };
            var command = new CreateRoleCommand(dto);

            _roleRepoMock.Setup(r => r.ExistsByCodeAsync("VIEWER", It.IsAny<CancellationToken>()))
                         .ReturnsAsync(false);

            _privRepoMock.Setup(p => p.GetByNameAsync("READ_ONLY", It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new Privilege { PrivilegeId = 99, Name = "READ_ONLY" });

            _roleRepoMock.Setup(r => r.AddAsync(It.IsAny<Role>(), It.IsAny<CancellationToken>()))
                         .Returns(Task.CompletedTask);

            _roleRepoMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                         .Returns(Task.CompletedTask);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().NotBeNull();
            result.Privileges.Should().ContainSingle(p => p == "READ_ONLY");
        }
    }
}