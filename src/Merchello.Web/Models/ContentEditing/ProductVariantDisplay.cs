using System;
using System.Collections.Generic;
using Merchello.Core.Models;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Web.Models.ContentEditing
{
    public class ProductVariantDisplay : ProductDisplayBase
    {   
        public int Id { get; set; }
        public Guid ProductKey { get; set; }
        public IEnumerable<IProductAttribute> Attributes { get; set; }
    }
}