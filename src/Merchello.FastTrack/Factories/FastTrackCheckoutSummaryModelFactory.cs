namespace Merchello.FastTrack.Factories
{
    using System;

    using Core.Models;

    using Merchello.Core;
    using Merchello.FastTrack.Models;
    using Merchello.Web;
    using Merchello.Web.Factories;
    using Merchello.Web.Models.Ui;
    using Merchello.Web.Models.VirtualContent;
    using Merchello.Web.Store.Models;

    /// <summary>
    /// A factory responsible for building FastTrack the <see cref="CheckoutSummaryModel"/>.
    /// </summary>
    public class FastTrackCheckoutSummaryModelFactory : CheckoutSummaryModelFactory<FastTrackCheckoutSummary, FastTrackBillingAddressModel, CheckoutAddressModel, BasketItemModel>
    {
        /// <summary>
        /// The <see cref="MerchelloHelper"/>.
        /// </summary>
        /// <remarks>
        /// TODO move this to a base factory
        /// </remarks>
        private readonly MerchelloHelper _merchello;

        /// <summary>
        /// Overrides the creation of <see cref="BasketItemModel"/>.
        /// </summary>
        /// <param name="lineItem">
        /// The <see cref="BasketItemModel"/>.
        /// </param>
        /// <param name="item">
        /// The <see cref="ILineItem"/>.
        /// </param>
        /// <returns>
        /// The modified <see cref="BasketItemModel"/>.
        /// </returns>
        protected override BasketItemModel OnCreate(BasketItemModel lineItem, ILineItem item)
        {
            // Get the product key from the extended data collection
            // This is added internally when the product was added to the basket
            var productKey = item.ExtendedData.GetProductKey();

            // Get an instantiated IProductContent for use in the basket table design
            var product = item.LineItemType == LineItemType.Product ?
                            this.GetProductContent(productKey) :
                            null;

            // Get a list of choices the customer made.  This can also be done by looking at the variant (Attributes)
            // but this is a bit quicker and is something commonly done.
            var customerChoices = item.GetProductOptionChoicePairs();

            // Modifiy the BasketItemModel generated in the base factory
            lineItem.Product = product;
            lineItem.ProductKey = productKey;
            lineItem.CustomerOptionChoices = customerChoices;

            return base.OnCreate(lineItem, item);
        }


        /// <summary>
        /// Gets the <see cref="IProductContent"/>.
        /// </summary>
        /// <param name="productKey">
        /// The product key.
        /// </param>
        /// <returns>
        /// The <see cref="IProductContent"/>.
        /// </returns>
        /// <remarks>
        /// Move this to a base "store" factory
        /// </remarks>
        private IProductContent GetProductContent(Guid productKey)
        {
            if (productKey.Equals(Guid.Empty)) return null;
            return this._merchello.TypedProductContent(productKey);
        }
    }
}