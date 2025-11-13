using Laboratory_Service.Application.DTOs.Patients;
using MediatR;

namespace Laboratory_Service.Application.Patients.Commands
{
    /// <summary>
    /// create attribute for CreatePatientByIdentityCommand
    /// </summary>
    /// <seealso cref="MediatR.IRequest&lt;Laboratory_Service.Application.DTOs.Patients.PatientDto&gt;" />
    public class CreatePatientByIdentityCommand : IRequest<PatientDto>
    {
        /// <summary>
        /// Gets or sets the identify number.
        /// </summary>
        /// <value>
        /// The identify number.
        /// </value>
        public string IdentifyNumber { get; set; } = string.Empty;
    }
}
