namespace Merchello.Web.Workflow.Notification.Monitor
{
    using System.Linq;

    using Merchello.Core.Gateways.Notification;
    using Merchello.Core.Gateways.Notification.Monitors;
    using Merchello.Core.Models.MonitorModels;

    /// <summary>
    /// A base class for razor based monitors.
    /// </summary>
    /// <typeparam name="TModel">
    /// The type of view model
    /// </typeparam>
    public abstract class RazorMonitorBase<TModel> : NotificationMonitorBase<TModel>
        where TModel : INotifyModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RazorMonitorBase{TModel}"/> class.
        /// </summary>
        /// <param name="notificationContext">
        /// The notification context.
        /// </param>
        protected RazorMonitorBase(INotificationContext notificationContext)
            : base(notificationContext)
        {
        }

        /// <summary>
        /// Overrides the base monitor execute based off the notifying trigger.
        /// </summary>
        /// <param name="value">
        /// The model to be used by the monitor
        /// </param>
        public override void OnNext(TModel value)
        {
            if (!Messages.Any()) return;

            var formatter = new RazorFormatter(value);

            foreach (var message in Messages)
            {
                if (value.Contacts.Any() && message.SendToCustomer)
                {
                    // add the additional contacts to the recipients list
                    if (!message.Recipients.EndsWith(";"))
                        message.Recipients += ";";

                    if (message.Recipients[0] == ';')
                        message.Recipients = message.Recipients.TrimStart(';');

                    message.Recipients = string.Format("{0}{1}", message.Recipients, string.Join(";", value.Contacts));
                }

                // send the message
                NotificationContext.Send(message, formatter);
            }
        }
    }
}