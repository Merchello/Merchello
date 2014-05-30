using Umbraco.Core;

namespace Merchello.Core.Observation
{
    internal static class ObservationExtensions
    {
        /// <summary>
        /// Gets the <see cref="TriggerForAttribute"/>
        /// </summary>
        internal static TriggerForAttribute TriggerFor(this ITrigger trigger)
        {
            return trigger.GetType().GetCustomAttribute<TriggerForAttribute>(false);
        }

        /// <summary>
        /// Gets the <see cref="MonitorForAttribute"/>
        /// </summary>        
        internal static MonitorForAttribute MonitorFor(this IMonitor monitor)
        {
            return monitor.GetType().GetCustomAttribute<MonitorForAttribute>(false);
        }        

    }
}