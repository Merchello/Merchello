namespace Merchello.Core.Gateways.Notification.Formatters
{
    /// <summary>
    /// Represents the default notification formatter
    /// </summary>
    public class DefaultNotificationFormatter : INotificationFormatter
    {
        public string Format(string message)
        {
            return message;
        }
    }
}