using System;
using System.Collections.Generic;

namespace Merchello.Web.Models.ContentEditing
{
    public class ProductOptionDisplay
    {
        public Guid Key { get; set; }
        public string Name { get; set; }
        public bool Required { get; set; }
        public int SortOrder { get; set; }

        public IEnumerable<ProductAttributeDisplay> Choices { get; set; }

    }
}
