namespace Merchello.Core.Gateways.Notification.Triggering
{
    using System.Collections.Generic;
    using Observation;

    /// <summary>
    /// Defines a notification trigger
    /// </summary>
    public interface INotificationTrigger : ITrigger
    {
        /// <summary>
        /// Value to pass to the notification monitors
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        void Notify(object model);

        /// <summary>
        /// Value to pass to the notification monitors
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="contacts">
        /// The contacts.
        /// </param>
        void Notify(object model, IEnumerable<string> contacts);
    }
}