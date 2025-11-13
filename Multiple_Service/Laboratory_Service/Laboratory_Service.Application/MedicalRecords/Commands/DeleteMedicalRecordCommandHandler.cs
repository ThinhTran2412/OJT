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
    /// Handle for DeleteMedicalRecord
    /// </summary>
    /// <seealso cref="MediatR.IRequestHandler&lt;Laboratory_Service.Application.MedicalRecords.Commands.DeleteMedicalRecordCommand, Laboratory_Service.Application.DTOs.MedicalRecords.DeleteMedicalRecordResultDto&gt;" />
    public class DeleteMedicalRecordCommandHandler : IRequestHandler<DeleteMedicalRecordCommand, DeleteMedicalRecordResultDto>
    {
        /// <summary>
        /// The medical record repository
        /// </summary>
        private readonly IMedicalRecordRepository _medicalRecordRepository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<DeleteMedicalRecordCommandHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteMedicalRecordCommandHandler"/> class.
        /// </summary>
        /// <param name="medicalRecordRepository">The medical record repository.</param>
        /// <param name="logger">The logger.</param>
        public DeleteMedicalRecordCommandHandler(
            IMedicalRecordRepository medicalRecordRepository,
            ILogger<DeleteMedicalRecordCommandHandler> logger)
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
        /// <exception cref="System.InvalidOperationException">Cannot delete medical record while there are pending or ongoing test orders.</exception>
        public async Task<DeleteMedicalRecordResultDto> Handle(DeleteMedicalRecordCommand request, CancellationToken cancellationToken)
        {
            var existing = await _medicalRecordRepository.GetByIdAsync(request.MedicalRecordId);
            if (existing == null)
            {
                _logger.LogWarning("Delete failed. MedicalRecord {Id} not found", request.MedicalRecordId);
                throw new KeyNotFoundException("Medical record not found");
            }

            if (existing.IsDeleted)
            {
                return new DeleteMedicalRecordResultDto
                {
                    Success = false,
                    Message = "Medical record is already deleted."
                };
            }
            try
            {
                var patient = existing.Patient;
                if (patient?.TestOrders != null)
                {
                    // adjust status values according to your TestOrder.Status domain
                    var hasPending = patient.TestOrders.Any(o =>
                        string.Equals(o.Status, "Pending", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(o.Status, "InProgress", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(o.Status, "Ongoing", StringComparison.OrdinalIgnoreCase)
                    );

                    if (hasPending)
                    {
                        throw new InvalidOperationException("Cannot delete medical record while there are pending or ongoing test orders.");
                    }
                }
            }
            catch (Exception ex) when (ex is NullReferenceException)
            {
            }

            var snapshotObj = new
            {
                existing.MedicalRecordId,
                existing.PatientId,
                existing.PatientName,
                existing.TestType,
                existing.TestResults,
                existing.ReferenceRanges,
                existing.Interpretation,
                existing.InstrumentUsed,
                existing.BatchNumber,
                existing.LotNumber,
                existing.ClinicalNotes,
                existing.ErrorMessages,
                existing.TestDate,
                existing.ResultsDate,
                existing.Status,
                existing.Priority,
                existing.CreatedAt,
                existing.CreatedBy,
                existing.UpdatedAt,
                existing.UpdatedBy,
                existing.IsDeleted
            };

            var jsonOptions = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = false,
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            };

            var snapshotJson = JsonSerializer.Serialize(snapshotObj, jsonOptions);
            var history = new MedicalRecordHistory
            {
                MedicalRecordId = existing.MedicalRecordId,
                SnapshotJson = snapshotJson,
                ChangeSummary = $"Record marked as deleted by {request.DeletedBy}",
                ChangedBy = string.IsNullOrEmpty(request.DeletedBy) ? "unknown" : request.DeletedBy,
                ChangedAt = DateTime.UtcNow
            };
            existing.IsDeleted = true;
            existing.UpdatedAt = DateTime.UtcNow;
            existing.UpdatedBy = request.DeletedBy ?? existing.UpdatedBy;
            await _medicalRecordRepository.AddHistoryAsync(history);
            await _medicalRecordRepository.UpdateAsync(existing);

            _logger.LogInformation("MedicalRecord {Id} soft-deleted by {User}", existing.MedicalRecordId, history.ChangedBy);

            return new DeleteMedicalRecordResultDto
            {
                Success = true,
                Message = "Medical record soft-deleted successfully."
            };
        }
    }
}