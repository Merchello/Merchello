namespace Merchello.Web.Store.Models
{
    using System.Collections.Generic;

    using Merchello.Web.Models.Ui;

    /// <summary>
    /// Represents a checkout summary.
    /// </summary>
    public class CheckoutSummaryModel : ICheckoutSummaryModel<CheckoutAddressModel, CheckoutAddressModel, BasketItemModel>
    {
        /// <summary>
        /// Gets or sets the billing address.
        /// </summary>
        public CheckoutAddressModel BillingAddress { get; set; }

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