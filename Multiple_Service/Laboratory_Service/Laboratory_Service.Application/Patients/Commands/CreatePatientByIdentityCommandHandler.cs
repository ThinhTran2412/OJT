using AutoMapper;
using Laboratory_Service.Application.DTOs.Patients;
using Laboratory_Service.Application.Interface;
using Laboratory_Service.Domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Laboratory_Service.Application.Patients.Commands
{
    /// <summary>
    /// Handle for CreatePatientByIdentityCommandHandler
    /// </summary>
    /// <seealso cref="MediatR.IRequestHandler&lt;Laboratory_Service.Application.Patients.Commands.CreatePatientByIdentityCommand, Laboratory_Service.Application.DTOs.Patients.PatientDto&gt;" />
    public class CreatePatientByIdentityCommandHandler : IRequestHandler<CreatePatientByIdentityCommand, PatientDto>
    {
        /// <summary>
        /// The patient repository
        /// </summary>
        private readonly IPatientRepository _patientRepository;
        /// <summary>
        /// The iam service
        /// </summary>
        private readonly IIAMService _iamService;
        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger _logger;
        /// <summary>
        /// The encrypt
        /// </summary>
        private readonly IEncryptionService _encrypt;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreatePatientByIdentityCommandHandler"/> class.
        /// </summary>
        /// <param name="patientRepository">The patient repository.</param>
        /// <param name="iamService">The iam service.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="encrypt">The encrypt.</param>
        /// <param name="logger">The logger.</param>
        public CreatePatientByIdentityCommandHandler(
            IPatientRepository patientRepository,
            IIAMService iamService,
            IMapper mapper,
            IEncryptionService encrypt,
            ILogger<CreatePatientByIdentityCommandHandler> logger)
        {
            _patientRepository = patientRepository;
            _iamService = iamService;
            _mapper = mapper;
            _encrypt = encrypt; 
            _logger = logger;
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
        /// Patient with this identification number already exists
        /// or
        /// User not found in IAM Service
        /// </exception>
        public async Task<PatientDto> Handle(CreatePatientByIdentityCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var plainIdentifyNumber = request.IdentifyNumber;


                var encryptedIdentify = _encrypt.Encrypt(plainIdentifyNumber);

                var existingPatient = await _patientRepository.GetByIdentifyNumberAsync(encryptedIdentify);
                if (existingPatient != null)
                    throw new InvalidOperationException("Patient with this identification number already exists");

                var userInfo = await _iamService.GetUserByIdentifyNumberAsync(plainIdentifyNumber);
                if (userInfo == null)
                    throw new InvalidOperationException("User not found in IAM Service");

                var patient = new Patient
                {
                    IdentifyNumber = encryptedIdentify,
                    FullName = userInfo.FullName,
                    DateOfBirth = userInfo.DateOfBirth,
                    Gender = userInfo.Gender,
                    PhoneNumber = userInfo.PhoneNumber ?? string.Empty,
                    Email = userInfo.Email ?? string.Empty,
                    Address = userInfo.Address ?? string.Empty,

                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                    UpdatedAt = null,

                };

                var createdPatient = await _patientRepository.AddAsync(patient);

                return _mapper.Map<PatientDto>(createdPatient);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating patient for IdentifyNumber {IdentifyNumber}", request.IdentifyNumber);

                throw;
            }
        }
    }
}
