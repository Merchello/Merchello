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

    using Umbraco.Core;

    /// <summary>
    /// A factory responsible for building FastTrack the <see cref="StoreSummaryModel"/>.
    /// </summary>
    public class FastTrackCheckoutSummaryModelFactory : CheckoutSummaryModelFactory<FastTrackCheckoutSummaryModel, FastTrackBillingAddressModel, StoreAddressModel, StoreLineItemModel>
    {
        /// <summary>
        /// The <see cref="MerchelloHelper"/>.
        /// </summary>
        private readonly MerchelloHelper _merchello;

        /// <summary>
        /// Initializes a new instance of the <see cref="FastTrackCheckoutSummaryModelFactory"/> class.
        /// </summary>
        public FastTrackCheckoutSummaryModelFactory()
            : this(new MerchelloHelper())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FastTrackCheckoutSummaryModelFactory"/> class.
        /// </summary>
        /// <param name="merchello">
        /// The <see cref="MerchelloHelper"/>.
        /// </param>
        public FastTrackCheckoutSummaryModelFactory(MerchelloHelper merchello)
        {
            Ensure.ParameterNotNull(merchello, "merchello");
            _merchello = merchello;
        }

        /// <summary>
        /// Overrides the creation of <see cref="StoreLineItemModel"/>.
        /// </summary>
        /// <param name="lineItem">
        /// The <see cref="StoreLineItemModel"/>.
        /// </param>
        /// <param name="item">
        /// The <see cref="ILineItem"/>.
        /// </param>
        /// <returns>
        /// The modified <see cref="StoreLineItemModel"/>.
        /// </returns>
        protected override StoreLineItemModel OnCreate(StoreLineItemModel lineItem, ILineItem item)
        {

            if (item.LineItemType == LineItemType.Product)
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
            }

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