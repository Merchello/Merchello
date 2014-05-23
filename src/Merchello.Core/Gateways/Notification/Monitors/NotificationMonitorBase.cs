using Merchello.Core.Observation;

namespace Merchello.Core.Gateways.Notification.Monitors
{
    /// <summary>
    /// Defines a <see cref="NotificationMonitorBase{T}"/> base class
    /// </summary>
    public abstract class NotificationMonitorBase<T> : MonitorBase<T>
    {
        private readonly INotificationContext _notificationContext;

        protected NotificationMonitorBase(INotificationContext notificationContext)
        {
            Mandate.ParameterNotNull(notificationContext, "notificationContext");
            _notificationContext = notificationContext;
        }

        /// <summary>
        /// Gets the <see cref="INotificationContext"/>
        /// </summary>
        protected INotificationContext NotificationContext
        {
            get { return _notificationContext; }
        }
    }
}