namespace Laboratory_Service.Application.DTOs.Comment
{
    /// <summary>
    /// 
    /// </summary>
    public class CreateCommentDto
    {
        /// <summary>
        /// Gets or sets the test order identifier.
        /// </summary>
        /// <value>
        /// The test order identifier.
        /// </value>
        public Guid? TestOrderId { get; set; }
        /// <summary>
        /// Gets or sets the test result identifier.
        /// </summary>
        /// <value>
        /// The test result identifier.
        /// </value>
        public int? TestResultId { get; set; }
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; } = string.Empty;
    }
}
