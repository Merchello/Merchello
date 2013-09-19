using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Models;

namespace Merchello.Tests.Base.Data
{
    public class ProductData
    {
        public static IProduct MockProductForInserting(string sku)
        {
            return new Product()
            {
                Sku = sku,
                Name = "fake product",
                Price = 15.00m,
                CostOfGoods = null,
                SalePrice = null,
                Weight = null,
                Length = null,
                Width = null,
                Height = null,
                Taxable = true,
                Shippable = false,
                Download = false,
                Template = false
            };
        }

        public static string MockSku()
        {
            return Guid.NewGuid().ToString().Substring(0, 8);
        }
    }
}
