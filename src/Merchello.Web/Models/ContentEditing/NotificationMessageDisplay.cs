using System;

namespace Merchello.Web.Models.ContentEditing
{
    public class NotificationMessageDisplay
    {
		public Guid Key { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
        public string FromAddress { get; set; }
        public string ReplyTo { get; set; }
		public string BodyText { get; set; }
		public int MaxLength { get; set; }
		public bool BodyTextIsFilePath { get; set; }
        public Guid? MonitorKey { get; set; }
		public Guid MethodKey { get; set; }
		public string Recipients { get; set; }
		public bool SendToCustomer { get; set; }
		public bool Disabled { get; set; }
    }
}