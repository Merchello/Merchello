namespace Merchello.Bazaar.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using Merchello.Core.Models;
    using Merchello.Web.Models.ContentEditing;

    /// <summary>
    /// Simple Model for the Add To Cart form.
    /// </summary>
    public partial class AddItemModel
    {
        /// <summary>
        /// Gets or sets the Content Id of the ProductDetail page
        /// </summary>
        public int ContentId { get; set; }

        /// <summary>
        /// Gets or sets the basket page id.
        /// </summary>
        public int BasketPageId { get; set; }

        /// <summary>
        /// Gets or sets the wish list page id.
        /// </summary>
        public int WishListPageId { get; set; }

        /// <summary>
        /// Gets or sets the product.
        /// </summary>
        public ProductDisplay Product { get; set; }

        /// <summary>
        /// Gets or sets the option choices (if there are any), used to determine the variant 
        /// </summary>
        public Guid[] OptionChoices { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether show wish list.
        /// </summary>
        public bool ShowWishList { get; set; }

        /// <summary>
        /// Gets or sets the currency.
        /// </summary>
        public ICurrency Currency { get; set; }
    }
}