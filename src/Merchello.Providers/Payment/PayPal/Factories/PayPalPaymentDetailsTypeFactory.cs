namespace Merchello.Providers.Payment.PayPal.Factories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using global::PayPal.PayPalAPIInterfaceService.Model;

    using Merchello.Core.Events;
    using Merchello.Core.Logging;
    using Merchello.Core.Models;
    using Merchello.Providers.Payment.PayPal.Models;
    using Merchello.Web;

    using Umbraco.Core;
    using Umbraco.Core.Events;

    /// <summary>
    /// A factory responsible for building <see cref="PaymentDetailsItemType"/>.
    /// </summary>
    public class PayPalPaymentDetailsTypeFactory
    {
        /// <summary>
        /// The <see cref="MerchelloHelper"/>.
        /// </summary>
        private readonly MerchelloHelper _merchello = new MerchelloHelper();

        /// <summary>
        /// The base url.
        /// </summary>
        private readonly string _baseUrl;

        /// <summary>
        /// Initializes a new instance of the <see cref="PayPalPaymentDetailsTypeFactory"/> class.
        /// </summary>
        /// <param name="baseUrl">
        /// The base url.
        /// </param>
        public PayPalPaymentDetailsTypeFactory(string baseUrl)
        {
            Mandate.ParameterNotNullOrEmpty(baseUrl, "baseUrl");
            _baseUrl = baseUrl;
        }

        /// <summary>
        /// Builds the <see cref="PaymentDetailsItemType"/>.
        /// </summary>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
        /// <returns>
        /// The <see cref="PaymentDetailsType"/>.
        /// </returns>
        public PaymentDetailsType Build(IInvoice invoice)
        {
            // Get the decimal configuration for the current currency
            var currencyCodeType = GetPayPalCurrency(invoice.CurrencyCode);
            var basicAmountFactory = new PayPalBasicAmountTypeFactory(currencyCodeType);

            // Get the tax total
            var itemTotal = basicAmountFactory.Build(invoice.TotalItemPrice());
            var shippingTotal = basicAmountFactory.Build(invoice.TotalShipping());
            var taxTotal = basicAmountFactory.Build(invoice.TotalTax());
            var invoiceTotal = basicAmountFactory.Build(invoice.Total);

            var items = BuildPaymentDetailsItemTypes(invoice.ProductLineItems());

            // ShipToAddress
            AddressType shipToAddress = null;
            if (invoice.ShippingLineItems().Any())
            {
                var addressTypeFactory = new PayPalAddressTypeFactory();
                shipToAddress = addressTypeFactory.Build(invoice.GetShippingAddresses().FirstOrDefault());

            }

            throw new NotImplementedException();
        }


        /// <summary>
        /// Gets the PayPal <see cref="CurrencyCodeType"/>.
        /// </summary>
        /// <param name="currencyCode">
        /// The currency code.
        /// </param>
        /// <returns>
        /// The <see cref="CurrencyCodeType"/>.
        /// </returns>
        public virtual CurrencyCodeType GetPayPalCurrency(string currencyCode)
        {
            try
            {
                return (CurrencyCodeType)Enum.Parse(typeof(CurrencyCodeType), currencyCode, true);
            }
            catch (Exception ex)
            {
                var logData = MultiLogger.GetBaseLoggingData();
                logData.AddCategory("PayPal");

                MultiLogHelper.Error<PayPalBasicAmountTypeFactory>("Failed to map currency code", ex, logData);

                throw;
            }
        }


        /// <summary>
        /// Builds a list of <see cref="PaymentDetailsItemType"/>.
        /// </summary>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{PaymentDetailsItemType}"/>.
        /// </returns>
        public virtual IEnumerable<PaymentDetailsItemType> BuildPaymentDetailsItemTypes(IInvoice invoice)
        {
            var paymentDetailItems = new List<PaymentDetailsItemType>();
            paymentDetailItems.AddRange(BuildPaymentDetailsItemTypes(invoice.ProductLineItems()));
            paymentDetailItems.AddRange(BuildPaymentDetailsItemTypes(invoice.CustomLineItems()));
            paymentDetailItems.AddRange(BuildPaymentDetailsItemTypes(invoice.DiscountLineItems(), true));

            return paymentDetailItems;
        }

        /// <summary>
        /// Builds a list of <see cref="PaymentDetailsItemType"/>.
        /// </summary>
        /// <param name="items">
        /// The items.
        /// </param>
        /// <param name="areDiscounts">
        /// The are discounts.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{PaymentDetailItemType}"/>.
        /// </returns>
        public virtual IEnumerable<PaymentDetailsItemType> BuildPaymentDetailsItemTypes(IEnumerable<ILineItem> items, bool areDiscounts = false)
        {
            var paymentDetailItems = new List<PaymentDetailsItemType>();

            return paymentDetailItems;
        }
    }
}