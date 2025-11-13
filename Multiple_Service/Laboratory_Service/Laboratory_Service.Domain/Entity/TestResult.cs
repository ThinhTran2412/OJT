namespace Laboratory_Service.Domain.Entity
{
    public class TestResult
    {
        /// <summary>
        /// Unique identifier for the test result.
        /// </summary>
        public int TestResultId { get; set; }

        /// <summary>
        /// Foreign key to the associated test order.
        /// </summary>
        public Guid TestOrderId { get; set; }

        /// <summary>
        /// Navigation to the parent test order.
        /// </summary>
        public TestOrder TestOrder { get; set; } = null!;

        /// <summary>
        /// Standardized test code (e.g., LOINC code) if available.
        /// </summary>
        public string TestCode { get; set; } = string.Empty;

        /// <summary>
        /// Parameter/Analyte name within the test (e.g., Hemoglobin for CBC panel).
        /// </summary>
        public string Parameter { get; set; } = string.Empty;

        /// <summary>
        /// Numeric value of the result (if applicable).
        /// </summary>
        public double? ValueNumeric { get; set; }

        /// <summary>
        /// Text value of the result (for non-numeric or qualitative results).
        /// </summary>
        public string? ValueText { get; set; }

        /// <summary>
        /// Unit of measurement for the result (e.g., g/dL, mmol/L).
        /// </summary>
        public string Unit { get; set; } = string.Empty;

        /// <summary>
        /// Reference range for the result (display string).
        /// </summary>
        public string ReferenceRange { get; set; } = string.Empty;

        /// <summary>
        /// Instrument name used to perform the test.
        /// </summary>
        public string Instrument { get; set; } = string.Empty;

        /// <summary>
        /// Datetime when the measurement was performed.
        /// </summary>
        public DateTime PerformedDate { get; set; }

        /// <summary>
        /// Status of the result: Completed | Reviewed.
        /// </summary>
        public string ResultStatus { get; set; } = string.Empty;

        /// <summary>
        /// UserId of performer.
        /// </summary>
        public int? PerformedByUserId { get; set; }

        /// <summary>
        /// UserId of reviewer (if reviewed).
        /// </summary>
        public int? ReviewedByUserId { get; set; }

        /// <summary>
        /// Review datetime (if reviewed).
        /// </summary>
        public DateTime? ReviewedDate { get; set; }
    }
}
