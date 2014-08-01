namespace Merchello.Plugin.Taxation.Avalara.Models
{
    /// <summary>
    /// Represents an AvaTax API response message.
    /// </summary>
    public class ApiResponseMessage : IApiResponseMessage
    {
        /// <summary>
        /// Gets or sets the message summary.
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// Gets or sets the detailed message body.
        /// </summary>
        public string Details { get; set; }

        /// <summary>
        /// Gets or sets a value indicating what the message refers to.
        /// </summary>
        public string RefersTo { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the response severity.
        /// </summary>
        public SeverityLevel Severity { get; set; }

        /// <summary>
        /// Gets or sets a value identifying the message's source.
        /// </summary>
        public string Source { get; set; }
    }
}