namespace Merchello.Web.Models.Ui
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines a DiscountApplicationResult.
    /// </summary>
    /// <typeparam name="TLineItemModel">
    /// The type of <see cref="ILineItemModel"/>
    /// </typeparam>
    /// <remarks>
    /// This result has already been internally added to the invoice and is returned ONLY so that additional information
    /// can be extracted for use in view.
    /// </remarks>
    public interface IDiscountViewData<TLineItemModel> : IMerchelloViewData
        where TLineItemModel : class, ILineItemModel, new()
    {
        /// <summary>
        /// Gets or sets the discount line item generated and applied to the invoice.
        /// </summary>
        TLineItemModel LineItem { get; set; }
    }
}