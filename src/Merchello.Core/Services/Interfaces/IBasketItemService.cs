using System.Collections.Generic;
using Merchello.Core.Models;
using Umbraco.Core.Services;

namespace Merchello.Core.Services
{
    /// <summary>
    /// Defines the AddressService, which provides access to operations involving <see cref="IPurchaseLineItem"/>
    /// </summary>
    public interface IBasketItemService : IService
    {
        /// <summary>
        /// Creates a BasketItem
        /// </summary>
        IPurchaseLineItem CreateBasketItem(ICustomerRegistry customerRegistry, string sku, string name, int baseQuantity, int unitOfMeasureMultiplier, decimal amount);

        ///// <summary>
        ///// Creates a BasketItem
        ///// </summary>
        //IBasketItem CreateBasketItem(IBasket basket, Guid invoiceItemTypeFieldKey, string sku, string name, int baseQuantity, int unitOfMeasureMultiplier, decimal amount);

        /// <summary>
        /// Saves a single <see cref="IPurchaseLineItem"/> object
        /// </summary>
        /// <param name="purchaseLineItem">The <see cref="IPurchaseLineItem"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Save(IPurchaseLineItem purchaseLineItem, bool raiseEvents = true);

        /// <summary>
        /// Saves a collection of <see cref="IPurchaseLineItem"/> objects
        /// </summary>
        /// <param name="basketItemList">Collection of <see cref="IPurchaseLineItem"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Save(IEnumerable<IPurchaseLineItem> basketItemList, bool raiseEvents = true);

        /// <summary>
        /// Deletes a single <see cref="IPurchaseLineItem"/> object
        /// </summary>
        /// <param name="purchaseLineItem"><see cref="IPurchaseLineItem"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Delete(IPurchaseLineItem purchaseLineItem, bool raiseEvents = true);

        /// <summary>
        /// Deletes a collection of <see cref="IPurchaseLineItem"/> objects
        /// </summary>
        /// <param name="basketItemList">Collection of <see cref="IPurchaseLineItem"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Delete(IEnumerable<IPurchaseLineItem> basketItemList, bool raiseEvents = true);

        /// <summary>
        /// Gets an <see cref="IPurchaseLineItem"/> object by its 'UniqueId'
        /// </summary>
        /// <param name="id">int Id of the BasketItem to retrieve</param>
        /// <returns><see cref="IPurchaseLineItem"/></returns>
        IPurchaseLineItem GetById(int id);

        /// <summary>
        /// Gets list of <see cref="IPurchaseLineItem"/> objects given a list of Unique keys
        /// </summary>
        /// <param name="ids">List of int Id for BasketItem objects to retrieve</param>
        /// <returns>List of <see cref="IPurchaseLineItem"/></returns>
        IEnumerable<IPurchaseLineItem> GetByIds(IEnumerable<int> ids);

    }
}
