using System;

namespace Merchello.Web.Models.ContentEditing
{
    public class ProductAttributeDisplay
    {
        public Guid Key { get; set; }
        public Guid OptionKey { get; set; }
        public string Name { get; set; }
        public string Sku { get; set; }
        public int SortOrder { get; set; }
    }
}
