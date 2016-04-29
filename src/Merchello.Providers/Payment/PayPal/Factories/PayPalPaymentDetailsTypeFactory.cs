namespace Merchello.Providers.Payment.PayPal.Factories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Logging;
    using Merchello.Core.Models;
    using Merchello.Web;
    using Merchello.Web.Models.VirtualContent;

    using global::PayPal.PayPalAPIInterfaceService.Model;

    using Umbraco.Core;

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
        private readonly PayPalFactorySettings _settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="PayPalPaymentDetailsTypeFactory"/> class.
        /// </summary>
        public PayPalPaymentDetailsTypeFactory()
            : this(new PayPalFactorySettings())
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PayPalPaymentDetailsTypeFactory"/> class.
        /// </summary>
        /// <param name="settings">
        /// The PayPal factory settings.
        /// </param>
        public PayPalPaymentDetailsTypeFactory(PayPalFactorySettings settings)
        {
            Mandate.ParameterNotNull(settings, "settings");
            _settings = settings;
        }

        /// <summary>
        /// Builds the <see cref="PaymentDetailsItemType"/>.
        /// </summary>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
        /// <param name="actionCode">
        /// The <see cref="PaymentActionCodeType"/>.
        /// </param>
        /// <returns>
        /// The <see cref="PaymentDetailsType"/>.
        /// </returns>
        public PaymentDetailsType Build(IInvoice invoice, PaymentActionCodeType actionCode)
        {
            // Get the decimal configuration for the current currency
            var currencyCodeType = PayPalApiHelper.GetPayPalCurrencyCode(invoice.CurrencyCode);
            var basicAmountFactory = new PayPalBasicAmountTypeFactory(currencyCodeType);

            // Get the tax total
            var itemTotal = basicAmountFactory.Build(invoice.TotalItemPrice());
            var shippingTotal = basicAmountFactory.Build(invoice.TotalShipping());
            var taxTotal = basicAmountFactory.Build(invoice.TotalTax());
            var invoiceTotal = basicAmountFactory.Build(invoice.Total);

            var items = BuildPaymentDetailsItemTypes(invoice.ProductLineItems(), basicAmountFactory);

            var paymentDetails = new PaymentDetailsType
            {
                PaymentDetailsItem = items.ToList(),
                ItemTotal = itemTotal,
                TaxTotal = taxTotal,
                ShippingTotal = shippingTotal,
                OrderTotal = invoiceTotal,
                PaymentAction = actionCode,
                InvoiceID = invoice.PrefixedInvoiceNumber()
            };

            // ShipToAddress
            if (invoice.ShippingLineItems().Any())
            {
                var addressTypeFactory = new PayPalAddressTypeFactory();
                paymentDetails.ShipToAddress = addressTypeFactory.Build(invoice.GetShippingAddresses().FirstOrDefault());
            }

            return paymentDetails;
        }

        /// <summary>
        /// Builds a list of <see cref="PaymentDetailsItemType"/>.
        /// </summary>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
        /// <param name="factory">
        /// The <see cref="PayPalBasicAmountTypeFactory"/>.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{PaymentDetailsItemType}"/>.
        /// </returns>
        public virtual IEnumerable<PaymentDetailsItemType> BuildPaymentDetailsItemTypes(IInvoice invoice, PayPalBasicAmountTypeFactory factory)
        {
            var paymentDetailItems = new List<PaymentDetailsItemType>();
            paymentDetailItems.AddRange(BuildPaymentDetailsItemTypes(invoice.ProductLineItems(), factory));
            paymentDetailItems.AddRange(BuildPaymentDetailsItemTypes(invoice.CustomLineItems(), factory));
            paymentDetailItems.AddRange(BuildPaymentDetailsItemTypes(invoice.DiscountLineItems(), factory, true));

            return paymentDetailItems;
        }

        /// <summary>
        /// Builds a list of <see cref="PaymentDetailsItemType"/>.
        /// </summary>
        /// <param name="items">
        /// The items.
        /// </param>        
        /// <param name="factory">
        /// The <see cref="PayPalBasicAmountTypeFactory"/>.
        /// </param>
        /// <param name="areDiscounts">
        /// The are discounts.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{PaymentDetailItemType}"/>.
        /// </returns>
        public virtual IEnumerable<PaymentDetailsItemType> BuildPaymentDetailsItemTypes(IEnumerable<ILineItem> items, PayPalBasicAmountTypeFactory factory, bool areDiscounts = false)
        {
            return items.ToArray().Select(
                item => item.ExtendedData.ContainsProductKey() ? 
                    this.BuildProductPaymentDetailsItemType(item, factory) : 
                    this.BuildGenericPaymentDetailsItemType(item, factory, areDiscounts)).ToList();
        }

        /// <summary>
        /// Builds a <see cref="PaymentDetailsItemType"/>.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <param name="factory">
        /// The <see cref="PayPalBasicAmountTypeFactory"/>.
        /// </param>
        /// <param name="isDiscount">
        /// The is discount.
        /// </param>
        /// <returns>
        /// The <see cref="PaymentDetailsItemType"/>.
        /// </returns>
        protected virtual PaymentDetailsItemType BuildGenericPaymentDetailsItemType(ILineItem item, PayPalBasicAmountTypeFactory factory, bool isDiscount)
        {
            var detailsItemType = new PaymentDetailsItemType
            {
                Name = item.Name,
                ItemURL = null,
                Amount = factory.Build(item.Price),
                Quantity = item.Quantity,
            };

            return detailsItemType;
        }

        /// <summary>
        /// The build product payment details item type.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>        
        /// <param name="factory">
        /// The <see cref="PayPalBasicAmountTypeFactory"/>.
        /// </param>
        /// <returns>
        /// The <see cref="PaymentDetailsItemType"/>.
        /// </returns>
        protected virtual PaymentDetailsItemType BuildProductPaymentDetailsItemType(ILineItem item, PayPalBasicAmountTypeFactory factory)
        {
            IProductContent product = null;
            if (_settings.UsesProductContent)
            {
                var productKey = item.ExtendedData.GetProductKey();
                product = _merchello.TypedProductContent(productKey);
            }

            var detailsItemType = new PaymentDetailsItemType
            {
                Name = item.Name,
                ItemURL = product != null ? 
                    string.Format("{0}{1}", _settings.WebsiteUrl, product.Url) :
                    null,
                Amount = factory.Build(item.Price),
                Quantity = item.Quantity
            };

            return detailsItemType;
        }
    }
}