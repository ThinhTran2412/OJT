using MediatR;
using Laboratory_Service.Application.DTOs.MedicalRecords;

namespace Laboratory_Service.Application.MedicalRecords.Commands
{
    /// <summary>
    /// Create attribute for AddMedicalRecordCommand
    /// </summary>
    /// <seealso cref="MediatR.IRequest&lt;Laboratory_Service.Application.DTOs.MedicalRecords.CreateMedicalRecordResultDto&gt;" />
    public class AddMedicalRecordCommand : IRequest<CreateMedicalRecordResultDto>
    {
        /// <summary>
        /// Gets or sets the patient identifier.
        /// </summary>
        /// <value>
        /// The patient identifier.
        /// </value>
        public int PatientId { get; set; }
        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        /// <value>
        /// The created by.
        /// </value>
        public string CreatedBy { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets a value indicating whether [create iam user].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [create iam user]; otherwise, <c>false</c>.
        /// </value>
        public bool CreateIAMUser { get; set; } = false; 
    }
}
