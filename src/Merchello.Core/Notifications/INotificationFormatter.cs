namespace Merchello.Core.Notifications
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