using System;
using Merchello.Core.Models;

namespace Merchello.Web.Models.ContentEditing
{
    public class GatewayProviderDisplay
    {
        public Guid Key { get; set; }
        public Guid ProviderTfKey { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ExtendedDataCollection ExtendedData { get; set; }
        public bool EncryptExtendedData { get; set; }
        public bool Activated { get; set; }
        public DialogEditorViewDisplay DialogEditorView { get; set; }
    }
}
