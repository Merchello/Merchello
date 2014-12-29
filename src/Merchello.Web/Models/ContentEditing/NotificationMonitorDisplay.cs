namespace Merchello.Web.Models.ContentEditing
{
    using System;

    /// <summary>
    /// Represents a NotificationMonitorDisplay
    /// </summary>
    public class NotificationMonitorDisplay
    {
        /// <summary>
        /// Gets or sets the trigger key
        /// </summary>
        public Guid TriggerKey { get; set; }

        /// <summary>
        /// Gets or sets the monitor key
        /// </summary>
        public Guid MonitorKey { get; set; }
        
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Gets or sets the alias
        /// </summary>
        public string Alias { get; set; }
    }
}