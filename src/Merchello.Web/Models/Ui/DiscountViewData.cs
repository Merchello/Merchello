namespace Merchello.Web.Models.Ui
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a <see cref="IDiscountViewData{TLineItemModel}"/>.
    /// </summary>
    /// <typeparam name="TLineItemModel">
    /// The type of <see cref="ILineItemModel"/>
    /// </typeparam>
    /// <remarks>
    /// This result has already been internally added to the invoice and is returned ONLY so that additional information
    /// can be extracted for use in view.
    /// </remarks>
    internal class DiscountViewData<TLineItemModel> : IDiscountViewData<TLineItemModel>
        where TLineItemModel : class, ILineItemModel, new()
    {
        /// <summary>
        /// Gets or sets a value indicating whether the application attempt was successful.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets any exception that occurred during the application attempt.
        /// </summary>
        /// <remarks>
        /// Defaults is null
        /// </remarks>
        public Exception Exception { get; set; }

        /// <summary>
        /// Gets or sets the collection of messages resulting from the attempt.
        /// </summary>
        public IEnumerable<string> Messages { get; set; }

        /// <summary>
        /// Gets or sets the discount line item generated and applied to the invoice.
        /// </summary>
        public TLineItemModel LineItem { get; set; }
    }
}