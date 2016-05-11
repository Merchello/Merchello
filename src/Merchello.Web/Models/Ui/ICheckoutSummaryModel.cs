namespace Merchello.Web.Models.Ui
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines a checkout summary.
    /// </summary>
    /// <typeparam name="TBillingAddress">
    /// The type of the <see cref="ICheckoutAddressModel"/> for billing
    /// </typeparam>
    /// <typeparam name="TShippingAddress">
    /// The type of the <see cref="ICheckoutAddressModel"/> for shipping
    /// </typeparam>
    /// <typeparam name="TLineItem">
    /// The type of the <see cref="ILineItemModel"/>
    /// </typeparam>
    public interface ICheckoutSummaryModel<TBillingAddress, TShippingAddress, TLineItem> : IUiModel
        where TBillingAddress : class, ICheckoutAddressModel, new()
        where TShippingAddress : class, ICheckoutAddressModel, new()
        where TLineItem : class, ILineItemModel, new()
    {
        /// <summary>
        /// Gets or sets the billing address.
        /// </summary>
        TBillingAddress BillingAddress { get; set; }

        /// <summary>
        /// Gets or sets the shipping address.
        /// </summary>
        TShippingAddress ShippingAddress { get; set; }

        /// <summary>
        /// Gets or sets the checkout summary items.
        /// </summary>
        IEnumerable<TLineItem> Items { get; set; }
    }
}