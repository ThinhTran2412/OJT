namespace Laboratory_Service.Application.DTOs.TestOrders
{
    /// <summary>
    /// 
    /// </summary>
    public class TestResultDto
    {
        /// <summary>
        /// Gets or sets the test result identifier.
        /// </summary>
        /// <value>
        /// The test result identifier.
        /// </value>
        public int TestResultId { get; set; }
        /// <summary>
        /// Gets or sets the test code.
        /// </summary>
        /// <value>
        /// The test code.
        /// </value>
        public string TestCode { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the parameter.
        /// </summary>
        /// <value>
        /// The parameter.
        /// </value>
        public string Parameter { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the value numeric.
        /// </summary>
        /// <value>
        /// The value numeric.
        /// </value>
        public double? ValueNumeric { get; set; }
        /// <summary>
        /// Gets or sets the value text.
        /// </summary>
        /// <value>
        /// The value text.
        /// </value>
        public string? ValueText { get; set; }
        /// <summary>
        /// Gets or sets the unit.
        /// </summary>
        /// <value>
        /// The unit.
        /// </value>
        public string Unit { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the reference range.
        /// </summary>
        /// <value>
        /// The reference range.
        /// </value>
        public string ReferenceRange { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the instrument.
        /// </summary>
        /// <value>
        /// The instrument.
        /// </value>
        public string Instrument { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the performed date.
        /// </summary>
        /// <value>
        /// The performed date.
        /// </value>
        public DateTime PerformedDate { get; set; }
        /// <summary>
        /// Gets or sets the result status.
        /// </summary>
        /// <value>
        /// The result status.
        /// </value>
        public string ResultStatus { get; set; } = string.Empty; // Completed | Reviewed
        /// <summary>
        /// Gets or sets the performed by user identifier.
        /// </summary>
        /// <value>
        /// The performed by user identifier.
        /// </value>
        public int? PerformedByUserId { get; set; }
        /// <summary>
        /// Gets or sets the reviewed by user identifier.
        /// </summary>
        /// <value>
        /// The reviewed by user identifier.
        /// </value>
        public int? ReviewedByUserId { get; set; }
        /// <summary>
        /// Gets or sets the reviewed date.
        /// </summary>
        /// <value>
        /// The reviewed date.
        /// </value>
        public DateTime? ReviewedDate { get; set; }
    }
}
