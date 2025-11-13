using AutoMapper;
using Laboratory_Service.Application.Interface;
using Laboratory_Service.Application.TestOrders.Queries;
using Laboratory_Service.Domain.Entity;
using Moq;

namespace Laboratory_Service.Application.UnitTest.TestOrders.Query
{
    public class GetTestOrderDetailQueryHandlerTests
    {
        private readonly Mock<ITestOrderRepository> _orderRepoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetTestOrderDetailQueryHandler _handler;

        public GetTestOrderDetailQueryHandlerTests()
        {
            _orderRepoMock = new Mock<ITestOrderRepository>();
            _mapperMock = new Mock<IMapper>();
            _handler = new GetTestOrderDetailQueryHandler(_orderRepoMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsDto_When_OrderExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            var createdAt = DateTime.UtcNow.AddDays(-1);
            var runDate = DateTime.UtcNow;

            var patient = new Patient
            {
                PatientId = 10,
                IdentifyNumber = "CCCD-123456789",
                FullName = "John Doe",
                DateOfBirth = new DateOnly(1990, 1, 1),
                Gender = "Male",
                PhoneNumber = "0900000000"
            };

            var order = new TestOrder
            {
                TestOrderId = id,
                PatientId = patient.PatientId,
                Patient = patient,
                PatientName = "John Doe",
                Age = 34,
                Gender = "Male",
                PhoneNumber = "0900000000",
                Status = "Completed",
                CreatedAt = createdAt,
                RunDate = runDate,
                TestType = "Blood"
            };

            _orderRepoMock
                .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            var query = new GetTestOrderDetailQuery(id);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(order.TestOrderId, result!.TestOrderId);
            Assert.Equal(order.PatientId, result.PatientId);
            Assert.Equal(order.PatientName, result.PatientName);
            Assert.Equal(order.Age, result.Age);
            Assert.Equal(order.Gender, result.Gender);
            Assert.Equal(order.PhoneNumber, result.PhoneNumber);
            Assert.Equal(order.Patient.IdentifyNumber, result.IdentifyNumber);
            Assert.Equal(order.Status, result.Status);
            Assert.Equal(order.CreatedAt, result.CreatedAt);
            Assert.Equal(order.RunDate, result.RunDate);
            Assert.Equal(order.TestType, result.TestType);
        }

        [Fact]
        public async Task Handle_ReturnsNull_When_OrderNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            _orderRepoMock
                .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((TestOrder?)null);

            var query = new GetTestOrderDetailQuery(id);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Null(result);
        }
    }
}


