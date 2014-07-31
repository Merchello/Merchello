namespace Merchello.Plugin.Taxation.Avalara.Models
{
    /// <summary>
    /// Defines an API Response Message.
    /// </summary>
    public interface IApiResponseMessage
    {
        /// <summary>
        /// Gets or sets the message summary.
        /// </summary>
        string Summary { get; set; }

        /// <summary>
        /// Gets or sets the detailed message body.
        /// </summary>
        string Details { get; set; }

        /// <summary>
        /// Gets or sets a value indicating what the message refers to.
        /// </summary>
        string RefersTo { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the response severity.
        /// </summary>
        SeverityLevel Severity { get; set; }

        /// <summary>
        /// Gets or sets a value identifying the message's source.
        /// </summary>
        string Source { get; set; } 
    }
}