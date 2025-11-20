using Laboratory_Service.Application.Interface;
using Laboratory_Service.Domain.Entity;
using MediatR;

namespace Laboratory_Service.Application.AiReviewForTestOrder.Command
{
    /// <summary>
    /// Create TriggerAiReviewCommandHandler
    /// </summary>
    /// <seealso cref="MediatR.IRequestHandler&lt;Laboratory_Service.Application.AiReviewForTestOrder.Command.TriggerAiReviewCommand, Laboratory_Service.Domain.Entity.TestOrder&gt;" />
    public class TriggerAiReviewCommandHandler : IRequestHandler<TriggerAiReviewCommand, TestOrder?>
    {
        /// <summary>
        /// The test order repository
        /// </summary>
        private readonly ITestOrderRepository _testOrderRepository;
        /// <summary>
        /// The test result repository
        /// </summary>
        private readonly ITestResultRepository _testResultRepository;
        /// <summary>
        /// The ai service
        /// </summary>
        private readonly IAiReviewService _aiService;

        /// <summary>
        /// Initializes a new instance of the <see cref="TriggerAiReviewCommandHandler"/> class.
        /// </summary>
        /// <param name="testOrderRepository">The test order repository.</param>
        /// <param name="testResultRepository">The test result repository.</param>
        /// <param name="aiService">The ai service.</param>
        public TriggerAiReviewCommandHandler(
            ITestOrderRepository testOrderRepository,
            ITestResultRepository testResultRepository,
            IAiReviewService aiService)
        {
            _testOrderRepository = testOrderRepository;
            _testResultRepository = testResultRepository;
            _aiService = aiService;
        }

        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Response from the request
        /// </returns>
        /// <exception cref="System.InvalidOperationException">
        /// AI review feature is not enabled for this test order.
        /// or
        /// Test order has no results to review.
        /// or
        /// No training data available. Cannot perform AI review.
        /// </exception>
        public async Task<TestOrder?> Handle(TriggerAiReviewCommand request, CancellationToken cancellationToken)
        {
            // Query TestOrder separately to avoid SQL issues with Include
            var testOrder = await _testOrderRepository.GetByIdForUpdateAsync(request.TestOrderId, cancellationToken);
            if (testOrder == null)
            {
                return null;
            }

            // AC01 + AC02: check bằng thuộc tính trực tiếp
            if (!testOrder.IsAiReviewEnabled)
            {
                throw new InvalidOperationException("AI review feature is not enabled for this test order.");
            }

            // Query TestResults separately to avoid SQL issues with Include
            var testResults = await _testResultRepository.GetByTestOrderIdAsync(request.TestOrderId, cancellationToken);
            if (testResults == null || !testResults.Any())
            {
                throw new InvalidOperationException("Test order has no results to review.");
            }
            
            // Assign TestResults to TestOrder for processing
            testOrder.TestResults = testResults;

            var allTrainingResults = await _testResultRepository.GetTrainingDatasetAsync(cancellationToken);
            
            if (!allTrainingResults.Any())
            {
                throw new InvalidOperationException("No training data available. Cannot perform AI review.");
            }

            // Train AI model
            await _aiService.TrainModelAsync(allTrainingResults);

            // Predict results
            foreach (var result in testOrder.TestResults)
            {
                var predictedStatus = await _aiService.PredictAsync(result);
                result.ResultStatus = predictedStatus;

                // AC04: Mark as reviewed by AI
                result.ReviewedByAI = true;
                result.AiReviewedDate = DateTime.UtcNow;
            }

            // Save results
            await _testResultRepository.UpdateRangeAsync(testOrder.TestResults, cancellationToken);

            // Update TestOrder status using lightweight method to avoid SQL issues
            await _testOrderRepository.UpdateStatusAsync(testOrder.TestOrderId, "Reviewed By AI", cancellationToken);
            
            // Update local entity status for return value
            testOrder.Status = "Reviewed By AI";
            
            // Reload TestResults from database to ensure we have the latest data and avoid duplicates
            var updatedResults = await _testResultRepository.GetByTestOrderIdAsync(request.TestOrderId, cancellationToken);
            testOrder.TestResults = updatedResults ?? new List<TestResult>();

            return testOrder;
        }

    }
}
