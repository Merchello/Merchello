namespace Merchello.Bazaar.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using Merchello.Web.Models.ContentEditing;

    /// <summary>
    /// Simple Model for the Add To Cart form.
    /// </summary>
    public class AddItemModel
    {
        /// <summary>
        /// Gets or sets the Content Id of the ProductDetail page
        /// </summary>
        [Required]
        public int ContentId { get; set; }

        /// <summary>
        /// Gets or sets the basket page id.
        /// </summary>
        public int BasketPageId { get; set; }

        /// <summary>
        /// Gets or sets the product.
        /// </summary>
        public ProductDisplay Product { get; set; }

        /// <summary>
        /// Gets or sets the option choices (if there are any), used to determine the variant 
        /// </summary>
        public Guid[] OptionChoices { get; set; }
    }
}