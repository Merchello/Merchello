using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Models;
using Merchello.Core.Models.TypeFields;
using Umbraco.Core.Services;

namespace Merchello.Core.Services
{
    /// <summary>
    /// Defines the AddressService, which provides access to operations involving <see cref="IBasketItem"/>
    /// </summary>
    public interface IBasketItemService : IService
    {
        /// <summary>
        /// Creates a BasketItem
        /// </summary>
        IBasketItem CreateBasketItem(IBasket basket, string sku, string name, int baseQuantity, int unitOfMeasureMultiplier, decimal amount);

        ///// <summary>
        ///// Creates a BasketItem
        ///// </summary>
        //IBasketItem CreateBasketItem(IBasket basket, Guid invoiceItemTypeFieldKey, string sku, string name, int baseQuantity, int unitOfMeasureMultiplier, decimal amount);

        /// <summary>
        /// Saves a single <see cref="IBasketItem"/> object
        /// </summary>
        /// <param name="basketItem">The <see cref="IBasketItem"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Save(IBasketItem basketItem, bool raiseEvents = true);

        /// <summary>
        /// Saves a collection of <see cref="IBasketItem"/> objects
        /// </summary>
        /// <param name="basketItemList">Collection of <see cref="IBasketItem"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Save(IEnumerable<IBasketItem> basketItemList, bool raiseEvents = true);

        /// <summary>
        /// Deletes a single <see cref="IBasketItem"/> object
        /// </summary>
        /// <param name="basketItem"><see cref="IBasketItem"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Delete(IBasketItem basketItem, bool raiseEvents = true);

        /// <summary>
        /// Deletes a collection of <see cref="IBasketItem"/> objects
        /// </summary>
        /// <param name="basketItemList">Collection of <see cref="IBasketItem"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Delete(IEnumerable<IBasketItem> basketItemList, bool raiseEvents = true);

        /// <summary>
        /// Gets an <see cref="IBasketItem"/> object by its 'UniqueId'
        /// </summary>
        /// <param name="id">int Id of the BasketItem to retrieve</param>
        /// <returns><see cref="IBasketItem"/></returns>
        IBasketItem GetById(int id);

        /// <summary>
        /// Gets list of <see cref="IBasketItem"/> objects given a list of Unique keys
        /// </summary>
        /// <param name="ids">List of int Id for BasketItem objects to retrieve</param>
        /// <returns>List of <see cref="IBasketItem"/></returns>
        IEnumerable<IBasketItem> GetByIds(IEnumerable<int> ids);

    }
}
