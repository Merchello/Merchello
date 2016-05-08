namespace Merchello.Implementation.Models
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

        /// <summary>
        /// Gets or sets the URL of the page we redirect to after a successful basket add item operation.
        /// </summary>
        /// <remarks>
        /// This is not included on the <see cref="IAddItemModel"/> interface since the success result can be customized.
        /// e.g. Some implementations may wish to redirect to actions in other controllers
        /// </remarks>
        public string SuccessRedirectUrl { get; set; }
    }
}