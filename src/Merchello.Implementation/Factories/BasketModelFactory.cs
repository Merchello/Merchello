namespace Merchello.Implementation.Factories
{
    using System.Linq;

    using Merchello.Core.Models;
    using Merchello.Implementation.Models;
    using Merchello.Web.Workflow;

    /// <summary>
    /// A factory responsible for building <see cref="TBasketModel"/> and <see cref="TBasketItemModel"/>.
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
        /// Creates <see cref="TBasketModel"/> from <see cref="IBasket"/>.
        /// </summary>
        /// <param name="basket">
        /// The <see cref="IBasket"/>.
        /// </param>
        /// <returns>
        /// The <see cref="TBasketModel"/>.
        /// </returns>
        public TBasketModel Create(IBasket basket)
        {
            var basketItems = basket.Items.Select(Create).ToArray();

            var basketModel = new TBasketModel
            {
                WishListEnabled = false,
                Items = basketItems
            };

            return OnCreate(basketModel, basket);
        }

        /// <summary>
        /// Creates <see cref="TBasketItemModel"/> from <see cref="ILineItem"/>.
        /// </summary>
        /// <param name="lineItem">
        /// The <see cref="ILineItem"/>.
        /// </param>
        /// <returns>
        /// The <see cref="TBasketItemModel"/>.
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

            return OnCreate(basketItem, lineItem);
        }

        /// <summary>
        /// Allows for overriding the creation of <see cref="TBasketModel"/> from <see cref="IBasket"/>.
        /// </summary>
        /// <param name="basketModel">
        /// The <see cref="TBasketModel"/>.
        /// </param>
        /// <param name="basket">
        /// The <see cref="IBasket"/>.
        /// </param>
        /// <returns>
        /// The <see cref="TBasketItemModel"/>.
        /// </returns>
        protected virtual TBasketModel OnCreate(TBasketModel basketModel, IBasket basket)
        {
            return basketModel;
        }

        /// <summary>
        /// Allows for overriding the creation of <see cref="TBasketItemModel"/> from <see cref="ILineItem"/>.
        /// </summary>
        /// <param name="basketItem">
        /// The basket item.
        /// </param>
        /// <param name="lineItem">
        /// The line item.
        /// </param>
        /// <returns>
        /// The <see cref="TBasketItemModel"/>.
        /// </returns>
        protected virtual TBasketItemModel OnCreate(TBasketItemModel basketItem, ILineItem lineItem)
        {
            return basketItem;
        } 
    }
}