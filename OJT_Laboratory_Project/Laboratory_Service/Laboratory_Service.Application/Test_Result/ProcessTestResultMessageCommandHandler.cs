using Grpc.Core;
using Laboratory_Service.Application.DTOs.TestResult;
using Laboratory_Service.Application.Interface;
using Laboratory_Service.Domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Laboratory_Service.Application.Test_Result.Commands
{
    /// <summary>
    /// Handle for ProcessTestResultMessageCommand
    /// </summary>
    /// <seealso cref="MediatR.IRequestHandler&lt;Laboratory_Service.Application.Test_Result.ProcessTestResultMessageCommand, Laboratory_Service.Application.DTOs.TestResult.TestResultIngressResponseDto&gt;" />
    public class ProcessTestResultMessageCommandHandler : IRequestHandler<ProcessTestResultMessageCommand, TestResultIngressResponseDto>
    {
        /// <summary>
        /// The test order repo
        /// </summary>
        private readonly ITestOrderRepository _testOrderRepo;
        /// <summary>
        /// The test result repo
        /// </summary>
        private readonly ITestResultRepository _testResultRepo;
        /// <summary>
        /// The flagging service
        /// </summary>
        private readonly IFlaggingService _flaggingService;
        /// <summary>
        /// The event log service
        /// </summary>
        private readonly IEventLogService _eventLogService;
        /// <summary>
        /// The processed message repo
        /// </summary>
        private readonly IProcessedMessageRepository _processedMessageRepo;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<ProcessTestResultMessageCommandHandler> _logger;
        /// <summary>
        /// The simulator client
        /// </summary>
        private readonly ISimulatorGrpcClient _simulatorClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessTestResultMessageCommandHandler"/> class.
        /// </summary>
        /// <param name="testOrderRepo">The test order repo.</param>
        /// <param name="testResultRepo">The test result repo.</param>
        /// <param name="flaggingService">The flagging service.</param>
        /// <param name="eventLogService">The event log service.</param>
        /// <param name="processedMessageRepo">The processed message repo.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="simulatorClient">The simulator client.</param>
        public ProcessTestResultMessageCommandHandler(
            ITestOrderRepository testOrderRepo,
            ITestResultRepository testResultRepo,
            IFlaggingService flaggingService,
            IEventLogService eventLogService,
            IProcessedMessageRepository processedMessageRepo,
            ILogger<ProcessTestResultMessageCommandHandler> logger,
            ISimulatorGrpcClient simulatorClient)
        {
            _testOrderRepo = testOrderRepo;
            _testResultRepo = testResultRepo;
            _flaggingService = flaggingService;
            _eventLogService = eventLogService;
            _processedMessageRepo = processedMessageRepo;
            _logger = logger;
            _simulatorClient = simulatorClient;
        }

        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Response from the request
        /// </returns>
        public async Task<TestResultIngressResponseDto> Handle(ProcessTestResultMessageCommand request, CancellationToken cancellationToken)
        {
            var sourceSystem = "Simulator_gRPC";
            Guid testOrderId = request.TestOrderId;

            string messageId = $"Temporary_{testOrderId}";

            _logger.LogInformation("START processing via gRPC pull. TestOrderId: {TestOrderId}", testOrderId);

            try
            {
                // STEP 1: Call Simulator via gRPC to generate and retrieve results
                var rawResultDto = await _simulatorClient.CreateAndGetRawResultsAsync(testOrderId);

                if (rawResultDto == null || !rawResultDto.Results.Any())
                {
                    string msg = rawResultDto == null ? "gRPC call returned null." : "No results returned.";
                    _logger.LogWarning("FAILED to retrieve data from Simulator. TestOrderId: {TestOrderId}. Reason: {Reason}", testOrderId, msg);

                    return new TestResultIngressResponseDto { Success = false, Message = $"Failed to retrieve results: {msg}" };
                }

                // STEP 2: Prepare data and create persistent MessageId
                var instrument = rawResultDto.Instrument;
                var performedDate = rawResultDto.PerformedDate;
                var resultsDto = rawResultDto.Results;

                // Create persistent MessageId based on PerformedDate returned from Simulator
                messageId = $"{testOrderId}_{performedDate:yyyyMMddHHmmss}";

                // Skip validation step (already performed on Simulator side, DTO was checked for null)

                // STEP 3: Idempotency check – ensure we do not process duplicated messages
                var isNewMessage = await _processedMessageRepo.TryAddIfNotExistsAsync(
                    messageId,
                    sourceSystem,
                    testOrderId,
                    cancellationToken);

                if (!isNewMessage)
                {
                    var existingMessage = await _processedMessageRepo.GetByMessageIdAsync(messageId, cancellationToken);
                    _logger.LogInformation("Message {MessageId} already processed. Skipping duplicate.", messageId);

                    return new TestResultIngressResponseDto
                    {
                        Success = true,
                        Message = $"Message {messageId} has already been processed (duplicate).",
                        MessageId = messageId,
                        CreatedCount = existingMessage?.CreatedCount ?? 0,
                        ProcessedAt = existingMessage?.ProcessedAt ?? DateTime.UtcNow
                    };
                }

                // Bước 4: Verify TestOrder exists
                var testOrder = await _testOrderRepo.GetByIdAsync(testOrderId, cancellationToken);
                if (testOrder == null)
                {
                    var errorMessage = $"TestOrder with ID {testOrderId} not found.";
                    _logger.LogWarning(errorMessage);
                    // Optional: remove processedMessage entry if you plan to reprocess later

                    return new TestResultIngressResponseDto { Success = false, Message = errorMessage, MessageId = messageId };
                }

                // Retrieve gender from TestOrder
                var gender = testOrder.MedicalRecord?.Patient?.Gender;
                if (string.IsNullOrEmpty(gender))
                {
                    _logger.LogWarning("Gender is null or empty for TestOrder {TestOrderId}.", testOrderId);
                }

                // STEP 5: Transform DTOs to entities and apply flagging
                var testResults = new List<TestResult>();
                var processedAt = DateTime.UtcNow;

                foreach (var resultDto in resultsDto)
                {
                    // STEP 5.1: Calculate flag beforehand based on numeric value and gender
                    var flag = await _flaggingService.CalculateFlagAsync(
                        resultDto.TestCode,
                        resultDto.ValueNumeric,
                        gender,
                        cancellationToken);

                    // STEP 5.2: Create TestResult entity from DTO and attach calculated flag
                    var testResult = new TestResult
                    {
                        TestOrderId = testOrderId,
                        TestCode = resultDto.TestCode,
                        Parameter = resultDto.Parameter,
                        ValueNumeric = resultDto.ValueNumeric,
                        ValueText = resultDto.ValueText,
                        Unit = resultDto.Unit,
                        ReferenceRange = resultDto.ReferenceRange,
                        Instrument = instrument,
                        ResultStatus = "Pending",
                        PerformedDate = performedDate,
                        Flag = flag,
                        FlaggedAt = processedAt,
                        CreatedAt = processedAt,
                        // AI Review fields default to false
                        ReviewedByAI = false,
                        IsConfirmed = false
                    };

                    testResults.Add(testResult);
                }

                // STEP 6: Bulk insert TestResult records
                var createdCount = await _testResultRepo.AddRangeAsync(testResults, cancellationToken);

                // STEP 6.5: Update ProcessedMessage entry with CreatedCount
                var processedMessage = await _processedMessageRepo.GetByMessageIdAsync(messageId, cancellationToken);
                if (processedMessage != null)
                {
                    processedMessage.CreatedCount = createdCount;
                    processedMessage.ProcessedAt = processedAt;
                    await _processedMessageRepo.UpdateAsync(processedMessage, cancellationToken);
                }

                // Bước 7: Log EventLog
                await _eventLogService.CreateAsync(new EventLog
                {
                    EventId = "E_00010",
                    Action = "Process Test Result Message (gRPC)",
                    EventLogMessage = $"Processed test result message {messageId} from {sourceSystem}. Created {createdCount} test results for TestOrder {testOrderId}.",
                    OperatorName = "System",
                    EntityType = "TestResult",
                    EntityId = testOrderId,
                    CreatedOn = processedAt
                });

                _logger.LogInformation("Successfully processed message {MessageId}. Created {Count} test results.", messageId, createdCount);

                return new TestResultIngressResponseDto
                {
                    Success = true,
                    Message = $"Successfully processed message and created {createdCount} test results.",
                    MessageId = messageId,
                    CreatedCount = createdCount,
                    ProcessedAt = processedAt
                };
            }
            catch (RpcException ex)
            {
                // Xử lý lỗi gRPC cụ thể
                _logger.LogError(ex, "gRPC Error while calling Simulator for TestOrderId: {TestOrderId}. Status: {StatusCode}", testOrderId, ex.StatusCode);
                return new TestResultIngressResponseDto { Success = false, Message = $"gRPC Error: {ex.Status.Detail}", ErrorDetails = ex.Message };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "General Error processing test result message. TestOrderId: {TestOrderId}", testOrderId);

                return new TestResultIngressResponseDto
                {
                    Success = false,
                    Message = "An error occurred during processing or gRPC call.",
                    MessageId = messageId,
                    ErrorDetails = ex.Message
                };
            }
        }
    }
}