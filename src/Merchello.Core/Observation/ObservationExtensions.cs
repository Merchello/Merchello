namespace Merchello.Core.Observation
{
    using Umbraco.Core;

    internal static class ObservationExtensions
    {
        /// <summary>
        /// Gets the <see cref="TriggerForAttribute"/>
        /// </summary>
        /// <param name="trigger">
        /// The trigger.
        /// </param>
        /// <returns>
        /// The <see cref="TriggerForAttribute"/>.
        /// </returns>
        internal static TriggerForAttribute TriggerFor(this ITrigger trigger)
        {
            return trigger.GetType().GetCustomAttribute<TriggerForAttribute>(false);
        }

        /// <summary>
        /// Gets the <see cref="MonitorForAttribute"/>
        /// </summary>
        /// <param name="monitor">
        /// The monitor.
        /// </param>
        /// <returns>
        /// The <see cref="MonitorForAttribute"/>.
        /// </returns>
        internal static MonitorForAttribute MonitorFor(this IMonitor monitor)
        {
            return monitor.GetType().GetCustomAttribute<MonitorForAttribute>(false);
        }        

    }
}