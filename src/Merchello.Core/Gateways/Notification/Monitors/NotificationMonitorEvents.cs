namespace Merchello.Core.Gateways.Notification.Monitors
{
    using System.Linq;
    using Models;
    using Observation;
    using Services;
    using Umbraco.Core;
    using Umbraco.Core.Events;
    using Umbraco.Core.Logging;

    /// <summary>
    /// The notification monitor events.
    /// </summary>
    public class NotificationMonitorEvents : ApplicationEventHandler
    {
        /// <summary>
        /// The application started.
        /// </summary>
        /// <param name="umbracoApplication">
        /// The umbraco application.
        /// </param>
        /// <param name="applicationContext">
        /// The application context.
        /// </param>
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            base.ApplicationStarted(umbracoApplication, applicationContext);

            LogHelper.Info<NotificationMonitorEvents>("Initializing Merchello NotificationMonitor binding events");

            NotificationMessageService.Saved += NotificationMessageServiceOnSaved;
            LogHelper.Info<NotificationMonitorEvents>("Completed Merchello NotificationMonitor binding events");
        }

        /// <summary>
        /// The notification message service on saved.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="saveEventArgs">
        /// The save event args.
        /// </param>
        private void NotificationMessageServiceOnSaved(INotificationMessageService sender, SaveEventArgs<INotificationMessage> saveEventArgs)
        {
            // TODO target this a bit better
            if (!MonitorResolver.HasCurrent) LogHelper.Info<NotificationMonitorEvents>("MonitorResolver singleton has not be set");

            var monitors = MonitorResolver.Current.GetAllMonitors().Where(x => x is INotificationMonitorBase);

            foreach (var monitor in monitors)
            {
                // TODO - this will need to be a distributed call
                ((INotificationMonitorBase)monitor).RebuildCache();
            }
        }
    }
}