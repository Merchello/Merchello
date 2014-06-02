﻿using System;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Builders;
using Merchello.Core.Gateways.Notification;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Models;
using Merchello.Core.Models.TypeFields;
using Merchello.Core.Services;
using Umbraco.Core;
using Umbraco.Core.Cache;
using Umbraco.Core.Logging;

namespace Merchello.Core.Sales
{
    /// <summary>
    /// Represents an abstract SalesPreparation class resposible for temporarily persisting invoice and order information
    /// while it's being collected
    /// </summary>
    public abstract class SalePreparationBase : ISalePreparationBase
    {
        private readonly IItemCache _itemCache;
        private readonly ICustomerBase _customer;
        private readonly IMerchelloContext _merchelloContext;

        internal SalePreparationBase(IMerchelloContext merchelloContext, IItemCache itemCache, ICustomerBase customer)
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
        /// Purges sales manager information
        /// </summary>
        public virtual void Reset()
        {
            Reset(_merchelloContext, _customer);
        }

        /// <summary>
        /// Purges sales manager information
        /// </summary>
        internal static void Reset(IMerchelloContext merchelloContext, Guid entityKey)
        {
            var customer = merchelloContext.Services.CustomerService.GetAnyByKey(entityKey);
            if(customer == null) return;

            Reset(merchelloContext, customer);
        }

        /// <summary>
        /// Purges persisted checkout information
        /// </summary>
        private static void Reset(IMerchelloContext merchelloContext, ICustomerBase customer)
        {
            customer.ExtendedData.RemoveValue(Constants.ExtendedDataKeys.ShippingDestinationAddress);
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

            _customer.ExtendedData.AddAddress(approvedShipmentRateQuote.Shipment.GetDestinationAddress(), AddressType.Shipping);
            SaveCustomer(_merchelloContext, _customer);
        }

