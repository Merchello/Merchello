namespace Merchello.Core.Checkout
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Gateways.Shipping;
    using Merchello.Core.Models;

    /// <summary>
    /// A base class for CheckoutShippingManagers.
    /// </summary>
    public abstract class CheckoutShippingManagerBase : CheckoutCustomerDataManagerBase, ICheckoutShippingManager
    {
        /// <summary>
        /// A value indicating whether or not shipping charges are taxable.
        /// </summary>
        /// <remarks>
        /// Determined by the global back office setting.
        /// </remarks>
        private Lazy<bool> _shippingTaxable;

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutShippingManagerBase"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        protected CheckoutShippingManagerBase(ICheckoutContext context)
            : base(context)
        {
            this.Initialize();
        }

        /// <summary>
        /// Gets a value indicating whether or not shipping is taxable.
        /// </summary>
        protected virtual bool ShippingIsTaxable
        {
            get
            {
                return _shippingTaxable.Value;
            }
        } 

        /// <summary>
        /// Saves a <see cref="IShipmentRateQuote"/> as a shipment line item
        /// </summary>
        /// <param name="approvedShipmentRateQuote">
        /// The <see cref="IShipmentRateQuote"/> to be saved
        /// </param>
        public abstract void SaveShipmentRateQuote(IShipmentRateQuote approvedShipmentRateQuote);

        /// <summary>
        /// Saves a collection of <see cref="IShipmentRateQuote"/>s as shipment line items
        /// </summary>
        /// <param name="approvedShipmentRateQuotes">
        /// The collection of <see cref="IShipmentRateQuote"/>s to be saved
        /// </param>
        public abstract void SaveShipmentRateQuote(IEnumerable<IShipmentRateQuote> approvedShipmentRateQuotes);

        /// <summary>
        /// Clears all <see cref="IShipmentRateQuote"/>s previously saved
        /// </summary>
        public abstract void ClearShipmentRateQuotes();

        /// <summary>
        /// Clears the shipment rate quotes.
        /// </summary>
        public override void Reset()
        {
            ClearShipmentRateQuotes();
        }

        /// <summary>
        /// Maps the <see cref="IShipmentRateQuote"/> to a <see cref="ILineItem"/> 
        /// </summary>
        /// <param name="shipmentRateQuote">The <see cref="IShipmentRateQuote"/> to be added as a <see cref="ILineItem"/></param>
        protected virtual void AddShipmentRateQuoteLineItem(IShipmentRateQuote shipmentRateQuote)
        {
            var lineItem = shipmentRateQuote.AsLineItemOf<ItemCacheLineItem>();
            if (_shippingTaxable.Value) lineItem.ExtendedData.SetValue(Core.Constants.ExtendedDataKeys.Taxable, true.ToString());
            Context.ItemCache.AddItem(lineItem);
        }

        /// <summary>
        /// Initializes the manager.
        /// </summary>
        private void Initialize()
        {
            _shippingTaxable = new Lazy<bool>(() => Convert.ToBoolean(Context.Services.StoreSettingService.GetByKey(Constants.StoreSetting.GlobalShippingIsTaxableKey).Value));

            if (Context.IsNewVersion && Context.Settings.ResetShippingManagerDataOnVersionChange)
            {
                this.ClearShipmentRateQuotes();
            }
        }
    }
}