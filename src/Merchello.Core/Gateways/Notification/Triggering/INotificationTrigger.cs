using System.Collections.Generic;
using Merchello.Core.Observation;

namespace Merchello.Core.Gateways.Notification.Triggering
{
    public interface INotificationTrigger : ITrigger
    {
        /// <summary>
        /// Value to pass to the notification monitors
        /// </summary>
        void Notify(object model);

        /// <summary>
        /// Value to pass to the notification monitors
        /// </summary>
        void Notify(object model, IEnumerable<string> contacts);
    }
}