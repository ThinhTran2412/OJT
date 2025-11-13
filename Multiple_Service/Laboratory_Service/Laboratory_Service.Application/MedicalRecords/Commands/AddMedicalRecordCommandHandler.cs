using Laboratory_Service.Application.DTOs.MedicalRecords;
using Laboratory_Service.Application.Interface;
using Laboratory_Service.Application.MedicalRecords.Commands;
using Laboratory_Service.Domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;

/// <summary>
/// Handle for AddMedicalRecordCommandHandler
/// </summary>
public class AddMedicalRecordCommandHandler : IRequestHandler<AddMedicalRecordCommand, CreateMedicalRecordResultDto>
{
    /// <summary>
    /// The medical record repository
    /// </summary>
    private readonly IMedicalRecordRepository _medicalRecordRepository;
    /// <summary>
    /// The patient repository
    /// </summary>
    private readonly IPatientRepository _patientRepository;
    /// <summary>
    /// The logger
    /// </summary>
    private readonly ILogger<AddMedicalRecordCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AddMedicalRecordCommandHandler"/> class.
    /// </summary>
    /// <param name="medicalRecordRepository">The medical record repository.</param>
    /// <param name="patientRepository">The patient repository.</param>
    /// <param name="logger">The logger.</param>
    public AddMedicalRecordCommandHandler(
        IMedicalRecordRepository medicalRecordRepository,
        IPatientRepository patientRepository,
        ILogger<AddMedicalRecordCommandHandler> logger)
    {
        _medicalRecordRepository = medicalRecordRepository;
        _patientRepository = patientRepository;
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
    /// <exception cref="System.Collections.Generic.KeyNotFoundException">Patient not found</exception>
    public async Task<CreateMedicalRecordResultDto> Handle(AddMedicalRecordCommand request, CancellationToken cancellationToken)
    {

        var patient = await _patientRepository.GetByIdAsync(request.PatientId);
        if (patient == null)
            throw new KeyNotFoundException("Patient not found");

        var medicalRecord = new MedicalRecord
        {
            PatientId = patient.PatientId,
            CreatedBy = request.CreatedBy,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        await _medicalRecordRepository.AddAsync(medicalRecord);

        _logger.LogInformation("Medical record created for PatientId {PatientId} by {CreatedBy}",
            patient.PatientId, request.CreatedBy);

        return new CreateMedicalRecordResultDto
        {
            Success = true,
            Message = "Medical record created successfully."
        };
    }
}
