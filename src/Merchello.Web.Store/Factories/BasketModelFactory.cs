namespace Merchello.Web.Store.Factories
{
    using System;
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Models;
    using Merchello.Web;
    using Merchello.Web.Factories;
    using Merchello.Web.Models.Ui;
    using Merchello.Web.Models.VirtualContent;
    using Merchello.Web.Store.Models;
    using Merchello.Web.Workflow;

    using Umbraco.Core;

    /// <summary>
    /// The basket model factory for the default implementation.
    /// </summary>
    public class BasketModelFactory : BasketModelFactory<StoreBasketModel, StoreLineItemModel>
    {
        /// <summary>
        /// The <see cref="MerchelloHelper"/>.
        /// </summary>
        private readonly MerchelloHelper _merchello;

        /// <summary>
        /// Initializes a new instance of the <see cref="BasketModelFactory"/> class.
        /// </summary>
        public BasketModelFactory()
            : this(new MerchelloHelper())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BasketModelFactory"/> class.
        /// </summary>
        /// <param name="merchello">
        /// The <see cref="MerchelloHelper"/>.
        /// </param>
        public BasketModelFactory(MerchelloHelper merchello)
        {
            Mandate.ParameterNotNull(merchello, "merchello");
            this._merchello = merchello;
        }

        /// <summary>
        /// Overrides the base basket model creation.
        /// </summary>
        /// <param name="basketModel">
        /// The <see cref="StoreBasketModel"/>.
        /// </param>
        /// <param name="basket">
        /// The <see cref="IBasket"/>.
        /// </param>
        /// <returns>
        /// The modified <see cref="StoreBasketModel"/>.
        /// </returns>
        protected override StoreBasketModel OnCreate(StoreBasketModel basketModel, IBasket basket)
        {
            // Ensure to order of the basket items is in alphabetical order.
            basketModel.Items = basketModel.Items.OrderBy(x => x.Name).ToArray();

            return base.OnCreate(basketModel, basket);
        }

        /// <summary>
        /// Overrides the base basket item model creation.
        /// </summary>
        /// <param name="storeLineItem">
        /// The <see cref="StoreLineItemModel"/>.
        /// </param>
        /// <param name="lineItem">
        /// The <see cref="ILineItem"/>.
        /// </param>
        /// <returns>
        /// The modified <see cref="StoreLineItemModel"/>.
        /// </returns>
        protected override StoreLineItemModel OnCreate(StoreLineItemModel storeLineItem, ILineItem lineItem)
        {
            // Get the product key from the extended data collection
            // This is added internally when the product was added to the basket
            var productKey = lineItem.ExtendedData.GetProductKey();

            // Get an instantiated IProductContent for use in the basket table design
            var product = lineItem.LineItemType == LineItemType.Product ?
                            this.GetProductContent(productKey) :
                            null;

            // Get a list of choices the customer made.  This can also be done by looking at the variant (Attributes)
            // but this is a bit quicker and is something commonly done.
            var customerChoices = lineItem.GetProductOptionChoicePairs();

            // Modifiy the BasketItemModel generated in the base factory
            storeLineItem.Product = product;
            storeLineItem.ProductKey = productKey;
            storeLineItem.CustomerOptionChoices = customerChoices;

            return base.OnCreate(storeLineItem, lineItem);
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
        private IProductContent GetProductContent(Guid productKey)
        {
            if (productKey.Equals(Guid.Empty)) return null;
            return this._merchello.TypedProductContent(productKey);
        }
    }
}