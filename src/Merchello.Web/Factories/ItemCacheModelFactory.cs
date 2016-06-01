namespace Merchello.Web.Factories
{
    using System.Linq;

    using Merchello.Core.Models;
    using Merchello.Web.Models.Ui;
    using Merchello.Web.Workflow;

    /// <summary>
    /// A factory responsible for creating Item Cache Models.
    /// </summary>
    /// <typeparam name="TItemCacheModel">
    /// The type of the item cache model
    /// </typeparam>
    /// <typeparam name="TLineItemModel">
    /// The type of the line item model
    /// </typeparam>
    public class ItemCacheModelFactory<TItemCacheModel, TLineItemModel>
        where TItemCacheModel : class, IItemCacheModel<TLineItemModel>, new()
        where TLineItemModel : class, ILineItemModel, new()
    {
        /// <summary>
        /// Creates an <see cref="IItemCacheModel{TLineItemModel}"/>.
        /// </summary>
        /// <param name="itemCache">
        /// The item cache.
        /// </param>
        /// <returns>
        /// The <see cref="IItemCacheModel{TLineItemModel}"/>.
        /// </returns>
        public TItemCacheModel Create(IItemCache itemCache)
        {
            var items = itemCache.Items.Select(this.Create).ToArray();

            var model = new TItemCacheModel
            {
                Items = items
            };

            return this.OnCreate(model, itemCache);
        }

        /// <summary>
        /// Creates <see cref="ILineItemModel"/> from <see cref="ILineItem"/>.
        /// </summary>
        /// <param name="lineItem">
        /// The <see cref="ILineItem"/>.
        /// </param>
        /// <returns>
        /// The <see cref="ILineItemModel"/>.
        /// </returns>
        public TLineItemModel Create(ILineItem lineItem)
        {
            var item = new TLineItemModel
            {
                Key = lineItem.Key,
                Name = lineItem.Name,
                Amount = lineItem.Price,
                Quantity = lineItem.Quantity,
                ExtendedData = lineItem.ExtendedData.AsEnumerable()
            };

            return this.OnCreate(item, lineItem);
        }

        /// <summary>
        /// Allows for overriding the creation of <see cref="IBasketModel{TBasketItemModel}"/> from <see cref="IBasket"/>.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="itemCache">
        /// The <see cref="IItemCache"/>.
        /// </param>
        /// <returns>
        /// The <see cref="ILineItemModel"/>.
        /// </returns>
        protected virtual TItemCacheModel OnCreate(TItemCacheModel model, IItemCache itemCache)
        {
            return model;
        }

        /// <summary>
        /// Allows for overriding the creation of <see cref="ILineItemModel"/> from <see cref="ILineItem"/>.
        /// </summary>
        /// <param name="item">
        /// The created item.
        /// </param>
        /// <param name="lineItem">
        /// The line item.
        /// </param>
        /// <returns>
        /// The <see cref="ILineItemModel"/>.
        /// </returns>
        protected virtual TLineItemModel OnCreate(TLineItemModel item, ILineItem lineItem)
        {
            return item;
        }
    }
}