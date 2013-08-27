using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Models;
using Umbraco.Core.Services;

namespace Merchello.Core.Services
{
    /// <summary>
    /// Defines the BasketService, which provides access to operations involving <see cref="IBasket"/>
    /// </summary>
    public interface IBasketService : IService
    {
        /// <summary>
        /// Creates a Basket
        /// </summary>
        IBasket CreateBasket(IConsumer consumer, BasketType basketType);

        /// <summary>
        /// Saves a single <see cref="IAddress"/> object
        /// </summary>
        /// <param name="basket">The <see cref="IBasket"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Save(IBasket basket, bool raiseEvents = true);

        /// <summary>
        /// Saves a collection of <see cref="IBasket"/> objects
        /// </summary>
        /// <param name="baskets"></param>
        /// <param name="raiseEvents"></param>
        void Save(IEnumerable<IBasket> baskets, bool raiseEvents = true);

        /// <summary>
        /// Deletes a single <see cref="IBasket"/> object
        /// </summary>
        /// <param name="basket"><see cref="IBasket"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Delete(IBasket basket, bool raiseEvents = true);

        /// <summary>
        /// Deletes a collection of <see cref="IAddress"/> objects
        /// </summary>
        /// <param name="baskets">Collection of <see cref="IBasket"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Delete(IEnumerable<IBasket> baskets, bool raiseEvents = true);

        /// <summary>
        /// Gets an <see cref="IBasket"/> object by its Id
        /// </summary>
        /// <param name="id">int Id of the Address to retrieve</param>
        /// <returns><see cref="IBasket"/></returns>
        IBasket GetById(int id);

        /// <summary>
        /// Gets an <see cref="IBasket"/> object by the <see cref="IConsumer"/>
        /// </summary>
        /// <param name="consumer">The <see cref="IConsumer"/> object</param>
        /// <param name="basketType"></param>
        /// <returns><see cref="IBasket"/></returns>
        IBasket GetByConsumer(IConsumer consumer, BasketType basketType);

        /// <summary>
        /// Gets a collection of <see cref="IBasket"/> objects by teh <see cref="IConsumer"/>
        /// </summary>
        /// <param name="consumer"></param>
        /// <returns></returns>
        IEnumerable<IBasket> GetByConsumer(IConsumer consumer); 
            
        /// <summary>
        /// Gets list of <see cref="IBasket"/> objects given a list of Ids
        /// </summary>
        /// <param name="ids">List of int Id for Baskets to retrieve</param>
        /// <returns>List of <see cref="IBasket"/></returns>
        IEnumerable<IBasket> GetByIds(IEnumerable<int> ids);

        /// <summary>
        /// Returns a collection of <see cref="IBasketItem"/>
        /// </summary>
        IEnumerable<IBasketItem> GetBasketItems(int basketId);

        /// <summary>
        /// Adds a <see cref="IBasketItem"/> to the basket
        /// </summary>
        /// <param name="basketItem"><see cref="IBasketItem"/></param>
        void AddBasketItem(IBasketItem basketItem);

        /// <summary>
        /// Removes a <see cref="IBasketItem"/> from the basket
        /// </summary>
        /// <param name="basketItem"><see cref="IBasketItem"/></param>
        void RemoveBasketItem(IBasketItem basketItem);

        /// <summary>
        /// Removes all <see cref="IBasketItem"/> from the <see cref="IBasket"/>
        /// </summary>
        void Empty(IBasket basket);
    }
}
