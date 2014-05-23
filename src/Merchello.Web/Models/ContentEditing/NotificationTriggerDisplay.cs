using System;

namespace Merchello.Web.Models.ContentEditing
{
    public class NotificationTriggerDisplay
    {
        public Guid TriggerKey { get; set; }
        public Guid MonitorKey { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
    }
}