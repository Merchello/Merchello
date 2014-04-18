using System;
using System.Collections.Generic;
using Merchello.Core.Models;

namespace Merchello.Web.Models.ContentEditing
{
    public class GatewayProviderDisplay : DialogEditorDisplayBase
    {
        public Guid Key { get; set; }
        public Guid ProviderTfKey { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public IEnumerable<KeyValuePair<string, string>> ExtendedData { get; set; }
        public bool EncryptExtendedData { get; set; }
        public bool Activated { get; set; }        
    }
}
