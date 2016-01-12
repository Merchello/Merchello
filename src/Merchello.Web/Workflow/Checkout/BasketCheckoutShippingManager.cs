namespace Merchello.Web.Workflow.Checkout
{
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Checkout;
    using Merchello.Core.Gateways.Shipping;
    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// Represents a CheckoutShippingManager for basket checkouts.
    /// </summary>
    internal class BasketCheckoutShippingManager : CheckoutShippingManagerBase, ICheckoutShippingManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BasketCheckoutShippingManager"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public BasketCheckoutShippingManager(ICheckoutContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Saves a <see cref="IShipmentRateQuote"/> as a shipment line item
        /// </summary>
        /// <param name="approvedShipmentRateQuote">
        /// The <see cref="IShipmentRateQuote"/> to be saved
        /// </param>
        public override void SaveShipmentRateQuote(IShipmentRateQuote approvedShipmentRateQuote)
        {
            this.AddShipmentRateQuoteLineItem(approvedShipmentRateQuote);
            Context.Services.ItemCacheService.Save(Context.ItemCache);

            Context.Customer.ExtendedData.AddAddress(approvedShipmentRateQuote.Shipment.GetDestinationAddress(), AddressType.Shipping);
            SaveCustomer();
        }

        /// <summary>
        /// Saves a collection of <see cref="IShipmentRateQuote"/>s as shipment line items
        /// </summary>
        /// <param name="approvedShipmentRateQuotes">
        /// The collection of <see cref="IShipmentRateQuote"/>s to be saved
        /// </param>
        public override void SaveShipmentRateQuote(IEnumerable<IShipmentRateQuote> approvedShipmentRateQuotes)
        {
            var shipmentRateQuotes = approvedShipmentRateQuotes as IShipmentRateQuote[] ?? approvedShipmentRateQuotes.ToArray();

            if (!shipmentRateQuotes.Any()) return;

            shipmentRateQuotes.ForEach(AddShipmentRateQuoteLineItem);
            Context.Services.ItemCacheService.Save(Context.ItemCache);

            Context.Customer.ExtendedData.AddAddress(shipmentRateQuotes.First().Shipment.GetDestinationAddress(), AddressType.Shipping);
            SaveCustomer();
        }

        /// <summary>
        /// The clear shipment rate quotes.
        /// </summary>
        public override void ClearShipmentRateQuotes()
        {
            var items = Context.ItemCache.Items.Where(x => x.LineItemType == LineItemType.Shipping).ToArray();

            foreach (var item in items)
            {
                Context.ItemCache.Items.RemoveItem(item.Sku);
            }

            Context.Services.ItemCacheService.Save(Context.ItemCache);
        }
    }
}