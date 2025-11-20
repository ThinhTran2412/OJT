using AutoMapper;
using Laboratory_Service.Application.Comments.Commands;
using Laboratory_Service.Application.DTOs.Comment;
using Laboratory_Service.Application.Interface;
using Moq;

namespace Laboratory_Service.Application.UnitTest.Comments
{
    public class AddCommentCommandHandlerTests
    {
        private readonly Mock<ICommentRepository> _commentRepoMock;
        private readonly IMapper _mapper;

        public AddCommentCommandHandlerTests()
        {
            _commentRepoMock = new Mock<ICommentRepository>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CreateCommentDto, Domain.Entity.Comment>();
            });

            _mapper = config.CreateMapper();
        }

        [Fact]
        public async Task Handle_ShouldCreateComment_WhenValidRequest()
        {
            // Arrange
            var dto = new CreateCommentDto
            {
                TestOrderId = Guid.Parse("8e200a3c-6fe0-46b8-a628-98fa3a5eed3b"),
                TestResultId = 10,
                Message = "New test comment"
            };

            long jwtUserId = 99; // giả lập userId lấy từ JWT

            var command = new AddCommentCommand(dto, jwtUserId);

            _commentRepoMock
                .Setup(x => x.AddAsync(It.IsAny<Domain.Entity.Comment>()))
                .Returns(Task.CompletedTask);

            _commentRepoMock
                .Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            var handler = new AddCommentCommandHandler(_commentRepoMock.Object, _mapper);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result >= 0);

            _commentRepoMock.Verify(x => x.AddAsync(It.Is<Domain.Entity.Comment>(c =>
                c.CreatedBy == jwtUserId &&
                c.Message == dto.Message
            )), Times.Once);

            _commentRepoMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }
    }
}