        /// <summary>
        /// Saves a collection of <see cref="IShipmentRateQuote"/>s as shipment line items
        /// </summary>
        /// <param name="approvedShipmentRateQuotes"></param>
        public virtual void SaveShipmentRateQuote(IEnumerable<IShipmentRateQuote> approvedShipmentRateQuotes)
        {
            var shipmentRateQuotes = approvedShipmentRateQuotes as IShipmentRateQuote[] ?? approvedShipmentRateQuotes.ToArray();

            if (!shipmentRateQuotes.Any()) return;
            
            shipmentRateQuotes.ForEach(AddShipmentRateQuoteLineItem);
            _merchelloContext.Services.ItemCacheService.Save(_itemCache);

            _customer.ExtendedData.AddAddress(shipmentRateQuotes.First().Shipment.GetDestinationAddress(), AddressType.Shipping);
            SaveCustomer(_merchelloContext, _customer);
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
        /// True/false indicating whether or not the <see cref="ISalePreparationBase"/> is ready to prepare an <see cref="IInvoice"/>
        /// </summary>
        /// <remarks>
        /// 
        /// This ommits checking that 
        /// 
        /// </remarks>
        public virtual bool IsReadyToInvoice()
        {
            return (_customer.ExtendedData.GetAddress(AddressType.Billing) != null) &&
                   (_customer.ExtendedData.GetAddress(AddressType.Shipping) != null);
        }


        /// <summary>
        /// Generates an <see cref="IInvoice"/>
        /// </summary>
        /// <returns>An <see cref="IInvoice"/></returns>
        public virtual IInvoice PrepareInvoice()
        {
            return !IsReadyToInvoice() ? null : PrepareInvoice(new InvoiceBuilderChain(this));
        }

        /// <summary>
        /// Generates an <see cref="IInvoice"/> representing the bill for the current "checkout order"
        /// </summary>
        /// <param name="invoiceBuilder">The invoice builder class</param>
        /// <returns>An <see cref="IInvoice"/> that is not persisted to the database.</returns>
        public IInvoice PrepareInvoice(IBuilderChain<IInvoice> invoiceBuilder)
        {
            if (!IsReadyToInvoice()) return null;

            var attempt = invoiceBuilder.Build();
            if (!attempt.Success)
            {
                LogHelper.Error<SalePreparationBase>("The invoice builder failed to generate an invoice.", attempt.Exception);
                throw attempt.Exception;
            }

            return attempt.Result;
        }

        /// <summary>
        /// Gets a list of all possible Payment Methods
        /// </summary>
        /// <returns>A collection of <see cref="IPaymentGatewayMethod"/>s</returns>
        public IEnumerable<IPaymentGatewayMethod> GetPaymentGatewayMethods()
        {
            return _merchelloContext.Gateways.Payment.GetPaymentGatewayMethods();
        }

        /// <summary>
        /// Attempts to process a payment
        /// </summary>
        /// <param name="paymentGatewayMethod">The <see cref="IPaymentGatewayMethod"/> to use in processing the payment</param>
        /// <param name="args">Additional arguements required by the payment processor</param>
        /// <returns>The <see cref="IPaymentResult"/></returns>
        public virtual IPaymentResult AuthorizePayment(IPaymentGatewayMethod paymentGatewayMethod, ProcessorArgumentCollection args)
        {
            Mandate.ParameterNotNull(paymentGatewayMethod, "paymentGatewayMethod");

            if(!IsReadyToInvoice()) return new PaymentResult(Attempt<IPayment>.Fail(new InvalidOperationException("SalesPreparation is not ready to invoice")), null, false);

            // invoice
            var invoice = PrepareInvoice(new InvoiceBuilderChain(this));

            MerchelloContext.Services.InvoiceService.Save(invoice);

            //TODO
            // Raise the notification event
           // Announce.Broadcast.InvoicedCustomer(_customer, invoice);

            var result = invoice.AuthorizePayment(paymentGatewayMethod, args);

            //if(result.Payment.Success) 
            //    Announce.Broadcast.PaymentWasAuthorized(_customer, result);

            if (!result.ApproveOrderCreation) return result;

            // order
            var order = result.Invoice.PrepareOrder(MerchelloContext);

            MerchelloContext.Services.OrderService.Save(order);

            return result;
        }

        /// <summary>
        /// Attempts to process a payment
        /// </summary>
        /// <param name="paymentGatewayMethod">The <see cref="IPaymentGatewayMethod"/> to use in processing the payment</param>
        /// <returns>The <see cref="IPaymentResult"/></returns>
        public virtual IPaymentResult AuthorizePayment(IPaymentGatewayMethod paymentGatewayMethod)
        {
            return AuthorizePayment(paymentGatewayMethod, new ProcessorArgumentCollection());
        }


        /// <summary>
        /// Attempts to process a payment
        /// </summary>
        /// <param name="paymentMethodKey">The <see cref="IPaymentMethod"/> key</param>
        /// <param name="args">Additional arguements required by the payment processor</param>
        /// <returns>The <see cref="IPaymentResult"/></returns>
        public virtual IPaymentResult AuthorizePayment(Guid paymentMethodKey, ProcessorArgumentCollection args)
        {
            var paymentMethod = _merchelloContext.Gateways.Payment.GetPaymentGatewayMethods().FirstOrDefault(x => x.PaymentMethod.Key.Equals(paymentMethodKey));

            return AuthorizePayment(paymentMethod, args);

        }

        /// <summary>
        /// Attempts to process a payment
        /// </summary>
        /// <param name="paymentMethodKey">The <see cref="IPaymentMethod"/> key</param>
        /// <returns>The <see cref="IPaymentResult"/></returns>
        public virtual IPaymentResult AuthorizePayment(Guid paymentMethodKey)
        {
            return AuthorizePayment(paymentMethodKey, new ProcessorArgumentCollection());
        }

        /// <summary>
        /// Authorizes and Captures a Payment
        /// </summary>
        /// <param name="paymentGatewayMethod">The <see cref="IPaymentMethod"/></param>
        /// <param name="args">Additional arguements required by the payment processor</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        public virtual IPaymentResult AuthorizeCapturePayment(IPaymentGatewayMethod paymentGatewayMethod, ProcessorArgumentCollection args)
        {
            Mandate.ParameterNotNull(paymentGatewayMethod, "paymentGatewayMethod");

            if (!IsReadyToInvoice()) return new PaymentResult(Attempt<IPayment>.Fail(new InvalidOperationException("SalesPreparation is not ready to invoice")), null, false);

            // invoice
            var invoice = PrepareInvoice(new InvoiceBuilderChain(this));

            MerchelloContext.Services.InvoiceService.Save(invoice);

            //TODO
            //Announce.Broadcast.InvoicedCustomer(_customer, invoice);

            var result = invoice.AuthorizeCapturePayment(paymentGatewayMethod, args);

            //if(result.Payment.Success)
            //    Announce.Broadcast.PaymentWasCaptured(_customer, result);
            
            if (!result.ApproveOrderCreation) return result;

            // order
            var order = result.Invoice.PrepareOrder(MerchelloContext);

            MerchelloContext.Services.OrderService.Save(order);

            return result;
        }

        /// <summary>
        /// Authorizes and Captures a Payment
        /// </summary>
        /// <param name="paymentGatewayMethod">The <see cref="IPaymentMethod"/></param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        public virtual IPaymentResult AuthorizeCapturePayment(IPaymentGatewayMethod paymentGatewayMethod)
        {
            return AuthorizeCapturePayment(paymentGatewayMethod, new ProcessorArgumentCollection());
        }

        /// <summary>
        /// Authorizes and Captures a Payment
        /// </summary>
        /// <param name="paymentMethodKey">The <see cref="IPaymentMethod"/> key</param>
        /// <param name="args">Additional arguements required by the payment processor</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        public virtual IPaymentResult AuthorizeCapturePayment(Guid paymentMethodKey, ProcessorArgumentCollection args)
        {
            var paymentMethod = _merchelloContext.Gateways.Payment.GetPaymentGatewayMethods().FirstOrDefault(x => x.PaymentMethod.Key.Equals(paymentMethodKey));

            return AuthorizeCapturePayment(paymentMethod, args);
        }

        /// <summary>
        /// Authorizes and Captures a Payment
        /// </summary>
        /// <param name="paymentMethodKey">The <see cref="IPaymentMethod"/> key</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        public virtual IPaymentResult AuthorizeCapturePayment(Guid paymentMethodKey)
        {
            return AuthorizeCapturePayment(paymentMethodKey, new ProcessorArgumentCollection());
        }



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
        /// <param name="versionKey">The version key for this <see cref="SalePreparationBase"/></param>
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
                Reset(merchelloContext, customer);

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
        /// Generates a unique cache key for runtime caching of the <see cref="SalePreparationBase"/>
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
        public IMerchelloContext MerchelloContext
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

        internal bool ApplyTaxesToInvoice { get; set; }

    }
}