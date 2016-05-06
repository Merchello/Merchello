namespace Merchello.Web.Ui.Implementation
{
    using System;
    using System.Collections.Generic;

    using Merchello.Web.Models.ContentEditing;

    /// <summary>
    /// A model used to add items to a basket or cart.
    /// </summary>
    public class AddItemModel : IAddItemModel
    {
        /// <summary>
        /// Gets or sets the product key.
        /// </summary>
        public Guid ProductKey { get; set; }

        /// <summary>
        /// Gets or sets the quantity.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the option choices (if there are any), used to determine the variant 
        /// in post back
        /// </summary>
        public Guid[] OptionChoices { get; set; }

        /// <summary>
        /// Gets or sets the product options.
        /// </summary>
        /// <remarks>
        /// This will be empty if the product does not have variants
        /// </remarks>
        public IEnumerable<ProductOptionDisplay> ProductOptions { get; set; }
    }
}