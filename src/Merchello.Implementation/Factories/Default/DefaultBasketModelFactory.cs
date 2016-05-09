namespace Merchello.Implementation.Factories
{
    using System;
    using System.Linq;

    using Core.Models;

    using Merchello.Core;
    using Merchello.Implementation.Models;
    using Merchello.Web;
    using Merchello.Web.Models.VirtualContent;

    using Umbraco.Core;

    using Web.Workflow;

    /// <summary>
    /// The basket model factory for the default implementation.
    /// </summary>
    public class DefaultBasketModelFactory : BasketModelFactory<BasketModel, BasketItemModel>
    {
        /// <summary>
        /// The <see cref="MerchelloHelper"/>.
        /// </summary>
        private readonly MerchelloHelper _merchello;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultBasketModelFactory"/> class.
        /// </summary>
        public DefaultBasketModelFactory()
            : this(new MerchelloHelper())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultBasketModelFactory"/> class.
        /// </summary>
        /// <param name="merchello">
        /// The <see cref="MerchelloHelper"/>.
        /// </param>
        public DefaultBasketModelFactory(MerchelloHelper merchello)
        {
            Mandate.ParameterNotNull(merchello, "merchello");
            _merchello = merchello;
        }

        /// <summary>
        /// Overrides the base basket model creation.
        /// </summary>
        /// <param name="basketModel">
        /// The <see cref="BasketModel"/>.
        /// </param>
        /// <param name="basket">
        /// The <see cref="IBasket"/>.
        /// </param>
        /// <returns>
        /// The modified <see cref="BasketModel"/>.
        /// </returns>
        protected override BasketModel OnCreate(BasketModel basketModel, IBasket basket)
        {
            // Ensure to order of the basket items is in alphabetical order.
            basketModel.Items = basketModel.Items.OrderBy(x => x.Name).ToArray();

            return base.OnCreate(basketModel, basket);
        }

        /// <summary>
        /// Overrides the base basket item model creation.
        /// </summary>
        /// <param name="basketItem">
        /// The <see cref="BasketItemModel"/>.
        /// </param>
        /// <param name="lineItem">
        /// The <see cref="ILineItem"/>.
        /// </param>
        /// <returns>
        /// The modified <see cref="BasketItemModel"/>.
        /// </returns>
        protected override BasketItemModel OnCreate(BasketItemModel basketItem, ILineItem lineItem)
        {
            // Get the product key from the extended data collection
            // This is added internally when the product was added to the basket
            var productKey = lineItem.ExtendedData.GetProductKey();

            // Get an instantiated IProductContent for use in the basket table design
            var product = lineItem.LineItemType == LineItemType.Product ?
                            GetProductContent(productKey) :
                            null;

            // Get a list of choices the customer made.  This can also be done by looking at the variant (Attributes)
            // but this is a bit quicker and is something commonly done.
            var customerChoices = lineItem.GetProductOptionChoicePairs();

            // Modifiy the BasketItemModel generated in the base factory
            basketItem.Product = product;
            basketItem.ProductKey = productKey;
            basketItem.CustomerOptionChoices = customerChoices;

            return base.OnCreate(basketItem, lineItem);
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
            return _merchello.TypedProductContent(productKey);
        }
    }
}