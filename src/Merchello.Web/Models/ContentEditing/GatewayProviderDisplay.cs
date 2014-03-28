using System;

namespace Merchello.Web.Models.ContentEditing
{
    public class GatewayProviderDisplay
    {
        public Guid Key { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool EncryptExtendedData { get; set; }
        public bool Activated { get; set; }
    }
}
