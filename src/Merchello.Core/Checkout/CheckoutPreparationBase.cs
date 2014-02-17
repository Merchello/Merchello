using System;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Builders;
using Merchello.Core.Configuration;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Models;
using Merchello.Core.Models.TypeFields;
using Merchello.Core.Services;
using Umbraco.Core;
using Umbraco.Core.Cache;
using Umbraco.Core.Logging;

namespace Merchello.Core.Checkout
{
    /// <summary>
    /// Represents a CheckoutBase class resposible for temporarily persisting invoice and order information
    /// while it's being collected
    /// </summary>
    public abstract class CheckoutPreparationBase : ICheckoutPreparationBase
    {
        private readonly IItemCache _itemCache;
        private readonly ICustomerBase _customer;
        private readonly IMerchelloContext _merchelloContext;

        internal CheckoutPreparationBase(IMerchelloContext merchelloContext, IItemCache itemCache, ICustomerBase customer)
        {                       
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");
            Mandate.ParameterNotNull(itemCache, "ItemCache");
            Mandate.ParameterCondition(itemCache.ItemCacheType == ItemCacheType.Checkout, "itemCache");
            Mandate.ParameterNotNull(customer, "customer");

            _merchelloContext = merchelloContext;
            _customer = customer;
            _itemCache = itemCache;
            ApplyTaxesToInvoice = true;
        }

        /// <summary>
        /// Purges persisted checkout information
        /// </summary>
        public virtual void RestartCheckout()
        {
            RestartCheckout(_merchelloContext, _customer, ItemCache.VersionKey);
        }

        /// <summary>
        /// Purges persisted checkout information
        /// </summary>
        internal static void RestartCheckout(IMerchelloContext merchelloContext, Guid entityKey, Guid versionKey)
        {
            var customer = merchelloContext.Services.CustomerService.GetAnyByKey(entityKey);
            if(customer == null) return;

            RestartCheckout(merchelloContext, customer, versionKey);
        }

        /// <summary>
        /// Purges persisted checkout information
        /// </summary>
        private static void RestartCheckout(IMerchelloContext merchelloContext, ICustomerBase customer, Guid versionKey)
        {
            customer.ExtendedData.RemoveValue(Constants.ExtendedDataKeys.BillingAddress);
            SaveCustomer(merchelloContext, customer);
        }

        /// <summary>
        /// Saves the bill to address
        /// </summary>
        /// <param name="billToAddress">The billing <see cref="IAddress"/></param>
        public virtual void SaveBillToAddress(IAddress billToAddress)
        {
            _customer.ExtendedData.AddAddress(billToAddress, AddressType.Billing);
            SaveCustomer(_merchelloContext, _customer);
        }

        /// <summary>
        /// Saves the ship to address
        /// </summary>
        /// <param name="shipToAddress">The shipping <see cref="IAddress"/></param>
        public virtual void SaveShipToAddress(IAddress shipToAddress)
        {
            _customer.ExtendedData.AddAddress(shipToAddress, AddressType.Shipping);
            SaveCustomer(_merchelloContext, _customer);
        }

        /// <summary>
        /// Gets the bill to address
        /// </summary>
        /// <returns>Return the billing <see cref="IAddress"/></returns>
        public IAddress GetBillToAddress()
        {
            return _customer.ExtendedData.GetAddress(AddressType.Billing);
        }

        /// <summary>
        /// Gets the bill to address
        /// </summary>
        /// <returns>Return the billing <see cref="IAddress"/></returns>
        public IAddress GetShipToAddress()
        {
            return _customer.ExtendedData.GetAddress(AddressType.Shipping);
        }

        /// <summary>
        /// Saves a <see cref="IShipmentRateQuote"/> as a shipment line item
        /// </summary>
        /// <param name="approvedShipmentRateQuote"></param>
        public virtual void SaveShipmentRateQuote(IShipmentRateQuote approvedShipmentRateQuote)
        {
            AddShipmentRateQuoteLineItem(approvedShipmentRateQuote);         
            _merchelloContext.Services.ItemCacheService.Save(_itemCache);
        }

        /// <summary>
        /// Saves a collection of <see cref="IShipmentRateQuote"/>s as shipment line items
        /// </summary>
        /// <param name="approvedShipmentRateQuotes"></param>
        public virtual void SaveShipmentRateQuote(IEnumerable<IShipmentRateQuote> approvedShipmentRateQuotes)
        {
            approvedShipmentRateQuotes.ForEach(AddShipmentRateQuoteLineItem);
            _merchelloContext.Services.ItemCacheService.Save(_itemCache);
        }

        /// <summary>
        /// Maps the <see cref="IShipmentRateQuote"/> to a <see cref="ILineItem"/> 
        /// </summary>
        /// <param name="shipmentRateQuote">The <see cref="IShipmentRateQuote"/> to be added as a <see cref="ILineItem"/></param>
        private void AddShipmentRateQuoteLineItem(IShipmentRateQuote shipmentRateQuote)
        {
            _itemCache.AddItem(shipmentRateQuote.AsLineItemOf<ItemCacheLineItem>());
        }



