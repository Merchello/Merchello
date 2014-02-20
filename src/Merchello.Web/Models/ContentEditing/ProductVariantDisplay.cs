using System;
using System.Collections.Generic;
using Merchello.Core.Models;

namespace Merchello.Web.Models.ContentEditing
{
    public class ProductVariantDisplay : ProductDisplayBase
    {
        public Guid Key { get; set; }

        public Guid ProductKey { get; set; }

        public int TotalInventoryCount { get; set; }

        public IEnumerable<ProductAttributeDisplay> Attributes { get; set; }
    }
}