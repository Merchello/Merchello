namespace Merchello.Web.Factories
{
    using System.Linq;

    using Merchello.Core.Models;
    using Merchello.Web.Models.Ui;
    using Merchello.Web.Workflow;

    /// <summary>
    /// A factory responsible for building <see cref="IBasketModel{TBasketItemModel}"/> and <see cref="ILineItemModel"/>.
    /// </summary>
    /// <typeparam name="TBasketModel">
    /// The type of <see cref="IBasketModel{TBasketItemModel}"/>
    /// </typeparam>
    /// <typeparam name="TBasketItemModel">
    /// The type of <see cref="ILineItemModel"/>
    /// </typeparam>
    public class BasketModelFactory<TBasketModel, TBasketItemModel>
        where TBasketModel : class, IBasketModel<TBasketItemModel>, new()
        where TBasketItemModel : class, ILineItemModel, new()
    {
        /// <summary>
        /// Creates <see cref="IBasketModel{TBasketItemModel}"/> from <see cref="IBasket"/>.
        /// </summary>
        /// <param name="basket">
        /// The <see cref="IBasket"/>.
        /// </param>
        /// <returns>
        /// The <see cref="IBasketModel{TBasketItemModel}"/>.
        /// </returns>
        public TBasketModel Create(IBasket basket)
        {
            var basketItems = basket.Items.Select(this.Create).ToArray();

            var basketModel = new TBasketModel
            {
                WishListEnabled = false,
                Items = basketItems
            };

            return this.OnCreate(basketModel, basket);
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
        public TBasketItemModel Create(ILineItem lineItem)
        {
            var basketItem = new TBasketItemModel
                {
                    Key = lineItem.Key,
                    Name = lineItem.Name,
                    Amount = lineItem.Price,
                    Quantity = lineItem.Quantity
                };

            return this.OnCreate(basketItem, lineItem);
        }

        /// <summary>
        /// Allows for overriding the creation of <see cref="IBasketModel{TBasketItemModel}"/> from <see cref="IBasket"/>.
        /// </summary>
        /// <param name="basketModel">
        /// The <see cref="IBasketModel{TBasketItemModel}"/>.
        /// </param>
        /// <param name="basket">
        /// The <see cref="IBasket"/>.
        /// </param>
        /// <returns>
        /// The <see cref="ILineItemModel"/>.
        /// </returns>
        protected virtual TBasketModel OnCreate(TBasketModel basketModel, IBasket basket)
        {
            return basketModel;
        }

        /// <summary>
        /// Allows for overriding the creation of <see cref="ILineItemModel"/> from <see cref="ILineItem"/>.
        /// </summary>
        /// <param name="basketItem">
        /// The basket item.
        /// </param>
        /// <param name="lineItem">
        /// The line item.
        /// </param>
        /// <returns>
        /// The <see cref="ILineItemModel"/>.
        /// </returns>
        protected virtual TBasketItemModel OnCreate(TBasketItemModel basketItem, ILineItem lineItem)
        {
            return basketItem;
        } 
    }
}