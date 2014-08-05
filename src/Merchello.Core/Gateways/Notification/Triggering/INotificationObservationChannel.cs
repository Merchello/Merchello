using System;
using System.Collections.Generic;
using Merchello.Core.Events;
using Merchello.Core.Observation;

namespace Merchello.Core.Gateways.Notification.Triggering
{
    /// <summary>
    /// Defines a NotificationBroadcaster
    /// </summary>
    /// <typeparam name="T">The type of the model to be passed to <see cref="ObservationChannelEventArgs{T}"/></typeparam>
    public interface INotificationObservationChannel<in T> : IObservationChannel
    {
        /// <summary>
        /// Constructs <see cref="ObservationChannelEventArgs{T}"/> with a custom model and raises the event
        /// </summary>
        /// <param name="model">The model to be passed to the monitor</param>
        void Update(object model);

        /// <summary>
        /// Constructs <see cref="ObservationChannelEventArgs{T}"/> with a custom model and raises the event
        /// </summary>
        /// <param name="model">The model to be passed to the monitor</param>
        /// <param name="contacts">Additional contacts to be included in the notification</param>
        void Update(object model, IEnumerable<string> contacts);
    }
}