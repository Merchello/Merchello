using System;

namespace Merchello.Web.Models.ContentEditing
{
    public class NotificationMessageDisplay
    {
		public Guid Key { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string Message { get; set; }
		public int MaxLength { get; set; }
		public bool MessageIsFilePath { get; set; }
		public Guid? TriggerKey { get; set; }
		public Guid MethodKey { get; set; }
		public string Recipients { get; set; }
		public bool SendToCustomer { get; set; }
		public bool Disabled { get; set; }
    }
}