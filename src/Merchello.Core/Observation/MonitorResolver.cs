namespace Merchello.Core.Observation
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using Gateways.Notification;
    using ObjectResolution;
    using Umbraco.Core;

    /// <summary>
    /// Represents a MonitorResolver
    /// </summary>
    internal sealed class MonitorResolver : MerchelloManyObjectsResolverBase<MonitorResolver, IMonitor>, IMonitorResolver
    {
        private static readonly ConcurrentDictionary<Guid, IMonitor> MonitorCache = new ConcurrentDictionary<Guid, IMonitor>();
        private readonly INotificationContext _notificationContext;

        public MonitorResolver(INotificationContext notificationContext, IEnumerable<Type> value)
            : base(value)
        {
            Mandate.ParameterNotNull(notificationContext, "notificationContext");
            _notificationContext = notificationContext;

            BuildCache();
        }

        /// <summary>
        /// Gets the instantiated values of the resolved types
        /// </summary>
        protected override IEnumerable<IMonitor> Values
        {
            get
            {
                var ctrArgs = new object[] { _notificationContext };

                var monitors = new List<IMonitor>();

// ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var et in InstanceTypes)
                {
                    var attempt = ActivatorHelper.CreateInstance<IMonitor>(et, ctrArgs);
                    if (attempt.Success) monitors.Add(attempt.Result);
                }

                return monitors;
            }
        }

        /// <summary>
        /// Gets the collection of all resovled <see cref="IMonitor"/>s
        /// </summary>
        /// <typeparam name="T">
        /// The type of monitor to resolve
        /// </typeparam>
        /// <returns>
        /// The collection of monitors.
        /// </returns>
        public IEnumerable<T> GetAllMonitors<T>()
        {
            return GetAllMonitors()
                .Where(x => x.GetType().IsAssignableFrom(typeof(T))).Select(x => (T)x);
        }

        /// <summary>
        /// Gets the collection of all resovled <see cref="IMonitor"/>s
        /// </summary>
        /// <returns>
        /// A collection of <see cref="IMonitor"/>
        /// </returns>
        public IEnumerable<IMonitor> GetAllMonitors()
        {
            return MonitorCache.Values;
        }

        /// <summary>
        /// Gets a <see cref="IMonitor"/> from the resolver
        /// </summary>
        /// <typeparam name="T">
        /// The type of monitor to resolve
        /// </typeparam>
        /// <returns>
        /// The collection of monitors resolved
        /// </returns>
        public IEnumerable<T> GetMonitors<T>()
        {
            return GetAllMonitors().Where(x => x.GetType().IsAssignableFrom(typeof(T))).Select(x => (T)x);
        }

        /// <summary>
        /// Get's a <see cref="IMonitor"/> by it's attribute Key
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="IMonitor"/></typeparam>
        /// <param name="key">The key from the <see cref="MonitorForAttribute"/> (Guid)</param>
        /// <returns>A <see cref="IMonitor"/> of T</returns>
        public T GetMonitorByKey<T>(Guid key)
        {
            return (T)GetMonitorByKey(key);
        }

        /// <summary>
        /// Get's a <see cref="IMonitor"/> by it's attribute Key
        /// </summary>
        /// <param name="key">The key from the <see cref="MonitorForAttribute"/> (Guid)</param>
        /// <returns>A <see cref="IMonitor"/> of T</returns>
        public IMonitor GetMonitorByKey(Guid key)
        {
            return MonitorCache[key];
        }

        /// <summary>
        /// Gets a collection of all monitors for a particular observable trigger
        /// </summary>
        /// <param name="triggerType">
        /// The Type of the Trigger
        /// </param>
        /// <returns>
        /// The collection of monitors resolved
        /// </returns>
        public IEnumerable<IMonitor> GetMonitorsForTrigger(Type triggerType)
        {
            return MonitorCache.Values.Where(
                    x => x.GetType().GetCustomAttribute<MonitorForAttribute>(false)
                        .ObservableTrigger == triggerType);
        }

        /// <summary>
        /// Gets a collection of all monitors for a particular observable trigger
        /// </summary>
        /// <typeparam name="T">
        /// The Type of the Trigger
        /// </typeparam>
        /// <returns>
        /// The collection of monitors resolved
        /// </returns>
        public IEnumerable<IMonitor> GetMonitorsForTrigger<T>()
        {
            return GetMonitorsForTrigger(typeof(T));
        }

        private void BuildCache()
        {
            foreach (var monitor in Values)
            {
                MonitorCache.AddOrUpdate(monitor.MonitorFor().Key, monitor, (x, y) => monitor);
            }
        }
    }
}