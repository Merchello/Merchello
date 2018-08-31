namespace Merchello.FastTrack.Models
{
    using System;
    using System.Collections.Generic;

    using Merchello.Web.Models.Ui;
    using Merchello.Web.Store.Models;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Represents a the FastTrack Checkout Summary.
    /// </summary>
    public class FastTrackCheckoutSummaryModel : ICheckoutSummaryModel<FastTrackBillingAddressModel, StoreAddressModel, StoreLineItemModel>
    {
        /// <summary>
        /// Gets or sets the invoice key.
        /// </summary>
        /// <remarks>
        /// Used in receipts
        /// </remarks>
        public Guid InvoiceKey { get; set; }

        /// <summary>
        /// Gets or sets the invoice number.
        /// </summary>
        /// <remarks>
        /// Used in receipts
        /// </remarks>
        public string InvoiceNumber { get; set; }

        /// <summary>
        /// Gets or sets the invoice date.
        /// </summary>
        /// <remarks>
        /// Used in receipts
        /// </remarks>
        public DateTime InvoiceDate { get; set; }

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

		/// <summary>
		/// CurrencyCode
		/// </summary>
		public string CurrencyCode { get; set; }
	}
}