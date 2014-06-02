using System;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Models;
using Merchello.Core.Observation;
using Umbraco.Core;
using Umbraco.Core.Logging;

namespace Merchello.Core.Gateways.Notification.Monitors
{
    /// <summary>
    /// Defines a <see cref="NotificationMonitorBase{T}"/> base class
    /// </summary>
    public abstract class NotificationMonitorBase<T> : MonitorBase<T>
    {
        private readonly INotificationContext _notificationContext;

        private Lazy<List<INotificationMessage>> _messages;

        protected NotificationMonitorBase(INotificationContext notificationContext)
        {
            Mandate.ParameterNotNull(notificationContext, "notificationContext");
            _notificationContext = notificationContext;

            Initialize();
        }

        private void Initialize()
        {
            if (_messages == null)
                _messages = new Lazy<List<INotificationMessage>>(BuidCache);
        }

        private List<INotificationMessage> BuidCache()
        {
            try
            {
                var key = GetType().GetCustomAttribute<MonitorForAttribute>(false).Key;
                return ((NotificationContext)_notificationContext).GetNotificationMessagesByMonitorKey(key).ToList();
            }
            catch (Exception ex)
            {
                LogHelper.Error<NotificationMonitorBase<T>>("Failed Building Cache", ex);
                throw;
            }
            
        }

        /// <summary>
        /// Caches a collection of <see cref="INotificationMessage"/>
        /// </summary>
        public virtual void CacheMessage(IEnumerable<INotificationMessage> messages)
        {
            messages.ForEach(CacheMessage);
        }

        /// <summary>
        /// Caches a single instance of <see cref="INotificationMessage"/>
        /// </summary>
        public virtual void CacheMessage(INotificationMessage message)
        {
            if (_messages.Value.All(x => x.Key == message.Key))
            {
                RemoveCachedMessage(message);
            }
            _messages.Value.Add(message);
            
        }

        /// <summary>
        /// Clears the message cache
        /// </summary>
        public virtual void RebuildCache()
        {
            _messages = new Lazy<List<INotificationMessage>>(BuidCache);
        }

        /// <summary>
        /// Removes a single instance of <see cref="INotificationMessage"/>
        /// </summary>
        public virtual void RemoveCachedMessage(INotificationMessage message)
        {
            if (_messages.Value.Contains(message))
                _messages.Value.Remove(message);
        }

        protected IEnumerable<INotificationMessage> Messages
        {
            get { return _messages.Value; }
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