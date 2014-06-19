namespace Merchello.Core.Gateways.Notification.Monitors
{
    using System.Collections.Generic;
    using Models;
    using Observation;

    /// <summary>
    /// Defines the base NotificationMonitor
    /// </summary>
    public interface INotificationMonitorBase : IMonitor
    {
        /// <summary>
        /// Caches a collection of <see cref="INotificationMessage"/>
        /// </summary>
        /// <param name="message">
        /// The messages to be cached
        /// </param>
        void CacheMessage(IEnumerable<INotificationMessage> message);

        /// <summary>
        /// Caches a single instance of <see cref="INotificationMessage"/>
        /// </summary>
        /// <param name="message">
        /// The message to be cached
        /// </param>
        void CacheMessage(INotificationMessage message);

        /// <summary>
        /// Clears and rebuilds the message cache
        /// </summary>
        void RebuildCache();

        /// <summary>
        /// Removes a single instance of <see cref="INotificationMessage"/>
        /// </summary>
        /// <param name="message">
        /// The message to be removed from cache
        /// </param>
        void RemoveCachedMessage(INotificationMessage message);
    }
}