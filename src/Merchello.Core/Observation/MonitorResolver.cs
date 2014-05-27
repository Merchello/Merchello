using System;
using System.Collections.Generic;
using Merchello.Core.ObjectResolution;

namespace Merchello.Core.Observation
{
    /// <summary>
    /// Represents a MonitorResolver
    /// </summary>
    internal sealed class MonitorResolver : MerchelloManyObjectsResolverBase<MonitorResolver, IMonitor>, IMonitorResolver
    {
        public MonitorResolver(IEnumerable<Type> value) 
            : base(value)
        { }

        /// <summary>
        /// Gets the collection of all resovled <see cref="IMonitor"/>s
        /// </summary>
        public IEnumerable<T> GetAllMonitors<T>()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a <see cref="IMonitor"/> from the resolver
        /// </summary>
        public T TryGetMonitor<T>(Type type)
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