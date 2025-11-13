using Laboratory_Service.Application.Interface;
using Laboratory_Service.Domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Laboratory_Service.Application.TestOrders.Commands
{
    /// <summary>
    /// Handle Create Test Order
    /// </summary>
    /// <seealso cref="MediatR.IRequestHandler&lt;Laboratory_Service.Application.TestOrders.Commands.CreateTestOrderCommand, System.Guid&gt;" />
    public class CreateTestOrderCommandHandler : IRequestHandler<CreateTestOrderCommand, Guid>
    {
        /// <summary>
        /// The iam client
        /// </summary>
        private readonly IIamUserService _iamClient;
        /// <summary>
        /// The patient repo
        /// </summary>
        private readonly IPatientRepository _patientRepo;
        /// <summary>
        /// The record repo
        /// </summary>
        private readonly IMedicalRecordRepository _recordRepo;
        /// <summary>
        /// The order repo
        /// </summary>
        private readonly ITestOrderRepository _orderRepo;
        /// <summary>
        /// The patient service
        /// </summary>
        private readonly IPatientService _patientService;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<CreateTestOrderCommandHandler> _logger;
        private readonly IEncryptionService _encrypt;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateTestOrderCommandHandler"/> class.
        /// </summary>
        /// <param name="iamClient">The iam client.</param>
        /// <param name="patientRepo">The patient repo.</param>
        /// <param name="recordRepo">The record repo.</param>
        /// <param name="orderRepo">The order repo.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="patientService">The patient service.</param>
        public CreateTestOrderCommandHandler(
            IIamUserService iamClient,
            IPatientRepository patientRepo,
            IMedicalRecordRepository recordRepo,
            ITestOrderRepository orderRepo,
            IEncryptionService encrypt,
            ILogger<CreateTestOrderCommandHandler> logger,
            IPatientService patientService)
        {
            _iamClient = iamClient;
            _patientRepo = patientRepo;
            _recordRepo = recordRepo;
            _orderRepo = orderRepo;
            _logger = logger;
            _encrypt = encrypt;
            _patientService = patientService;
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
        /// User with IdentifyNumber '{request.IdentifyNumber}' does not exist in IAM Service.
        /// or
        /// Unable to create patient from IAM Service data.
        /// </exception>
        public async Task<Guid> Handle(CreateTestOrderCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting Test Order creation for IdentifyNumber: {IdentifyNumber}", request.IdentifyNumber);

            var plainIdentify = request.IdentifyNumber; 
            var encryptedIdentify = _encrypt.Encrypt(plainIdentify);

            var userExists = await _iamClient.CheckUserExistsAsync(plainIdentify);
            if (!userExists)
            {
                _logger.LogWarning("User with IdentifyNumber {IdentifyNumber} not found in IAM Service", request.IdentifyNumber);
                throw new InvalidOperationException($"User with IdentifyNumber '{request.IdentifyNumber}' does not exist in IAM Service.");
            }

            var patient = await _patientRepo.GetByIdentifyNumberAsync(encryptedIdentify);
            if (patient == null)
            {
                _logger.LogInformation("Patient not found in Laboratory DB. Creating from IAM Service...");
                patient = await _patientService.CreatePatientFromUserAsync(plainIdentify, request.CreatedBy.ToString());

                if (patient == null)
                {
                    _logger.LogError("Failed to create patient from IAM user data.");
                    throw new InvalidOperationException("Unable to create patient from IAM Service data.");
                }

                _logger.LogInformation("Successfully created new patient: {PatientId}", patient.PatientId);
            }
            else
            {
                _logger.LogInformation("Patient already exists in Laboratory DB: {PatientId}", patient.PatientId);
            }

            var existingRecords = await _recordRepo.GetByPatientIdAsync(patient.PatientId);

            MedicalRecord medicalRecord;
            if (existingRecords == null || !existingRecords.Any())
            {
                _logger.LogInformation("No medical record found. Creating new record for PatientId: {PatientId}", patient.PatientId);

                medicalRecord = new MedicalRecord
                {
                    PatientId = patient.PatientId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = request.CreatedBy.ToString(),
                    IsDeleted = false
                };

                await _recordRepo.AddAsync(medicalRecord);
                _logger.LogInformation("New Medical Record created with Id: {RecordId}", medicalRecord.MedicalRecordId);
            }
            else
            {

                medicalRecord = existingRecords.First();
                _logger.LogInformation("Using existing Medical Record Id: {RecordId}", medicalRecord.MedicalRecordId);
            }
            var order = new TestOrder
            {
                TestOrderId = Guid.NewGuid(),
                PatientId = patient.PatientId,
                MedicalRecordId = medicalRecord.MedicalRecordId,
                ServicePackageId = request.ServicePackageId,
                OrderCode = $"ORD-{DateTime.UtcNow:yyyyMMddHHmmssfff}-{Guid.NewGuid().ToString("N").Substring(0, 6)}",

                PatientName = patient.FullName,
                DateOfBirth = patient.DateOfBirth,
                Age = DateTime.UtcNow.Year - patient.DateOfBirth.Year,
                Gender = patient.Gender,
                Address = patient.Address,
                PhoneNumber = patient.PhoneNumber,
                Email = patient.Email,

                TestType = string.Empty,
                TestResults = string.Empty,
                RunBy = null,
                RunDate = DateTime.MinValue,
                Priority = request.Priority,
                Note = request.Note,
                Status = "Created",

                CreatedAt = DateTime.UtcNow,
                CreatedBy = request.CreatedBy
            };

            await _orderRepo.AddAsync(order, cancellationToken);

            _logger.LogInformation("Successfully created Test Order {OrderId} for Patient {PatientId}", order.TestOrderId, patient.PatientId);

            return order.TestOrderId;
        }
    }
}
