namespace Merchello.FastTrack.Models
{
    using System.Collections.Generic;

    using Merchello.Web.Models.Ui;
    using Merchello.Web.Store.Models;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Represents a the FastTrack Checkout Summary.
    /// </summary>
    public class FastTrackCheckoutSummary : ICheckoutSummaryModel<FastTrackBillingAddressModel, StoreAddressModel, StoreLineItemModel>
    {
        /// <summary>
        /// Gets or sets the billing address.
        /// </summary>
        public FastTrackBillingAddressModel BillingAddress { get; set; }

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

        /// <summary>
        /// Gets or sets the current checkout stage.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public CheckoutStage CheckoutStage { get; set; }
    }
}