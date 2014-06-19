namespace Merchello.Core.Gateways.Notification.Triggering
{
    using System.Collections.Generic;
    using Observation;
    using Umbraco.Core.Logging;

    /// <summary>
    /// Defines the <see cref="NotificationTriggerBase{TInputModel, TMonitorMode}"/>
    /// </summary>
    /// <typeparam name="TInputModel">The type passed to the trigger</typeparam>
    /// <typeparam name="TMonitorModel">The type of the monitor</typeparam>
    public abstract class NotificationTriggerBase<TInputModel, TMonitorModel> : TriggerBase<TMonitorModel>, INotificationTrigger
    {       
        /// <summary>
        /// Value to pass to the notification monitors
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        public virtual void Notify(object model)
        {
            Notify(model, new string[] { });
        }

        /// <summary>
        /// Value to pass to the notification monitors with addtional contacts not defined in notification message (ex. an instance specific customer or vender)
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="contacts">
        /// The contacts.
        /// </param>
        public virtual void Notify(object model, IEnumerable<string> contacts)
        {
            // check to see if the model passed is the correct type or null
            if (WillWork<TInputModel>(model))
            {
                Notify((TInputModel)model, contacts);
                return;
            }

            LogHelper.Debug<NotificationTriggerBase<TInputModel, TMonitorModel>>(string.Format("Model passed to NotificationTriggerBase {0} does not match expected model {1}.  Notification trigger was skipped.", model.GetType(), typeof(TInputModel)));
        }

        /// <summary>
        /// Value to pass to the notification monitors with addtional contacts not defined in notification message (ex. an instance specific customer or vender)
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="contacts">
        /// The contacts.
        /// </param>
        protected abstract void Notify(TInputModel model, IEnumerable<string> contacts);
    }
}