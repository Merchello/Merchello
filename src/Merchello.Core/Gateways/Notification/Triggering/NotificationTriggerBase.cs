using System.Collections.Generic;
using Merchello.Core.Observation;

namespace Merchello.Core.Gateways.Notification.Triggering
{
    /// <summary>
    /// Defines the <see cref="NotificationTriggerBase{TTrigger, TMonitor}"/>
    /// </summary>
    /// <typeparam name="TTrigger">The type passed to the trigger</typeparam>
    /// <typeparam name="TMonitor">The type of the monitor</typeparam>
    public abstract class NotificationTriggerBase<TTrigger, TMonitor> : ObservableTriggerBase<TMonitor>
    {
        /// <summary>
        /// Value to pass to the notification monitors
        /// </summary>
        public virtual void Notify(TTrigger model)
        {
            Notify(model, new string[]{});
        }

        /// <summary>
        /// Value to pass to the notification monitors
        /// </summary>
        public abstract void Notify(TTrigger model, IEnumerable<string> contacts);
    }

}