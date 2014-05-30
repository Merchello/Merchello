namespace Merchello.Core.Observation
{
    /// <summary>
    /// Marker interface for Observable triggers
    /// </summary>
    public interface ITrigger
    {
        /// <summary>
        /// True / false indicating whether or not this trigger is being monitored
        /// </summary>
        bool HasMonitors { get; }

        /// <summary>
        /// Returns the count of monitors
        /// </summary>
        int MonitorCount { get; }
    }
}