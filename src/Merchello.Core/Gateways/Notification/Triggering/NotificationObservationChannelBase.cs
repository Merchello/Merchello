using System.Collections.Generic;
using Merchello.Core.Events;

namespace Merchello.Core.Gateways.Notification.Triggering
{
    public abstract class NotificationObservationChannelBase<T> : INotificationObservationChannel<T>
    {
        /// <summary>
        /// Constructs <see cref="ObservationChannelEventArgs{T}"/> with a custom model and raises the event
        /// </summary>
        /// <param name="model">The model to be passed to the monitor</param>
        public virtual void Update(object model)
        {
            Update(model, new LinkedList<string>());
        }

        /// <summary>
        /// Constructs <see cref="ObservationChannelEventArgs{T}"/> with a custom model and raises the event
        /// </summary>
        /// <param name="model">The model to be passed to the monitor</param>
        /// <param name="contacts">Additional contacts to be included in the notification</param>
        public abstract void Update(object model, IEnumerable<string> contacts);

    }
}