using System;
using Merchello.Core.Models;

namespace Models
{
    public class BasketViewLineItem
    {
        // ItemCacheLineItem Key (basket item key)
        public Guid Key { get; set; }

        // This is not part of Merchello - This is a customization so we can provide a link to the product page
        // eg. The Umbraco content Id
        public int ContentId { get; set; }

        /// <summary>
        /// This is the name from the <see cref="IProductVariant"/>
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// This is a hack ...
        /// </summary>
        public string Attributes { get; set; }

        /// <summary>
        /// The sku of the <see cref="IProductVariant"/>
        /// </summary>
        public string Sku { get; set; }

        /// <summary>
        /// The price (can be overrided) of the <see cref="IProductVariant"/>
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// The total price of the line item
        /// </summary>
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// The quantity to purchase
        /// </summary>
        public int Quantity { get; set; }
    }
}