namespace Merchello.Core.Models
{
    using System;

    /// <summary>
    /// The notification extensions.
    /// </summary>
    public static class NotificationExtensions
    {
        /// <summary>
        /// Performs a member wise clone of a notification message
        /// </summary>
        /// <param name="message">
        /// The message to be cloned.
        /// </param>
        /// <returns>
        /// The <see cref="INotificationMessage"/>.
        /// </returns>
        public static INotificationMessage MemberwiseClone(this INotificationMessage message)
        {
            ////http://issues.merchello.com/youtrack/issue/M-591
            return new NotificationMessage(message.MethodKey, message.Name, message.FromAddress)
                {
                    Key = message.Key,
                    BodyText = message.BodyText,
                    Recipients = message.Recipients,
                    BodyTextIsFilePath = message.BodyTextIsFilePath,
                    Description = message.Description,
                    Disabled = message.Disabled,
                    MaxLength = message.MaxLength,
                    MonitorKey = message.MonitorKey,
                    ReplyTo = message.ReplyTo,
                    SendToCustomer = message.SendToCustomer,
                    CreateDate = message.CreateDate,
                    UpdateDate = message.UpdateDate
                };
        }
    }
}