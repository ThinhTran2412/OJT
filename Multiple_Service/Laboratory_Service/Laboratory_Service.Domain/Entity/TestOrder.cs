using System.ComponentModel.DataAnnotations;

namespace Laboratory_Service.Domain.Entity
{
    /// <summary>
    /// Create attribyte for class TestOrder
    /// </summary>
    public class TestOrder
    {
        /// <summary>
        /// Gets or sets the test order identifier.
        /// </summary>
        /// <value>
        /// The test order identifier.
        /// </value>
        public Guid TestOrderId { get; set; }

        /// <summary>
        /// Gets or sets the patient identifier.
        /// </summary>
        /// <value>
        /// The patient identifier.
        /// </value>
        public int PatientId { get; set; }
        /// <summary>
        /// Gets or sets the patient.
        /// </summary>
        /// <value>
        /// The patient.
        /// </value>
        public virtual Patient Patient { get; set; } = null!;
        /// <summary>
        /// Gets or sets the service package identifier.
        /// </summary>
        /// <value>
        /// The service package identifier.
        /// </value>
        public Guid? ServicePackageId { get; set; }

        /// <summary>
        /// Gets or sets the order code.
        /// </summary>
        /// <value>
        /// The order code.
        /// </value>
        public string OrderCode { get; set; } = default!;
        /// <summary>
        /// Gets or sets the priority.
        /// </summary>
        /// <value>
        /// The priority.
        /// </value>
        public string Priority { get; set; } = "Normal";
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public string Status { get; set; } = "Created";
        /// <summary>
        /// Gets or sets the note.
        /// </summary>
        /// <value>
        /// The note.
        /// </value>
        public string? Note { get; set; }

        /// <summary>
        /// Gets or sets the name of the patient.
        /// </summary>
        /// <value>
        /// The name of the patient.
        /// </value>
        public string PatientName { get; set; } = default!;
        /// <summary>
        /// Gets or sets the date of birth.
        /// </summary>
        /// <value>
        /// The date of birth.
        /// </value>
        public DateOnly DateOfBirth { get; set; }
        /// <summary>
        /// Gets or sets the age.
        /// </summary>
        /// <value>
        /// The age.
        /// </value>
        public int Age { get; set; }
        /// <summary>
        /// Gets or sets the gender.
        /// </summary>
        /// <value>
        /// The gender.
        /// </value>
        public string Gender { get; set; } = default!;
        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        /// <value>
        /// The address.
        /// </value>
        public string Address { get; set; } = default!;
        /// <summary>
        /// Gets or sets the phone number.
        /// </summary>
        /// <value>
        /// The phone number.
        /// </value>
        public string PhoneNumber { get; set; } = default!;
        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        public string? Email { get; set; }

        /// <summary>
        /// Gets or sets the created at.
        /// </summary>
        /// <value>
        /// The created at.
        /// </value>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        /// <value>
        /// The created by.
        /// </value>
        public int CreatedBy { get; set; }
        /// <summary>
        /// Gets or sets the updated at.
        /// </summary>
        /// <value>
        /// The updated at.
        /// </value>
        public DateTime? UpdatedAt { get; set; }
        /// <summary>
        /// Gets or sets the updated by.
        /// </summary>
        /// <value>
        /// The updated by.
        /// </value>
        public int? UpdatedBy { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is deleted.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is deleted; otherwise, <c>false</c>.
        /// </value>
        public bool IsDeleted { get; set; } = false;
        /// <summary>
        /// Gets or sets the deleted at.
        /// </summary>
        /// <value>
        /// The deleted at.
        /// </value>
        public DateTime? DeletedAt { get; set; }
        /// <summary>
        /// Gets or sets the deleted by.
        /// </summary>
        /// <value>
        /// The deleted by.
        /// </value>
        public int? DeletedBy { get; set; }


        /// <summary>
        /// Gets or sets the type of the test.
        /// </summary>
        /// <value>
        /// The type of the test.
        /// </value>
        public string TestType { get; set; } = string.Empty;

        /// <summary>
        /// Test results with values
        /// </summary>
        /// <value>
        /// The test results.
        /// </value>

        public string TestResults { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the medical record identifier.
        /// </summary>
        /// <value>
        /// The medical record identifier.
        /// </value>
        public int MedicalRecordId { get; set; }

        /// <summary>
        /// Navigation property to the medical record
        /// </summary>
        /// <value>
        /// The medical record.
        /// </value>
        public virtual MedicalRecord MedicalRecord { get; set; } = null!;
        /// <summary>
        /// User who performed the test
        /// </summary>
        /// <value>
        /// The run by.
        /// </value>
        public int? RunBy { get; set; }

        /// <summary>
        /// Gets or sets the run date.
        /// </summary>
        /// <value>
        /// The run date.
        /// </value>
        public DateTime RunDate { get; set; }
    }
}
