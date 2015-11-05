namespace Merchello.Core.Sales
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Builders;
    using Merchello.Core.Events;
    using Merchello.Core.Gateways.Payment;
    using Merchello.Core.Gateways.Shipping;
    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Models;
    using Merchello.Core.Models.TypeFields;
    using Merchello.Core.Services;

    using Newtonsoft.Json;

    using Umbraco.Core;
    using Umbraco.Core.Cache;
    using Umbraco.Core.Events;
    using Umbraco.Core.Logging;

    /// <summary>
    /// Represents an abstract SalesPreparation class responsible for temporarily persisting invoice and order information
    /// while it's being collected
    /// </summary>
    public abstract class SalePreparationBase : ISalePreparationBase
    {
        /// <summary>
        /// The item cache.
        /// </summary>
        private readonly IItemCache _itemCache;

        /// <summary>
        /// The customer.
        /// </summary>
        private readonly ICustomerBase _customer;

        /// <summary>
        /// The merchello context.
        /// </summary>
        private readonly IMerchelloContext _merchelloContext;

        /// <summary>
        /// A value indicating whether or not shipping charges are taxable.
        /// </summary>
        /// <remarks>
        /// Determined by the global back office setting.
        /// </remarks>
        private Lazy<bool> _shippingTaxable;

        /// <summary>
        /// The offer code temp data.
        /// </summary>
        private Lazy<List<string>> _offerCodeTempData;

        /// <summary>
        /// Initializes a new instance of the <see cref="SalePreparationBase"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="itemCache">
        /// The item cache.
        /// </param>
        /// <param name="customer">
        /// The customer.
        /// </param>
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
            RaiseCustomerEvents = false;

            Initialize();
        }

        /// <summary>
        /// Occurs after an invoice has been prepared.
        /// </summary>
        public static event TypedEventHandler<SalePreparationBase, SalesPreparationEventArgs<IInvoice>> InvoicePrepared;

        /// <summary>
        /// Occurs after a sale has been finalized.
        /// </summary>
        public static event TypedEventHandler<SalePreparationBase, SalesPreparationEventArgs<IPaymentResult>> Finalizing;

        /// <summary>
        /// Gets or sets a value indicating whether raise customer events.
        /// </summary>
        public bool RaiseCustomerEvents { get; set; }


        /// <summary>
        /// Gets the <see cref="ICustomerBase"/>
        /// </summary>
        public ICustomerBase Customer
        {
            get { return _customer; }
        }

        /// <summary>
        /// Gets the <see cref="IMerchelloContext"/>
        /// </summary>
        public IMerchelloContext MerchelloContext
        {
            get { return _merchelloContext; }
        }

        /// <summary>
        /// Gets the <see cref="IItemCache"/>
        /// </summary>
        public IItemCache ItemCache
        {
            get { return _itemCache; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to apply taxes to invoice.
        /// </summary>
        public bool ApplyTaxesToInvoice { get; set; }

        /// <summary>
        /// Gets or sets a prefix to be prepended to an invoice number.
        /// </summary>
        public string InvoiceNumberPrefix { get; set; }

        /// <summary>
        /// Gets the offer codes.
        /// </summary>
        internal IEnumerable<string> OfferCodes
        {
            get
            {
                return this._offerCodeTempData.Value;
            }
        }

        /// <summary>
        /// Gets the <see cref="IRuntimeCacheProvider"/>
        /// </summary>
        protected IRuntimeCacheProvider RuntimeCache
        {
            get { return _merchelloContext.Cache.RuntimeCache; }
        }

        /// <summary>
        /// Purges sales manager information
        /// </summary>
        public virtual void Reset()
        {
            Reset(_merchelloContext, _customer);
        }        

        /// <summary>
        /// Saves the bill to address
        /// </summary>
        /// <param name="billToAddress">The billing <see cref="IAddress"/></param>
        public virtual void SaveBillToAddress(IAddress billToAddress)
        {           
            _customer.ExtendedData.AddAddress(billToAddress, AddressType.Billing);
            SaveCustomer(_merchelloContext, _customer, RaiseCustomerEvents);
        }

        /// <summary>
        /// Saves the ship to address
        /// </summary>
        /// <param name="shipToAddress">The shipping <see cref="IAddress"/></param>
        public virtual void SaveShipToAddress(IAddress shipToAddress)
        {
            _customer.ExtendedData.AddAddress(shipToAddress, AddressType.Shipping);
            SaveCustomer(_merchelloContext, _customer, RaiseCustomerEvents);
        }

        /// <summary>
        /// Saves the note
        /// </summary>
        /// <param name="note">The <see cref="INote"/></param>
        public virtual void SaveNote(NoteDisplay note)
        {
            // Check for existing note and modify it if it already exists so we don't end up with lots of orphan notes if the customer keeps submitting.
            var existingNote = GetNote();
            if (existingNote != null)
            {
                existingNote.Message = note.Message;
            }
            else
            {
                _customer.ExtendedData.AddNote(note);
            }
            SaveCustomer(_merchelloContext, _customer, RaiseCustomerEvents);
        }

        /// <summary>
        /// Saves the note
        /// </summary>
        /// <param name="message">The message to save into a note</param>
        public virtual void SaveNote(string message)
        {           
            _customer.ExtendedData.AddNote(new NoteDisplay() { Message = message});  
            SaveCustomer(_merchelloContext, _customer, RaiseCustomerEvents);
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
        /// Gets the note
        /// </summary>
        /// <returns>Return the <see cref="INote"/></returns>
        public NoteDisplay GetNote()
        {
            return _customer.ExtendedData.GetNote();
        }

        /// <summary>
        /// Saves a <see cref="IShipmentRateQuote"/> as a shipment line item
        /// </summary>
        /// <param name="approvedShipmentRateQuote">
        /// The <see cref="IShipmentRateQuote"/> to be saved
        /// </param>
        public virtual void SaveShipmentRateQuote(IShipmentRateQuote approvedShipmentRateQuote)
        {
            AddShipmentRateQuoteLineItem(approvedShipmentRateQuote);         
            _merchelloContext.Services.ItemCacheService.Save(_itemCache);

            _customer.ExtendedData.AddAddress(approvedShipmentRateQuote.Shipment.GetDestinationAddress(), AddressType.Shipping);
            SaveCustomer(_merchelloContext, _customer, RaiseCustomerEvents);
        }

        /// <summary>
        /// Saves a collection of <see cref="IShipmentRateQuote"/>s as shipment line items
        /// </summary>
        /// <param name="approvedShipmentRateQuotes">
        /// The collection of <see cref="IShipmentRateQuote"/>s to be saved
        /// </param>
        public virtual void SaveShipmentRateQuote(IEnumerable<IShipmentRateQuote> approvedShipmentRateQuotes)
        {
            var shipmentRateQuotes = approvedShipmentRateQuotes as IShipmentRateQuote[] ?? approvedShipmentRateQuotes.ToArray();

            if (!shipmentRateQuotes.Any()) return;
            
            shipmentRateQuotes.ForEach(AddShipmentRateQuoteLineItem);
            _merchelloContext.Services.ItemCacheService.Save(_itemCache);

            _customer.ExtendedData.AddAddress(shipmentRateQuotes.First().Shipment.GetDestinationAddress(), AddressType.Shipping);
            SaveCustomer(_merchelloContext, _customer, RaiseCustomerEvents);
        }

        /// <summary>
        /// Clears all <see cref="IShipmentRateQuote"/>s previously saved
        /// </summary>
        public void ClearShipmentRateQuotes()
        {
            var items = _itemCache.Items.Where(x => x.LineItemType == LineItemType.Shipping).ToArray();

            foreach (var item in items)
            {
                _itemCache.Items.RemoveItem(item.Sku);
            }

            _merchelloContext.Services.ItemCacheService.Save(_itemCache);
        }

        /// <summary>
        /// Saves a <see cref="IPaymentMethod"/> to <see cref="ICustomerBase"/> extended data
        /// </summary>
        /// <param name="paymentMethod">
        /// The payment Method.
        /// </param>
        public void SavePaymentMethod(IPaymentMethod paymentMethod)
        {
            _customer.ExtendedData.AddPaymentMethod(paymentMethod);
            SaveCustomer(_merchelloContext, _customer, RaiseCustomerEvents);
        }

        /// <summary>
        /// Gets a <see cref="IPaymentMethod"/> from <see cref="ICustomerBase"/> extended data
        /// </summary>
        /// <returns>
        /// The previously saved <see cref="IPaymentMethod"/>.
        /// </returns>
        public IPaymentMethod GetPaymentMethod()
        {
            var paymentMethodKey = _customer.ExtendedData.GetPaymentMethodKey();
            var paymentMethod = _merchelloContext.Gateways.Payment.GetPaymentGatewayMethodByKey(paymentMethodKey);
            return paymentMethodKey.Equals(Guid.Empty) || paymentMethod == null ? null : paymentMethod.PaymentMethod;
        }

        /// <summary>
        /// True/false indicating whether or not the <see cref="ISalePreparationBase"/> is ready to prepare an <see cref="IInvoice"/>
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public virtual bool IsReadyToInvoice()
        {
            return _customer.ExtendedData.GetAddress(AddressType.Billing) != null;
        }

        /// <summary>
        /// Adds a <see cref="ILineItem"/> to the collection of items
        /// </summary>
        /// <param name="lineItem">
        /// The line item.
        /// </param>
        /// <remarks>
        /// Intended for custom line item types
        /// http://issues.merchello.com/youtrack/issue/M-381
        /// </remarks>
        public void AddItem(ILineItem lineItem)
        {
            Mandate.ParameterNotNullOrEmpty(lineItem.Sku, "The line item must have a sku");
            Mandate.ParameterNotNullOrEmpty(lineItem.Name, "The line item must have a name");

            if (lineItem.Quantity <= 0) lineItem.Quantity = 1;
            if (lineItem.Price < 0) lineItem.Price = 0;

            if (lineItem.LineItemType == LineItemType.Custom)
            {
                if (!new LineItemTypeField().CustomTypeFields.Select(x => x.TypeKey).Contains(lineItem.LineItemTfKey))
                {
                    var argError = new ArgumentException("The LineItemTfKey was not found in merchello.config custom type fields");
                    LogHelper.Error<SalePreparationBase>("The LineItemTfKey was not found in merchello.config custom type fields", argError);

                    throw argError;
                }
            }

            _itemCache.AddItem(lineItem);
        }

        /// <summary>
        /// Removes a line item for the collection of items
        /// </summary>
        /// <param name="lineItem">
        /// The line item.
        /// </param>
        public void RemoveItem(ILineItem lineItem)
        {
            _itemCache.Items.Remove(lineItem.Sku);
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

            ////var requestCache = _merchelloContext.Cache.RequestCache;
            ////var cacheKey = string.Format("merchello.salespreparationbase.prepareinvoice.{0}", ItemCache.VersionKey);
            var attempt = invoiceBuilder.Build();

            if (attempt.Success)
            {
                attempt.Result.InvoiceNumberPrefix = InvoiceNumberPrefix;
                InvoicePrepared.RaiseEvent(new SalesPreparationEventArgs<IInvoice>(attempt.Result), this);

                return attempt.Result;
            }

            LogHelper.Error<SalePreparationBase>("The invoice builder failed to generate an invoice.", attempt.Exception);
            
            throw attempt.Exception;
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
        /// <param name="args">Additional arguments required by the payment processor</param>
        /// <returns>The <see cref="IPaymentResult"/></returns>
        public virtual IPaymentResult AuthorizePayment(IPaymentGatewayMethod paymentGatewayMethod, ProcessorArgumentCollection args)
        {
            Mandate.ParameterNotNull(paymentGatewayMethod, "paymentGatewayMethod");

            if (!IsReadyToInvoice()) return new PaymentResult(Attempt<IPayment>.Fail(new InvalidOperationException("SalesPreparation is not ready to invoice")), null, false);

            // invoice
            var invoice = PrepareInvoice(new InvoiceBuilderChain(this));

            MerchelloContext.Services.InvoiceService.Save(invoice);

            var result = invoice.AuthorizePayment(paymentGatewayMethod, args);

            Finalizing.RaiseEvent(new SalesPreparationEventArgs<IPaymentResult>(result), this);

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
        /// <param name="args">Additional arguments required by the payment processor</param>
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
        /// <param name="args">Additional arguments required by the payment processor</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        public virtual IPaymentResult AuthorizeCapturePayment(IPaymentGatewayMethod paymentGatewayMethod, ProcessorArgumentCollection args)
        {
            Mandate.ParameterNotNull(paymentGatewayMethod, "paymentGatewayMethod");

            if (!IsReadyToInvoice()) return new PaymentResult(Attempt<IPayment>.Fail(new InvalidOperationException("SalesPreparation is not ready to invoice")), null, false);

            // invoice
            var invoice = PrepareInvoice(new InvoiceBuilderChain(this));

            MerchelloContext.Services.InvoiceService.Save(invoice);

            var result = invoice.AuthorizeCapturePayment(paymentGatewayMethod, args);

            Finalizing.RaiseEvent(new SalesPreparationEventArgs<IPaymentResult>(result), this);

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
        /// <param name="args">Additional arguments required by the payment processor</param>
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
        /// Removes an offer code from the OfferCodes collection.
        /// </summary>
        /// <param name="offerCode">
        /// The offer code.
        /// </param>
        public void RemoveOfferCode(string offerCode)
        {
            if (OfferCodes.Contains(offerCode))
            {
                _offerCodeTempData.Value.Remove(offerCode);
                this.SaveOfferCodes();
            }
        }

        /// <summary>
        /// Clears the offer codes collection.
        /// </summary>
        public void ClearOfferCodes()
        {
            _offerCodeTempData.Value.Clear();
            this.SaveOfferCodes();
        }

        /// <summary>
        /// Attempts to apply an offer to the the checkout.
        /// </summary>
        /// <param name="validateAgainst">
        /// The object to validate against
        /// </param>
        /// <param name="offerCode">
        /// The offer code.
        /// </param>
        /// <typeparam name="TConstraint">
        /// The type of constraint
        /// </typeparam>
        /// <typeparam name="TAward">
        /// The type of award
        /// </typeparam>
        /// <returns>
        /// The <see cref="Attempt"/>.
        /// </returns>
        /// <remarks>
        /// Custom offer types
        /// </remarks>
        internal abstract Attempt<IOfferResult<TConstraint, TAward>> TryApplyOffer<TConstraint, TAward>(TConstraint validateAgainst, string offerCode)
            where TConstraint : class
            where TAward : class;


        /// <summary>
        /// Purges sales manager information
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello Context.
        /// </param>
        /// <param name="entityKey">
        /// The entity Key.
        /// </param>
        internal void Reset(IMerchelloContext merchelloContext, Guid entityKey)
        {
            var customer = merchelloContext.Services.CustomerService.GetAnyByKey(entityKey);

            if (customer == null) return;

            Reset(merchelloContext, customer, RaiseCustomerEvents);
        }

        /// <summary>
        /// Gets a clone of the ItemCache
        /// </summary>
        /// <returns>
        /// The <see cref="IItemCache"/>.
        /// </returns>
        internal IItemCache CloneItemCache()
        {
            // The ItemCache needs to be cloned as line items may be altered while applying constraints
            return this.CloneItemCache(ItemCache);
        }

        /// <summary>
        /// Clones a <see cref="ILineItemContainer"/> as <see cref="IItemCache"/>
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        /// <returns>
        /// The <see cref="IItemCache"/>.
        /// </returns>
        internal IItemCache CloneItemCache(ILineItemContainer container)
        {
            var clone = new ItemCache(Guid.NewGuid(), ItemCacheType.Backoffice);
            foreach (var item in container.Items)
            {
                clone.Items.Add(item.AsLineItemOf<ItemCacheLineItem>());
            }

            return clone;
        }

        /// <summary>
        /// Creates a new <see cref="ILineItemContainer"/> with filtered items.
        /// </summary>
        /// <param name="filteredItems">
        /// The line items.
        /// </param>
        /// <returns>
        /// The <see cref="ILineItemContainer"/>.
        /// </returns>
        internal ILineItemContainer CreateNewLineContainer(IEnumerable<ILineItem> filteredItems)
        {
            return LineItemExtensions.CreateNewItemCacheLineItemContainer(filteredItems);
        }

        /// <summary>
        /// Gets the checkout <see cref="IItemCache"/> for the <see cref="ICustomerBase"/>
        /// </summary>
        /// <param name="merchelloContext">
        /// The <see cref="IMerchelloContext"/>
        /// </param>
        /// <param name="customer">
        /// The customer associated with the checkout
        /// </param>
        /// <param name="versionKey">
        /// The version key for this <see cref="SalePreparationBase"/>
        /// </param>
        /// <returns>
        /// The <see cref="IItemCache"/> associated with the customer checkout
        /// </returns>
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
        /// <returns>
        /// The <see cref="string"/> cache key
        /// </returns>
        protected string MakeCacheKey()
        {
            return MakeCacheKey(_customer, _itemCache.VersionKey);
        }
        
        /// <summary>
        /// Saves offer code.
        /// </summary>
        /// <param name="offerCode">
        /// The offer code.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        protected bool SaveOfferCode(string offerCode)
        {
            if (!OfferCodes.Contains(offerCode))
            {
                _offerCodeTempData.Value.Add(offerCode);
                this.SaveOfferCodes();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Saves the current customer
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello Context.
        /// </param>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <param name="raiseEvents">
        /// The raise Events.
        /// </param>
        private static void SaveCustomer(IMerchelloContext merchelloContext, ICustomerBase customer, bool raiseEvents = true)
        {
            if (typeof(AnonymousCustomer) == customer.GetType())
            {
                merchelloContext.Services.CustomerService.Save(customer as AnonymousCustomer, raiseEvents);
            }
            else
            {
                ((CustomerService)merchelloContext.Services.CustomerService).Save(customer as Customer, raiseEvents);
            }
        }

        /// <summary>
        /// Purges persisted checkout information
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello Context.
        /// </param>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <param name="raiseEvents">
        /// The raise Events.
        /// </param>
        private static void Reset(IMerchelloContext merchelloContext, ICustomerBase customer, bool raiseEvents = true)
        {
            customer.ExtendedData.RemoveValue(Core.Constants.ExtendedDataKeys.ShippingDestinationAddress);
            customer.ExtendedData.RemoveValue(Core.Constants.ExtendedDataKeys.BillingAddress);
            SaveCustomer(merchelloContext, customer, raiseEvents);
        }

        /// <summary>
        /// Generates a unique cache key for runtime caching of the <see cref="SalePreparationBase"/>
        /// </summary>
        /// <param name="customer">The <see cref="ICustomerBase"/> for which to generate the cache key</param>
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
            return Cache.CacheKeys.ItemCacheCacheKey(customer.Key, itemCacheTfKey, versionKey);
        }

        #region Offer codes

        /// <summary>
        /// Saves the offer codes.
        /// </summary>
        private void SaveOfferCodes()
        {
            var json =
                JsonConvert.SerializeObject(
                    new OfferCodeTempData() { OfferCodes = OfferCodes, VersionKey = ItemCache.VersionKey });

            _customer.ExtendedData.SetValue(Core.Constants.ExtendedDataKeys.OfferCodeTempData, json);

            SaveCustomer(_merchelloContext, _customer, RaiseCustomerEvents);
        }

        /// <summary>
        /// Handles the instantiation of offer code queue.
        /// </summary>
        /// <returns>
        /// The <see cref="Queue{String}"/> offer codes.
        /// </returns>
        private List<string> BuildOfferCodeList()
        {
            var codes = new List<string>();
            var queueDataJson = Customer.ExtendedData.GetValue(Core.Constants.ExtendedDataKeys.OfferCodeTempData);
            if (string.IsNullOrEmpty(queueDataJson)) return codes;

            try
            {
                var savedData = JsonConvert.DeserializeObject<OfferCodeTempData>(queueDataJson);

                // verify that the offer codes are for this version of the checkout
                if (savedData.VersionKey != ItemCache.VersionKey) return codes;

                codes.AddRange(savedData.OfferCodes);
            }
            catch (Exception ex)
            {
                // don't throw an exception here as the customer is in the middle of a checkout.
                LogHelper.Error<SalePreparationBase>("Failed to deserialize OfferCodeTempData.  Returned empty offer code list instead.", ex);
            }

            return codes;
        }

        /// <summary>
        /// Class that gets serialized to customer's ExtendedDataCollection to save offer code queue data.
        /// </summary>
        private struct OfferCodeTempData
        {
            /// <summary>
            /// Gets or sets the version key to validate offer codes are validate with this preparation
            /// </summary>
            public Guid VersionKey { get; set; }

            /// <summary>
            /// Gets or sets the offer codes.
            /// </summary>
            public IEnumerable<string> OfferCodes { get; set; }
        }

        #endregion

        /// <summary>
        /// Maps the <see cref="IShipmentRateQuote"/> to a <see cref="ILineItem"/> 
        /// </summary>
        /// <param name="shipmentRateQuote">The <see cref="IShipmentRateQuote"/> to be added as a <see cref="ILineItem"/></param>
        private void AddShipmentRateQuoteLineItem(IShipmentRateQuote shipmentRateQuote)
        {
            var lineItem = shipmentRateQuote.AsLineItemOf<ItemCacheLineItem>();
            if (_shippingTaxable.Value) lineItem.ExtendedData.SetValue(Core.Constants.ExtendedDataKeys.Taxable, true.ToString());
            _itemCache.AddItem(lineItem);
        }

        /// <summary>
        /// Class initialization.
        /// </summary>
        private void Initialize()
        {
            var storeSettingsService = _merchelloContext.Services.StoreSettingService;
            var shippingTaxSetting = storeSettingsService.GetByKey(Core.Constants.StoreSettingKeys.GlobalShippingIsTaxableKey);
            _shippingTaxable = new Lazy<bool>(() => Convert.ToBoolean(shippingTaxSetting.Value));
            this._offerCodeTempData = new Lazy<List<string>>(this.BuildOfferCodeList);
        }

    }
}
