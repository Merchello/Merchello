namespace Merchello.Web.Store.Models
{
    using System.Collections.Generic;

    using Merchello.Web.Models.Ui;

    /// <summary>
    /// Represents a checkout summary.
    /// </summary>
    public class StoreSummaryModel : ICheckoutSummaryModel<StoreAddressModel, StoreAddressModel, StoreLineItemModel>
    {
        /// <summary>
        /// Gets or sets the billing address.
        /// </summary>
        public StoreAddressModel BillingAddress { get; set; }

        /// <summary>
        /// Gets or sets the shipping address.
        /// </summary>
        public StoreAddressModel ShippingAddress { get; set; }

        /// <summary>
        /// Gets or sets the checkout summary items.
        /// </summary>
        public IEnumerable<StoreLineItemModel> Items { get; set; }

        /// <summary>
        /// Gets or sets the sub total.
        /// </summary>
        public decimal Total { get; set; }
    }
}