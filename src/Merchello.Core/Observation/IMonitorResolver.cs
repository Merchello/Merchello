using System;
using System.Collections.Generic;

namespace Merchello.Core.Observation
{
    /// <summary>
    /// Defines the MonitorResolver
    /// </summary>
    internal interface IMonitorResolver
    {
        /// <summary>
        /// Gets the collection of all resovled <see cref="IMonitor"/>s
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="IMonitor"/></typeparam>
        IEnumerable<T> GetAllMonitors<T>();

        /// <summary>
        /// Gets the collection of all resovled <see cref="IMonitor"/>s
        /// </summary>
        IEnumerable<IMonitor> GetAllMonitors(); 
            
        /// <summary>
        /// Gets a <see cref="IMonitor"/> from the resolver
        /// </summary>
        /// <returns>A <see cref="IMonitor"/></returns>
        T GetMonitor<T>(Type type);

        /// <summary>
        /// Get's a <see cref="IMonitor"/> by it's attribute Key
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="IMonitor"/></typeparam>
        /// <param name="key">The key from the <see cref="MonitorForAttribute"/> (Guid)</param>
        /// <returns>A <see cref="IMonitor"/> of T</returns>
        T GetMonitorByKey<T>(Guid key);

        /// <summary>
        /// Get's a <see cref="IMonitor"/> by it's attribute Key
        /// </summary>
        /// <param name="key">The key from the <see cref="MonitorForAttribute"/> (Guid)</param>
        /// <returns>A <see cref="IMonitor"/> of T</returns>
        IMonitor GetMonitorByKey(Guid key);

        /// <summary>
        /// Gets a collection of all monitors for a particular observable
        /// </summary>
        /// <typeparam name="T">The Type of the Trigger</typeparam>
        IEnumerable<IMonitor> GetMonitorsForTrigger<T>();
    }
}