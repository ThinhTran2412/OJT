using System.ComponentModel.DataAnnotations;

namespace Laboratory_Service.Domain.Entity
{
    /// <summary>
    /// Represents a medical record for a patient
    /// </summary>
    public class MedicalRecord
    {
        /// <summary>
        /// Unique identifier for the medical record
        /// </summary>
        /// <value>
        /// The medical record identifier.
        /// </value>
        public int MedicalRecordId { get; set; }

        /// <summary>
        /// Foreign key to the patient (Database relationship)
        /// </summary>
        /// <value>
        /// The patient identifier.
        /// </value>
        [Required]
        public int PatientId { get; set; }

        /// <summary>
        /// Navigation property to the patient (Database relationship)
        /// Giao tiếp TRỰC TIẾP qua Entity Framework, không qua proto
        /// </summary>
        /// <value>
        /// The patient.
        /// </value>
        public virtual Patient Patient { get; set; } = null!;
        /// <summary>
        /// Gets or sets the name of the patient.
        /// </summary>
        /// <value>
        /// The name of the patient.
        /// </value>
        public string PatientName { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the type of the test.
        /// </summary>
        /// <value>
        /// The type of the test.
        /// </value>
        public string TestType { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the test results.
        /// </summary>
        /// <value>
        /// The test results.
        /// </value>
        public string TestResults { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the reference ranges.
        /// </summary>
        /// <value>
        /// The reference ranges.
        /// </value>
        public string ReferenceRanges { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the interpretation.
        /// </summary>
        /// <value>
        /// The interpretation.
        /// </value>
        public string Interpretation { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the instrument used.
        /// </summary>
        /// <value>
        /// The instrument used.
        /// </value>
        public string InstrumentUsed { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the batch number.
        /// </summary>
        /// <value>
        /// The batch number.
        /// </value>
        public string BatchNumber { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the lot number.
        /// </summary>
        /// <value>
        /// The lot number.
        /// </value>
        public string LotNumber { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the clinical notes.
        /// </summary>
        /// <value>
        /// The clinical notes.
        /// </value>
        public string ClinicalNotes { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the error messages.
        /// </summary>
        /// <value>
        /// The error messages.
        /// </value>
        public string ErrorMessages { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the test date.
        /// </summary>
        /// <value>
        /// The test date.
        /// </value>
        public DateTime TestDate { get; set; }
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
        public string Status { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the priority.
        /// </summary>
        /// <value>
        /// The priority.
        /// </value>
        public string Priority { get; set; } = string.Empty;


        /// <summary>
        /// Date when the record was created
        /// </summary>
        /// <value>
        /// The created at.
        /// </value>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Date when the record was last updated
        /// </summary>
        /// <value>
        /// The updated at.
        /// </value>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// User who created the medical record
        /// </summary>
        /// <value>
        /// The created by.
        /// </value>
        public string CreatedBy { get; set; } = string.Empty;

        /// <summary>
        /// User who last updated the medical record
        /// </summary>
        /// <value>
        /// The updated by.
        /// </value>
        public string UpdatedBy { get; set; } = string.Empty;

        /// <summary>
        /// Indicates if the record is active or archived (soft delete)
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is deleted; otherwise, <c>false</c>.
        /// </value>
        public bool IsDeleted { get; set; } = false;
    }
}
