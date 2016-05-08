namespace Merchello.Implementation.Factories
{
    using System;

    using Core.Models;

    using Merchello.Core;
    using Merchello.Implementation.Models;
    using Merchello.Web;
    using Merchello.Web.Models.VirtualContent;

    using Umbraco.Core;

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
        /// The on create.
        /// </summary>
        /// <param name="basketItem">
        /// The basket item.
        /// </param>
        /// <param name="lineItem">
        /// The line item.
        /// </param>
        /// <returns>
        /// The <see cref="BasketItemModel"/>.
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

            return basketItem;
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