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

        // TODO - ATM, this field is manually setup in the ApiController 
        // rather than through AutoMapper because presently we do not have an
        // elegant way to query for the messages.
        public IEnumerable<NotificationMessageDisplay> Messages { get; set; } 
    }
}