        /// <summary>
        /// Generates an <see cref="IInvoice"/>
        /// </summary>
        /// <returns>An <see cref="IInvoice"/></returns>
        public virtual IInvoice GenerateInvoice()
        {
            return GenerateInvoice(new InvoiceBuilderChain(this));
        }

        /// <summary>
        /// Generates an <see cref="IInvoice"/> representing the bill for the current "checkout order"
        /// </summary>
        /// <param name="invoiceBuilder">The invoice builder class</param>
        /// <returns>An <see cref="IInvoice"/> that is not persisted to the database.</returns>
        public IInvoice GenerateInvoice(IBuilderChain<IInvoice> invoiceBuilder)
        {
            var attempt = invoiceBuilder.Build();
            if (!attempt.Success)
            {
                LogHelper.Error<CheckoutPreparationBase>("The invoice builder failed to generate an invoice.", attempt.Exception);
                throw attempt.Exception;
            }

            return attempt.Result;
        }

        ///// <summary>
        ///// Does preliminary validation of the checkout process and then executes the start of the order fulfillment pipeline
        ///// </summary>
        ///// <param name="paymentGatewayProvider">The see <see cref="IPaymentGatewayProvider"/> to be used in payment processing and <see cref="IOrder"/> creation approval</param>
        //public abstract void CompleteCheckout(IPaymentGatewayProvider paymentGatewayProvider);


        /// <summary>
        /// Saves the current customer
        /// </summary>
        private static void SaveCustomer(IMerchelloContext merchelloContext, ICustomerBase customer)
        {
            if (typeof(AnonymousCustomer) == customer.GetType())
            {
                merchelloContext.Services.CustomerService.Save(customer as AnonymousCustomer);
            }
            else
            {
                ((CustomerService)merchelloContext.Services.CustomerService).Save(customer as Customer);
            }
        }

        /// <summary>
        /// Gets the checkout <see cref="IItemCache"/> for the <see cref="ICustomerBase"/>
        /// </summary>
        /// <param name="customer">The customer associated with the checkout</param>
        /// <param name="merchelloContext">The <see cref="IMerchelloContext"/></param>
        /// <param name="versionKey">The version key for this <see cref="CheckoutPreparationBase"/></param>
        /// <returns>The <see cref="IItemCache"/> associated with the customer checkout</returns>
        protected static IItemCache GetItemCache(IMerchelloContext merchelloContext, ICustomerBase customer, Guid versionKey)
        {
            var runtimeCache = merchelloContext.Cache.RuntimeCache;

            var cacheKey = MakeCacheKey(customer, versionKey);
            var itemCache = runtimeCache.GetCacheItem(cacheKey) as IItemCache;
            if (itemCache != null) return itemCache;
            
            
            itemCache = merchelloContext.Services.ItemCacheService.GetItemCacheWithKey(customer, ItemCacheType.Checkout, versionKey);

            // this is probably an invalid version of the checkout
            if (!itemCache.VersionKey.Equals(versionKey))
            {
                var oldCacheKey = MakeCacheKey(customer, itemCache.VersionKey);
                runtimeCache.ClearCacheItem(oldCacheKey);
                RestartCheckout(merchelloContext, customer, versionKey);

                // delete the old version
                merchelloContext.Services.ItemCacheService.Delete(itemCache);
                return GetItemCache(merchelloContext, customer, versionKey);
            }

            runtimeCache.InsertCacheItem(cacheKey, () => itemCache);
            return itemCache;
        }

        /// <summary>
        /// Makes the 'unique' RuntimeCache Key for the RuntimeCache
        /// </summary>
        protected string MakeCacheKey()
        {
            return MakeCacheKey(_customer, _itemCache.VersionKey);
        }

        /// <summary>
        /// Generates a unique cache key for runtime caching of the <see cref="CheckoutPreparationBase"/>
        /// </summary>
        /// <param name="customer"><see cref="ICustomerBase"/></param>
        /// <param name="versionKey">The version key</param>
        /// <returns>The unique CacheKey string</returns>
        /// <remarks>
        /// 
        /// CacheKey is assumed to be unique per customer and globally for CheckoutBase.  Therefore this will NOT be unique if 
        /// to different checkouts are happening for the same customer at the same time - we consider that an extreme edge case.
        /// 
        /// </remarks>
        private static string MakeCacheKey(ICustomerBase customer, Guid versionKey)
        {
            var itemCacheTfKey = EnumTypeFieldConverter.ItemItemCache.Checkout.TypeKey;
            return Cache.CacheKeys.ItemCacheCacheKey(customer.EntityKey, itemCacheTfKey, versionKey);
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
        protected IMerchelloContext MerchelloContext
        {
            get { return _merchelloContext; }
        }

        /// <summary>
        /// Shortcut to configured <see cref="IRuntimeCacheProvider"/>
        /// </summary>
        protected IRuntimeCacheProvider RuntimeCache
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