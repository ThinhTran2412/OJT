using Laboratory_Service.Application.DTOs.Pagination;
using Laboratory_Service.Application.DTOs.TestOrders;
using Laboratory_Service.Application.Interface;
using Laboratory_Service.Application.TestOrders.Queries;
using Laboratory_Service.Domain.Entity;
using Moq;

namespace Laboratory_Service.Application.UnitTest.TestOrders.Query
{
    public class GetTestOrdersQueryHandlerTests
    {
        private readonly Mock<ITestOrderRepository> _orderRepoMock;
        private readonly GetTestOrdersQueryHandler _handler;

        public GetTestOrdersQueryHandlerTests()
        {
            _orderRepoMock = new Mock<ITestOrderRepository>();
            _handler = new GetTestOrdersQueryHandler(_orderRepoMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsPagedResponse_WithMappedItems()
        {
            // Arrange
            var items = new List<TestOrder>
            {
                new TestOrder
                {
                    TestOrderId = Guid.NewGuid(),
                    PatientName = "Alice",
                    Age = 30,
                    Gender = "Female",
                    PhoneNumber = "0911111111",
                    Status = "Created",
                    CreatedAt = DateTime.UtcNow.AddDays(-2),
                    RunDate = DateTime.UtcNow.AddDays(-1)
                },
                new TestOrder
                {
                    TestOrderId = Guid.NewGuid(),
                    PatientName = "Bob",
                    Age = 40,
                    Gender = "Male",
                    PhoneNumber = "0922222222",
                    Status = "Completed",
                    CreatedAt = DateTime.UtcNow.AddDays(-3),
                    RunDate = DateTime.UtcNow
                }
            };

            var repoResult = new PagedResult<TestOrder>
            {
                Items = items,
                Total = 2,
                Page = 1,
                PageSize = 10
            };

            _orderRepoMock
                .Setup(r => r.GetTestOrdersAsync(
                    It.IsAny<string?>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<string?>(),
                    It.IsAny<bool>(),
                    It.IsAny<string?>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(repoResult);

            var query = new GetTestOrdersQuery
            {
                Search = "",
                Page = 1,
                PageSize = 10,
                SortBy = "createdDate",
                SortDesc = true,
                Status = null
            };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Total);
            Assert.Equal(1, result.Page);
            Assert.Equal(10, result.PageSize);
            Assert.Null(result.Message);
            Assert.Equal(2, result.Items.Count);

            // Verify mapping for first item
            var first = result.Items[0];
            var sourceFirst = items[0];
            Assert.Equal(sourceFirst.TestOrderId, first.TestOrderId);
            Assert.Equal(sourceFirst.PatientName, first.PatientName);
            Assert.Equal(sourceFirst.Age, first.Age);
            Assert.Equal(sourceFirst.Gender, first.Gender);
            Assert.Equal(sourceFirst.PhoneNumber, first.PhoneNumber);
            Assert.Equal(sourceFirst.Status, first.Status);
            Assert.Equal(sourceFirst.CreatedAt, first.CreatedAt);
            Assert.Equal(sourceFirst.RunDate, first.RunDate);
        }

        [Fact]
        public async Task Handle_ReturnsNoDataMessage_When_Empty()
        {
            // Arrange
            var repoResult = new PagedResult<TestOrder>
            {
                Items = new List<TestOrder>(),
                Total = 0,
                Page = 2,
                PageSize = 10
            };

            _orderRepoMock
                .Setup(r => r.GetTestOrdersAsync(
                    It.IsAny<string?>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<string?>(),
                    It.IsAny<bool>(),
                    It.IsAny<string?>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(repoResult);

            var query = new GetTestOrdersQuery { Page = 2, PageSize = 10 };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.Items);
            Assert.Equal(0, result.Total);
            Assert.Equal(2, result.Page);
            Assert.Equal(10, result.PageSize);
            Assert.Equal("No Data", result.Message);
        }
    }
}


