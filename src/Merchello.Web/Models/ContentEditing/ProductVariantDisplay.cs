using System.Collections.Generic;
using Merchello.Core.Models;

namespace Merchello.Web.Models.ContentEditing
{
    public class ProductVariantDisplay : ProductDisplayBase
    {
        public IEnumerable<ProductAttributeDisplay> Attributes { get; set; }
    }
}