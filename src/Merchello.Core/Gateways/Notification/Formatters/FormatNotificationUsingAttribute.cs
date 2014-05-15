using System;
using Merchello.Core.Models;

namespace Merchello.Core.Gateways.Notification.Formatters
{
    /// <summary>
    /// An attribute used to decorate <see cref="INotificationMessage"/> that can be used to change the default message formatting behavior
    /// </summary>
    public class FormatNotificationUsingAttribute : Attribute
    {
        public Type Formatter { get; private set; }

        public FormatNotificationUsingAttribute(Type formatter)
        {
            if(!typeof(INotificationFormatter).IsAssignableFrom(formatter)) throw new Exception("Only allowed to associate a NotificationFormatter types");
            Formatter = formatter;
        }

    }
}