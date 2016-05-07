namespace Merchello.Implementation.Generic.Models
{
    using System;
    using System.Collections.Generic;

    using Merchello.Implementation.Models;

    /// <summary>
    /// A model used to represent items in a basket or cart.
    /// </summary>
    public class BasketItemModel : IBasketItemModel
    {
        /// <summary>
        /// Gets or sets the line item key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the product key.
        /// </summary>
        public Guid ProductKey { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the quantity.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the attributes.
        /// </summary>
        public IEnumerable<Tuple<string, string>> CustomerOptionChoices { get; set; }
    }
}