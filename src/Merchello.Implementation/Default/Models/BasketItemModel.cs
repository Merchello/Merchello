namespace Merchello.Implementation.Default.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Implementation.Models;
    using Merchello.Web.Models.VirtualContent;

    /// <summary>
    /// A model used to represent items in a basket or cart.
    /// </summary>
    public class BasketItemModel : IBasketItemModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BasketItemModel"/> class.
        /// </summary>
        public BasketItemModel()
        {
            this.CustomerOptionChoices = Enumerable.Empty<KeyValuePair<string, string>>();
        }

        /// <summary>
        /// Gets or sets the line item key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the product key.
        /// </summary>
        public Guid ProductKey { get; set; }

        /// <summary>
        /// Gets or sets the product.
        /// </summary>
        public IProductContent Product { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the SKU.
        /// </summary>
        public string Sku { get; set; }

        /// <summary>
        /// Gets or sets the quantity.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the unit price.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the attributes.
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> CustomerOptionChoices { get; set; }
    }
}