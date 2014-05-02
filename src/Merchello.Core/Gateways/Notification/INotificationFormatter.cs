namespace Merchello.Core.Gateways.Notification
{
    public interface INotificationFormatter
    {
        /// <summary>
        /// Formats a message
        /// </summary>
        /// <returns>A formatted string</returns>
        string Format(string message);
    }
}