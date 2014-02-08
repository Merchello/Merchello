using System;
using System.Collections.Generic;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Umbraco.Core;
using Umbraco.Core.Cache;

namespace Merchello.Core.Checkout
{
    /// <summary>
    /// Represents a CheckoutBase class resposible for temporarily persisting invoice and order information
    /// while it's being collected
    /// </summary>
    public abstract class CheckoutBase : ICheckoutBase
    {
        private readonly IItemCache _itemCache;
        private readonly ICustomerBase _customer;
        private readonly IMerchelloContext _merchelloContext;

        internal CheckoutBase(IMerchelloContext merchelloContext, IItemCache itemCache, ICustomerBase customer)
        {                       
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");
            Mandate.ParameterNotNull(itemCache, "ItemCache");
            Mandate.ParameterCondition(itemCache.ItemCacheType == ItemCacheType.Checkout, "itemCache");
            Mandate.ParameterNotNull(customer, "customer");

            _merchelloContext = merchelloContext;
            _customer = customer;
            _itemCache = itemCache;
        }

        /// <summary>
        /// Purges all checkout information persisted
        /// </summary>
        public virtual void RestartCheckout()
        {
            _customer.ExtendedData.RemoveValue(Constants.ExtendedDataKeys.BillingAddress);
            SaveCustomer();

            // TODO this needs to be refactored
            MerchelloContext.Services.ItemCacheService.Delete(_itemCache);
        }

        /// <summary>
        /// Saves a billing address to the customer extended data
        /// </summary>
        /// <param name="billToAddress"></param>
        public virtual void SaveBillToAddress(IAddress billToAddress)
        {
            _customer.ExtendedData.AddAddress(billToAddress, AddressType.Billing);
            SaveCustomer();
        }

        /// <summary>
        /// Saves a <see cref="IShipmentRateQuote"/> as a shipment line item
        /// </summary>
        /// <param name="approvedShipmentRateQuote"></param>
        public virtual void SaveShipmentRateQuote(IShipmentRateQuote approvedShipmentRateQuote)
        {
            AddShipmentRateQuoteLineItem(approvedShipmentRateQuote);         
            MerchelloContext.Services.ItemCacheService.Save(_itemCache);
        }

        /// <summary>
        /// Saves a collection of <see cref="IShipmentRateQuote"/>s as shipment line items
        /// </summary>
        /// <param name="approvedShipmentRateQuotes"></param>
        public virtual void SaveShipmentRateQuote(IEnumerable<IShipmentRateQuote> approvedShipmentRateQuotes)
        {
            approvedShipmentRateQuotes.ForEach(AddShipmentRateQuoteLineItem);
            MerchelloContext.Services.ItemCacheService.Save(_itemCache);
        }

        /// <summary>
        /// Maps the <see cref="IShipmentRateQuote"/> to a <see cref="ILineItem"/> 
        /// </summary>
        /// <param name="shipmentRateQuote">The <see cref="IShipmentRateQuote"/> to be added as a <see cref="ILineItem"/></param>
        private void AddShipmentRateQuoteLineItem(IShipmentRateQuote shipmentRateQuote)
        {
            _itemCache.AddItem(shipmentRateQuote.AsLineItemOf<ItemCacheLineItem>());
        }

        ///// <summary>
        ///// Generates an <see cref="IInvoice"/>
        ///// </summary>
        ///// <param name="applyTax">True/false indicating whether or not to apply taxes to the invoice</param>
        ///// <returns>An <see cref="IInvoice"/></returns>
        //public virtual IInvoice GenerateInvoice(bool applyTax = true)
        //{
        //    var invoiceStatusKey = Constants.DefaultKeys.UnpaidInvoiceStatusKey;

        //    var billToAddress = _customer.ExtendedData.GetAddress(Constants.ExtendedDataKeys.BillingAddress);
        //    var lineItemCollection = _itemCache.Items;


        //    return billToAddress != null ?
        //        new Invoice(invoiceStatusKey, billToAddress, lineItemCollection) :
        //        new Invoice(invoiceStatusKey)
        //            {
        //                Items = lineItemCollection
        //            };
        //}

        ///// <summary>
        ///// Does preliminary validation of the checkout process and then executes the start of the order fulfillment pipeline
        ///// </summary>
        ///// <param name="paymentGatewayProvider">The see <see cref="IPaymentGatewayProvider"/> to be used in payment processing and <see cref="IOrder"/> creation approval</param>
        //public abstract void CompleteCheckout(IPaymentGatewayProvider paymentGatewayProvider);


        /// <summary>
        /// Saves the current customer
        /// </summary>
        private void SaveCustomer()
        {
            if (typeof(AnonymousCustomer) == _customer.GetType())
            {
                _merchelloContext.Services.CustomerService.Save(_customer as AnonymousCustomer);
            }
            else
            {
                ((CustomerService)_merchelloContext.Services.CustomerService).Save(_customer as Customer);
            }
        }

        /// <summary>
        /// The <see cref="ICustomerBase"/>
        /// </summary>
        public ICustomerBase Customer
        {
            get { return _customer; }
        }

        /// <summary>
        /// The <see cref="IMerchelloContext"/>
        /// </summary>
        public IMerchelloContext MerchelloContext
        {
            get { return _merchelloContext; }
        }

        /// <summary>
        /// Shortcut to configured <see cref="IRuntimeCacheProvider"/>
        /// </summary>
        public IRuntimeCacheProvider RuntimeCache
        {
            get { return _merchelloContext.Cache.RuntimeCache; }
        }

        /// <summary>
        /// The <see cref="IItemCache"/>
        /// </summary>
        public IItemCache ItemCache
        {
            get { return _itemCache; }
        }

        public bool ApplyTaxesToInvoice { get; set; }
    }
}