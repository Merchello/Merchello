namespace Merchello.Bazaar.Models
{
    using System;

    /// <summary>
    /// A model for rendering products in a "boxed" listing.
    /// </summary>
    public class ProductBoxModel
    {
        /// <summary>
        /// Gets or sets the product key.
        /// </summary>
        public Guid ProductKey { get; set; }

        /// <summary>
        /// Gets or sets the url.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the formatted sale price.
        /// </summary>
        public string FormattedSalePrice { get; set; }

        /// <summary>
        /// Gets or sets the sale price.
        /// </summary>
        public decimal SalePrice { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether on sale.
        /// </summary>
        public bool OnSale { get; set; }

        /// <summary>
        /// Gets or sets the formatted price.
        /// </summary>
        public string FormattedPrice { get; set; }

        /// <summary>
        /// Gets or sets the price.
        /// </summary>
        public decimal Price { get; set; }
    }
}
