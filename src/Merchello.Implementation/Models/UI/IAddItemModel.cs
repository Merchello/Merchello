namespace Merchello.Web.Ui.Implementation
{
    using System;
    using System.Collections.Generic;

    using Merchello.Implementation.Models.UI;
    using Merchello.Web.Models.ContentEditing;

    /// <summary>
    /// Defines a model used to add items to a basket or cart.
    /// </summary>
    public interface IAddItemModel : IStoreImplementationModel
    {
        /// <summary>
        /// Gets or sets the product key.
        /// </summary>
        Guid ProductKey { get; set; }

        /// <summary>
        /// Gets or sets the quantity.
        /// </summary>
        int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the option choices (if there are any), used to determine the variant 
        /// in post back
        /// </summary>
        Guid[] OptionChoices { get; set; }

        /// <summary>
        /// Gets or sets the product options.
        /// </summary>
        /// <remarks>
        /// This will be empty if the product does not have variants
        /// </remarks>
        IEnumerable<ProductOptionDisplay> ProductOptions { get; set; }
    }
}