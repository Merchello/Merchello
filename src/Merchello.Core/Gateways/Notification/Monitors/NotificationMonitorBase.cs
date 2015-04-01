namespace Merchello.Core.Gateways.Notification.Monitors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Events;

    using Models;
    using Observation;
    using Umbraco.Core;
    using Umbraco.Core.Events;
    using Umbraco.Core.Logging;

    /// <summary>
    /// Defines a <see cref="NotificationMonitorBase{TFormatter, TModel}"/> base class
    /// </summary>
    /// <typeparam name="T">
    /// The Type of the model passed to the monitor
    /// </typeparam>
    public abstract class NotificationMonitorBase<T> : MonitorBase<T>, INotificationMonitorBase
    {
        /// <summary>
        /// The notification context.
        /// </summary>
        private readonly INotificationContext _notificationContext;

        /// <summary>
        /// The _messages.
        /// </summary>
        private Lazy<List<INotificationMessage>> _messages;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationMonitorBase{T}"/> class. 
        /// </summary>
        /// <param name="notificationContext">
        /// The notification context.
        /// </param>
        protected NotificationMonitorBase(INotificationContext notificationContext)
        {
            Mandate.ParameterNotNull(notificationContext, "notificationContext");
            _notificationContext = notificationContext;

            Initialize();
        }

        /// <summary>
        /// Gets the cached collection of <see cref="INotificationMessage"/>
        /// </summary>
        protected IEnumerable<INotificationMessage> Messages
        {
            get
            {
                ////http://issues.merchello.com/youtrack/issue/M-591
                return _messages.Value.Select(x => x.MemberwiseClone());
            }
        }

        /// <summary>
        /// Gets the <see cref="INotificationContext"/>
        /// </summary>
        protected INotificationContext NotificationContext
        {
            get { return _notificationContext; }
        }

        /// <summary>
        /// Caches a collection of <see cref="INotificationMessage"/>
        /// </summary>
        /// <param name="messages">
        /// A collection of <see cref="INotificationMessage"/> to be cached
        /// </param>
        public virtual void CacheMessage(IEnumerable<INotificationMessage> messages)
        {
            messages.ForEach(CacheMessage);
        }

        /// <summary>
        /// Caches a single instance of <see cref="INotificationMessage"/>
        /// </summary>
        /// <param name="message">
        /// The <see cref="INotificationMessage"/> to be cached
        /// </param>
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
            _messages = new Lazy<List<INotificationMessage>>(BuidMessageCache);
        }

        /// <summary>
        /// Removes a single instance of <see cref="INotificationMessage"/>
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public virtual void RemoveCachedMessage(INotificationMessage message)
        {
            if (_messages.Value.Contains(message))
                _messages.Value.Remove(message);
        }
        

        /// <summary>
        /// Object initialization helper
        /// </summary>
        private void Initialize()
        {
            if (_messages == null)
                _messages = new Lazy<List<INotificationMessage>>(BuidMessageCache);
        }

        /// <summary>
        /// Method used in Lazy collection instantiation of <see cref="INotificationMessage"/>
        /// </summary>
        /// <returns>
        /// A collection of <see cref="INotificationMessage"/>
        /// </returns>
        private List<INotificationMessage> BuidMessageCache()
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
    }
}