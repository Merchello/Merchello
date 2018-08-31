namespace Merchello.Web.Models.Ui
{
    using System;
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
        /// Gets or sets the invoice key.
        /// </summary>
        /// <remarks>
        /// Used in receipts
        /// </remarks>
        Guid InvoiceKey { get; set; }

        /// <summary>
        /// Gets or sets the invoice number.
        /// </summary>
        /// <remarks>
        /// Used in receipts
        /// </remarks>
        string InvoiceNumber { get; set; }

        /// <summary>
        /// Gets or sets the invoice date.
        /// </summary>
        /// <remarks>
        /// Used in receipts
        /// </remarks>
        DateTime InvoiceDate { get; set; }

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

        /// <summary>
        /// Gets or sets the total.
        /// </summary>
        decimal Total { get; set; }

		/// <summary>
		/// CurrencyCode
		/// </summary>
		string CurrencyCode { get; set; }
	}
}