namespace Merchello.Core.Gateways.Notification.Formatters
{
    /// <summary>
    /// Defines the NotificationFormatter
    /// </summary>
    public interface INotificationFormatter
    {
        /// <summary>
        /// Formats a message
        /// </summary>
        /// <returns>A formatted string</returns>
        string Format(string message);
    }
}