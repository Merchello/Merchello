using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core.ObjectResolution;

namespace Merchello.Core.Observation
{
    /// <summary>
    /// Represents a MonitorResolver
    /// </summary>
    internal sealed class MonitorResolver : MerchelloManyObjectsResolverBase<MonitorResolver, IMonitor>, IMonitorResolver
    {
        private static readonly ConcurrentDictionary<Type, IMonitor> MonitorCache = new ConcurrentDictionary<Type, IMonitor>();

        public MonitorResolver(IEnumerable<Type> value) 
            : base(value)
        { }

        /// <summary>
        /// Gets the collection of all resovled <see cref="IMonitor"/>s
        /// </summary>
        public IEnumerable<T> GetAllMonitors<T>()
        {
            return GetAllMonitors()
                .Where(x => x.GetType().IsAssignableFrom(typeof (T))).Select(x => (T) x);
        }

        /// <summary>
        /// Gets the collection of all resovled <see cref="IMonitor"/>s
        /// </summary>
        public IEnumerable<IMonitor> GetAllMonitors()
        {
            return Values;
        }

        /// <summary>
        /// Gets a <see cref="IMonitor"/> from the resolver
        /// </summary>
        public T GetMonitor<T>(Type type)
        {
            throw new NotImplementedException();
        }

        public T GetMonitorByKey<T>(Guid key)
        {
            throw new NotImplementedException();
        }

        public IMonitor GetMonitorByKey(Guid key)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IMonitor> GetMonitorsForTrigger<T>()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the instantiated values of the resolved types
        /// </summary>
        protected override IEnumerable<IMonitor> Values
        {
            get
            {
                var ctrArgs = new object[] { };
                var monitors = new List<IMonitor>();

                foreach (var et in InstanceTypes)
                {
                    var attempt = ActivatorHelper.CreateInstance<IMonitor>(et, ctrArgs);
                    if (attempt.Success) monitors.Add(attempt.Result);
                }

                return monitors;
            }
        }
    }
}