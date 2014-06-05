namespace Merchello.Core.Formatters
{
    /// <summary>
    /// Defines the NotificationFormatter
    /// </summary>
    public interface IFormatter
    {
        /// <summary>
        /// Formats a message
        /// </summary>
        /// <returns>A formatted string</returns>
        string Format(string value);
    }
}