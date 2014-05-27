using System;
using System.Collections.Generic;

namespace Merchello.Core.Observation
{
    public interface IMonitorResolver
    {
        /// <summary>
        /// Gets the collection of all resovled <see cref="IMonitor"/>s
        /// </summary>
        IEnumerable<T> GetAllMonitors<T>();

        /// <summary>
        /// Gets a <see cref="IMonitor"/> from the resolver
        /// </summary>
        /// <returns>A <see cref="IMonitor"/></returns>
        T TryGetMonitor<T>(Type type);
    }
}