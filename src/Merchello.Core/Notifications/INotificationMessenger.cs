namespace Merchello.Core.Notifications
{
    /// <summary>
    /// Defines a notification messenger - a class responsible for sending notifications
    /// </summary>
    public interface INotificationMessenger
    {
        /// <summary>
        /// Sends the message
        /// </summary>
        /// <returns></returns>
        bool Send();

    }
}