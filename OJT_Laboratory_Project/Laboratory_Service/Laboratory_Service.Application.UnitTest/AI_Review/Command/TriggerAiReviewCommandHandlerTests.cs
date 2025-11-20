using Xunit;
using Moq;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Laboratory_Service.Application.AiReviewForTestOrder.Command;
using Laboratory_Service.Application.Interface;
using Laboratory_Service.Domain.Entity;

public class TriggerAiReviewCommandHandlerTests
{
    private readonly Mock<ITestOrderRepository> _orderRepo = new();
    private readonly Mock<ITestResultRepository> _resultRepo = new();
    private readonly Mock<IAiReviewService> _aiService = new();

    private TriggerAiReviewCommandHandler CreateHandler()
        => new TriggerAiReviewCommandHandler(
            _orderRepo.Object,
            _resultRepo.Object,
            _aiService.Object);

    // -------------------------------------------------------------
    // 1. TestOrder = null → return null
    // -------------------------------------------------------------
    [Fact]
    public async Task Handle_ReturnsNull_WhenTestOrderNotFound()
    {
        _orderRepo.Setup(x => x.GetByIdWithResultsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync((TestOrder?)null);

        var handler = CreateHandler();
        var result = await handler.Handle(
            new TriggerAiReviewCommand(Guid.NewGuid()),
            CancellationToken.None);

        Assert.Null(result);
    }

    // -------------------------------------------------------------
    // 2. TestOrder exists but AI disabled → throw
    // -------------------------------------------------------------
    [Fact]
    public async Task Handle_Throws_WhenAiReviewDisabled()
    {
        var order = new TestOrder
        {
            IsAiReviewEnabled = false
        };

        _orderRepo.Setup(x => x.GetByIdWithResultsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync(order);

        var handler = CreateHandler();

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.Handle(new TriggerAiReviewCommand(Guid.NewGuid()), CancellationToken.None));

        Assert.Equal("AI review feature is not enabled for this test order.", ex.Message);
    }

    // -------------------------------------------------------------
    // 3. AI enabled nhưng không có TestResults → throw
    // -------------------------------------------------------------
    [Fact]
    public async Task Handle_Throws_WhenNoTestResults()
    {
        var order = new TestOrder
        {
            IsAiReviewEnabled = true,
            TestResults = new List<TestResult>() // empty
        };

        _orderRepo.Setup(x => x.GetByIdWithResultsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync(order);

        var handler = CreateHandler();

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.Handle(new TriggerAiReviewCommand(Guid.NewGuid()), CancellationToken.None));

        Assert.Equal("Test order has no results to review.", ex.Message);
    }

    // -------------------------------------------------------------
    // 4. Không có training dataset → throw
    // -------------------------------------------------------------
    [Fact]
    public async Task Handle_Throws_WhenNoTrainingData()
    {
        var order = new TestOrder
        {
            IsAiReviewEnabled = true,
            TestResults = new List<TestResult>
            {
                new TestResult { TestResultId = 1 }
            }
        };

        _orderRepo.Setup(x => x.GetByIdWithResultsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync(order);

        _resultRepo.Setup(x => x.GetTrainingDatasetAsync(It.IsAny<CancellationToken>()))
                   .ReturnsAsync(new List<TestResult>()); // empty dataset

        var handler = CreateHandler();

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.Handle(new TriggerAiReviewCommand(Guid.NewGuid()), CancellationToken.None));

        Assert.Equal("No training data available. Cannot perform AI review.", ex.Message);
    }

    // -------------------------------------------------------------
    // 5. Success flow → Train → Predict → UpdateRange → UpdateOrder
    // -------------------------------------------------------------
    [Fact]
    public async Task Handle_SuccessfulAiReview_UpdatesAllFields()
    {
        // Fake TestResults
        var result1 = new TestResult { TestResultId = 1 };
        var result2 = new TestResult { TestResultId = 2 };

        var order = new TestOrder
        {
            IsAiReviewEnabled = true,
            TestResults = new List<TestResult> { result1, result2 }
        };

        _orderRepo.Setup(x => x.GetByIdWithResultsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync(order);

        _resultRepo.Setup(x => x.GetTrainingDatasetAsync(It.IsAny<CancellationToken>()))
                   .ReturnsAsync(new List<TestResult> { new TestResult { TestResultId = 999 } });

        _aiService.Setup(x => x.PredictAsync(It.IsAny<TestResult>()))
                  .ReturnsAsync("AI-PREDICTED");

        var handler = CreateHandler();

        var result = await handler.Handle(
            new TriggerAiReviewCommand(Guid.NewGuid()),
            CancellationToken.None);

        // Verify AI train/predict
        _aiService.Verify(x => x.TrainModelAsync(It.IsAny<IEnumerable<TestResult>>()), Times.Once);
        _aiService.Verify(x => x.PredictAsync(It.IsAny<TestResult>()), Times.Exactly(2));

        // Verify repository updates
        _resultRepo.Verify(x => x.UpdateRangeAsync(order.TestResults, It.IsAny<CancellationToken>()), Times.Once);
        _orderRepo.Verify(x => x.UpdateAsync(order), Times.Once);

        // Verify result
        Assert.NotNull(result);
        Assert.Equal("Reviewed By AI", result.Status);

        foreach (var r in result.TestResults)
        {
            Assert.Equal("AI-PREDICTED", r.ResultStatus);
            Assert.True(r.ReviewedByAI);
            Assert.NotNull(r.AiReviewedDate);
        }
    }
}
