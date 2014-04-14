using System;
using Merchello.Core.Models;

namespace Merchello.Web.Models.ContentEditing
{
    public class PaymentMethodDisplay : DialogEditorDisplayBase
    {
        public Guid Key { get; set; }
        public Guid ProviderKey { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PaymentCode { get; set; }        
    }
}