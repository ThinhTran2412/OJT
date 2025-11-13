using Laboratory_Service.Application.DTOs.MedicalRecords;
using MediatR;
using System;

namespace Laboratory_Service.Application.MedicalRecords.Commands
{
    /// <summary>
    /// Create attribute for UpdateMedicalRecordCommand
    /// </summary>
    /// <seealso cref="MediatR.IRequest&lt;Laboratory_Service.Application.DTOs.MedicalRecords.UpdateMedicalRecordResultDto&gt;" />
    public class UpdateMedicalRecordCommand : IRequest<UpdateMedicalRecordResultDto>
    {
        /// <summary>
        /// Gets or sets the medical record identifier.
        /// </summary>
        /// <value>
        /// The medical record identifier.
        /// </value>
        public int MedicalRecordId { get; set; }
        /// <summary>
        /// Gets or sets the name of the patient.
        /// </summary>
        /// <value>
        /// The name of the patient.
        /// </value>
        public string? PatientName { get; set; }
        /// <summary>
        /// Gets or sets the type of the test.
        /// </summary>
        /// <value>
        /// The type of the test.
        /// </value>
        public string? TestType { get; set; }
        /// <summary>
        /// Gets or sets the test results.
        /// </summary>
        /// <value>
        /// The test results.
        /// </value>
        public string? TestResults { get; set; }
        /// <summary>
        /// Gets or sets the reference ranges.
        /// </summary>
        /// <value>
        /// The reference ranges.
        /// </value>
        public string? ReferenceRanges { get; set; }
        /// <summary>
        /// Gets or sets the interpretation.
        /// </summary>
        /// <value>
        /// The interpretation.
        /// </value>
        public string? Interpretation { get; set; }
        /// <summary>
        /// Gets or sets the instrument used.
        /// </summary>
        /// <value>
        /// The instrument used.
        /// </value>
        public string? InstrumentUsed { get; set; }
        /// <summary>
        /// Gets or sets the batch number.
        /// </summary>
        /// <value>
        /// The batch number.
        /// </value>
        public string? BatchNumber { get; set; }
        /// <summary>
        /// Gets or sets the lot number.
        /// </summary>
        /// <value>
        /// The lot number.
        /// </value>
        public string? LotNumber { get; set; }
        /// <summary>
        /// Gets or sets the clinical notes.
        /// </summary>
        /// <value>
        /// The clinical notes.
        /// </value>
        public string? ClinicalNotes { get; set; }
        /// <summary>
        /// Gets or sets the error messages.
        /// </summary>
        /// <value>
        /// The error messages.
        /// </value>
        public string? ErrorMessages { get; set; }
        /// <summary>
        /// Gets or sets the test date.
        /// </summary>
        /// <value>
        /// The test date.
        /// </value>
        public DateTime? TestDate { get; set; }
        /// <summary>
        /// Gets or sets the results date.
        /// </summary>
        /// <value>
        /// The results date.
        /// </value>
        public DateTime? ResultsDate { get; set; }
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public string? Status { get; set; }
        /// <summary>
        /// Gets or sets the priority.
        /// </summary>
        /// <value>
        /// The priority.
        /// </value>
        public string? Priority { get; set; }
        /// <summary>
        /// Gets or sets the updated by.
        /// </summary>
        /// <value>
        /// The updated by.
        /// </value>
        public string UpdatedBy { get; set; } = string.Empty;
    }
}