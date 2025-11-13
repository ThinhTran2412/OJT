using System.Text.Json;
using System.Text.Json.Serialization;
using MediatR;
using Microsoft.Extensions.Logging;
using Laboratory_Service.Application.Interface;
using Laboratory_Service.Application.MedicalRecords.Commands;
using Laboratory_Service.Application.DTOs.MedicalRecords;
using Laboratory_Service.Domain.Entity;

namespace Laboratory_Service.Application.MedicalRecords.Commands
{
    /// <summary>
    /// Handle UpdateMedicalRecordCommandHandler
    /// </summary>
    /// <seealso cref="MediatR.IRequestHandler&lt;Laboratory_Service.Application.MedicalRecords.Commands.UpdateMedicalRecordCommand, Laboratory_Service.Application.DTOs.MedicalRecords.UpdateMedicalRecordResultDto&gt;" />
    public class UpdateMedicalRecordCommandHandler : IRequestHandler<UpdateMedicalRecordCommand, UpdateMedicalRecordResultDto>
    {
        /// <summary>
        /// The medical record repository
        /// </summary>
        private readonly IMedicalRecordRepository _medicalRecordRepository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<UpdateMedicalRecordCommandHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateMedicalRecordCommandHandler"/> class.
        /// </summary>
        /// <param name="medicalRecordRepository">The medical record repository.</param>
        /// <param name="logger">The logger.</param>
        public UpdateMedicalRecordCommandHandler(
            IMedicalRecordRepository medicalRecordRepository,
            ILogger<UpdateMedicalRecordCommandHandler> logger)
        {
            _medicalRecordRepository = medicalRecordRepository;
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
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Medical record not found</exception>
        public async Task<UpdateMedicalRecordResultDto> Handle(UpdateMedicalRecordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var existing = await _medicalRecordRepository.GetByIdAsync(request.MedicalRecordId);
                if (existing == null)
                {
                    _logger.LogWarning("MedicalRecord {Id} not found", request.MedicalRecordId);
                    throw new KeyNotFoundException("Medical record not found");
                }

                var changes = new List<string>();
                void AddChange<T>(string name, T? oldValue, T? newValue)
                {
                    if (!object.Equals(oldValue, newValue))
                    {
                        changes.Add($"{name}: '{oldValue?.ToString() ?? "<null>"}' -> '{newValue?.ToString() ?? "<null>"}'");
                    }
                }

                AddChange(nameof(existing.PatientName), existing.PatientName, request.PatientName ?? existing.PatientName);
                AddChange(nameof(existing.TestType), existing.TestType, request.TestType ?? existing.TestType);
                AddChange(nameof(existing.TestResults), existing.TestResults, request.TestResults ?? existing.TestResults);
                AddChange(nameof(existing.ReferenceRanges), existing.ReferenceRanges, request.ReferenceRanges ?? existing.ReferenceRanges);
                AddChange(nameof(existing.Interpretation), existing.Interpretation, request.Interpretation ?? existing.Interpretation);
                AddChange(nameof(existing.InstrumentUsed), existing.InstrumentUsed, request.InstrumentUsed ?? existing.InstrumentUsed);
                AddChange(nameof(existing.BatchNumber), existing.BatchNumber, request.BatchNumber ?? existing.BatchNumber);
                AddChange(nameof(existing.LotNumber), existing.LotNumber, request.LotNumber ?? existing.LotNumber);
                AddChange(nameof(existing.ClinicalNotes), existing.ClinicalNotes, request.ClinicalNotes ?? existing.ClinicalNotes);
                AddChange(nameof(existing.ErrorMessages), existing.ErrorMessages, request.ErrorMessages ?? existing.ErrorMessages);
                AddChange(nameof(existing.TestDate), existing.TestDate, request.TestDate ?? existing.TestDate);
                AddChange(nameof(existing.ResultsDate), existing.ResultsDate, request.ResultsDate ?? existing.ResultsDate);
                AddChange(nameof(existing.Status), existing.Status, request.Status ?? existing.Status);
                AddChange(nameof(existing.Priority), existing.Priority, request.Priority ?? existing.Priority);

                var snapshotObj = new
                {
                    MedicalRecordId = existing.MedicalRecordId,
                    PatientId = existing.PatientId,
                    PatientName = existing.PatientName,
                    TestType = existing.TestType,
                    TestResults = existing.TestResults,
                    ReferenceRanges = existing.ReferenceRanges,
                    Interpretation = existing.Interpretation,
                    InstrumentUsed = existing.InstrumentUsed,
                    BatchNumber = existing.BatchNumber,
                    LotNumber = existing.LotNumber,
                    ClinicalNotes = existing.ClinicalNotes,
                    ErrorMessages = existing.ErrorMessages,
                    TestDate = existing.TestDate,
                    ResultsDate = existing.ResultsDate,
                    Status = existing.Status,
                    Priority = existing.Priority,
                    CreatedAt = existing.CreatedAt,
                    CreatedBy = existing.CreatedBy,
                    UpdatedAt = existing.UpdatedAt,
                    UpdatedBy = existing.UpdatedBy,
                    IsDeleted = existing.IsDeleted
                };

                var jsonOptions = new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                    WriteIndented = false,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles 
                };

                string snapshotJson = JsonSerializer.Serialize(snapshotObj, jsonOptions);

                var history = new MedicalRecordHistory
                {
                    MedicalRecordId = existing.MedicalRecordId,
                    SnapshotJson = snapshotJson,
                    ChangeSummary = changes.Count > 0 ? string.Join("; ", changes) : "No changes detected",
                    ChangedBy = string.IsNullOrEmpty(request.UpdatedBy) ? "unknown" : request.UpdatedBy,
                    ChangedAt = DateTime.UtcNow
                };

                if (request.PatientName is not null) existing.PatientName = request.PatientName;
                if (request.TestType is not null) existing.TestType = request.TestType;
                if (request.TestResults is not null) existing.TestResults = request.TestResults;
                if (request.ReferenceRanges is not null) existing.ReferenceRanges = request.ReferenceRanges;
                if (request.Interpretation is not null) existing.Interpretation = request.Interpretation;
                if (request.InstrumentUsed is not null) existing.InstrumentUsed = request.InstrumentUsed;
                if (request.BatchNumber is not null) existing.BatchNumber = request.BatchNumber;
                if (request.LotNumber is not null) existing.LotNumber = request.LotNumber;
                if (request.ClinicalNotes is not null) existing.ClinicalNotes = request.ClinicalNotes;
                if (request.ErrorMessages is not null) existing.ErrorMessages = request.ErrorMessages;
                if (request.TestDate.HasValue) existing.TestDate = request.TestDate.Value;
                if (request.ResultsDate.HasValue) existing.ResultsDate = request.ResultsDate;
                if (request.Status is not null) existing.Status = request.Status;
                if (request.Priority is not null) existing.Priority = request.Priority;

                existing.UpdatedAt = DateTime.UtcNow;
                existing.UpdatedBy = string.IsNullOrEmpty(request.UpdatedBy) ? existing.UpdatedBy : request.UpdatedBy;

                await _medicalRecordRepository.AddHistoryAsync(history);
                await _medicalRecordRepository.UpdateAsync(existing);

                _logger.LogInformation("MedicalRecord {Id} updated by {User}. Changes: {ChangeSummary}",
                    existing.MedicalRecordId, history.ChangedBy, history.ChangeSummary);

                return new UpdateMedicalRecordResultDto
                {
                    Success = true,
                    Message = "Medical record updated successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating MedicalRecord {Id}", request.MedicalRecordId);
                throw;
            }
        }
    }
}