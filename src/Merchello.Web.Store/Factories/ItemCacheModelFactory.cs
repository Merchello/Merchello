namespace Merchello.Web.Store.Factories
{
    using System;
    using System.Linq;

    using Core.Models;

    using Merchello.Core;
    using Merchello.Web.Factories;
    using Merchello.Web.Models.Ui;
    using Merchello.Web.Models.VirtualContent;
    using Merchello.Web.Store.Models;

    using Umbraco.Core;

    /// <summary>
    /// A factory responsible for creating <see cref="StoreItemCacheModel"/>s.
    /// </summary>
    public class ItemCacheModelFactory : ItemCacheModelFactory<StoreItemCacheModel, StoreLineItemModel>
    {
        /// <summary>
        /// The <see cref="MerchelloHelper"/>.
        /// </summary>
        private readonly MerchelloHelper _merchello;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemCacheModelFactory"/> class.
        /// </summary>
        public ItemCacheModelFactory()
            : this(new MerchelloHelper())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemCacheModelFactory"/> class.
        /// </summary>
        /// <param name="merchello">
        /// The merchello.
        /// </param>
        public ItemCacheModelFactory(MerchelloHelper merchello)
        {
            Ensure.ParameterNotNull(merchello, "merchello");
            _merchello = merchello;
        }

        /// <summary>
        /// Overrides the creation of the <see cref="StoreItemCacheModel"/>.
        /// </summary>
        /// <param name="model">
        /// The <see cref="StoreItemCacheModel"/>.
        /// </param>
        /// <param name="itemCache">
        /// The original <see cref="IItemCache"/>.
        /// </param>
        /// <returns>
        /// The modified <see cref="StoreItemCacheModel"/>.
        /// </returns>
        protected override StoreItemCacheModel OnCreate(StoreItemCacheModel model, IItemCache itemCache)
        {
            // Ensure to order of the item cache items is in alphabetical order.
            model.Items = model.Items.OrderBy(x => x.Name).ToArray();

            return base.OnCreate(model, itemCache);
        }

        /// <summary>
        /// Overrides the base item cache item model creation.
        /// </summary>
        /// <param name="item">
        /// The <see cref="StoreLineItemModel"/>.
        /// </param>
        /// <param name="lineItem">
        /// The <see cref="ILineItem"/>.
        /// </param>
        /// <returns>
        /// The modified <see cref="StoreLineItemModel"/>.
        /// </returns>
        protected override StoreLineItemModel OnCreate(StoreLineItemModel item, ILineItem lineItem)
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
            item.Product = product;
            item.ProductKey = productKey;
            item.CustomerOptionChoices = customerChoices;

            return base.OnCreate(item, lineItem);
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