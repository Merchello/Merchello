using System.Collections.Generic;
using Merchello.Core.Observation;

namespace Merchello.Core.Gateways.Notification.Triggering
{
    /// <summary>
    /// Defines the <see cref="NotificationTriggerBase{T}"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class NotificationTriggerBase<T> : ObservableTriggerBase<T>
    {
        /// <summary>
        /// Value to pass to the notification monitors
        /// </summary>
        public virtual void Notify(T value)
        {
            Notify(value, new string[]{});
        }

        /// <summary>
        /// Value to pass to the notification monitors
        /// </summary>
        public abstract void Notify(T value, IEnumerable<string> contacts);
    }

}