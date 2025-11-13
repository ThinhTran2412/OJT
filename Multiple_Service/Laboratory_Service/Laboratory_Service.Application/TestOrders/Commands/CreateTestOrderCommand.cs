using System.Text.Json.Serialization;
using MediatR;

namespace Laboratory_Service.Application.TestOrders.Commands
{
    /// <summary>
    /// Command to create a new laboratory test order for a patient.
    /// </summary>
    public class CreateTestOrderCommand : IRequest<Guid>
    {
        /// <summary>
        /// The patient's identification number (linked to IAM Service).
        /// </summary>
        public string IdentifyNumber { get; set; } = string.Empty;

        /// <summary>
        /// The ID of the selected service package.
        /// </summary>
        public Guid? ServicePackageId { get; set; }

        /// <summary>
        /// Priority level of the test order (Normal, Urgent, etc.)
        /// </summary>
        public string Priority { get; set; } = "Normal";

        /// <summary>
        /// Additional notes for the test order.
        /// </summary>
        public string? Note { get; set; }

        /// <summary>
        /// The ID of the user (doctor/lab staff) who created the order.
        /// </summary>
        [JsonIgnore]
        public int CreatedBy { get; set; }
    }
}
