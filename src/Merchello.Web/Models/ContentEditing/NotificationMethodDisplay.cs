using System;
using System.Collections.Generic;

namespace Merchello.Web.Models.ContentEditing
{
    public class NotificationMethodDisplay
    {
        public Guid Key { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid ProviderKey { get; set; }
        public string ServiceCode { get; set; }

        public IEnumerable<NotificationMessageDisplay> Messages { get; set; } 
    }
}