using System.Collections.Generic;
using Merchello.Core.Models;

namespace Merchello.Web.Models.ContentEditing
{
    public class ProductVariantDisplay : ProductDisplayBase
    {
        public IEnumerable<IWarehouseInventory> WarehouseInventory { get; set; } // not in lucene
        public IEnumerable<IProductAttribute> Attributes { get; set; }
    }
}