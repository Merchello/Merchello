namespace Merchello.FastTrack.Models
{
    using System.Collections.Generic;

    using Merchello.Web.Models.Ui;
    using Merchello.Web.Store.Models;

    /// <summary>
    /// Represents a the FastTrack Checkout Summary.
    /// </summary>
    public class FastTrackCheckoutSummary : ICheckoutSummaryModel<FastTrackBillingAddressModel, CheckoutAddressModel, BasketItemModel>
    {
        /// <summary>
        /// Gets or sets the billing address.
        /// </summary>
        public FastTrackBillingAddressModel BillingAddress { get; set; }

        /// <summary>
        /// Gets or sets the shipping address.
        /// </summary>
        public CheckoutAddressModel ShippingAddress { get; set; }

        /// <summary>
        /// Gets or sets the checkout summary items.
        /// </summary>
        public IEnumerable<BasketItemModel> Items { get; set; }
    }
}