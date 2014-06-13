using System.Collections.Generic;
using Merchello.Core.Models;

namespace Merchello.Core.Gateways.Notification.Monitors
{
    /// <summary>
    /// Defines the base NotificationMonitor
    /// </summary>
    public interface INotificationMonitorBase
    {
        /// <summary>
        /// Caches a collection of <see cref="INotificationMessage"/>
        /// </summary>
        void CacheMessage(IEnumerable<INotificationMessage> message);

        /// <summary>
        /// Caches a single instance of <see cref="INotificationMessage"/>
        /// </summary>
        void CacheMessage(INotificationMessage message);

        /// <summary>
        /// Clears and rebuilds the message cache
        /// </summary>
        void RebuildCache();

        /// <summary>
        /// Removes a single instance of <see cref="INotificationMessage"/>
        /// </summary>
        void RemoveCachedMessage(INotificationMessage message);
    }
}