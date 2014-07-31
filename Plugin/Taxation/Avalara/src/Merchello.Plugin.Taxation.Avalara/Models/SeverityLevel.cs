namespace Merchello.Plugin.Taxation.Avalara.Models
{
    /// <summary>
    /// API response severity levels.
    /// </summary>
    public enum SeverityLevel
    {
        /// <summary>
        /// Successful response.
        /// </summary>
        Success,

        /// <summary>
        /// Response returned with warnings.
        /// </summary>
        Warning,

        /// <summary>
        /// Response returned as an error.
        /// </summary>
        Error,

        /// <summary>
        /// An exception was thrown while processing the request.
        /// </summary>
        Exception
    }
}