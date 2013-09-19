using System.Collections.Generic;
using Merchello.Core.Models;
using Umbraco.Core.Services;

namespace Merchello.Core.Services
{
    /// <summary>
    /// Defines the AddressService, which provides access to operations involving <see cref="IInvoiceItem"/>
    /// </summary>
    public interface IInvoiceItemService : IService
    {

        /// <summary>
        /// Creates a InvoiceItem
        /// </summary>
        IInvoiceItem CreateInvoiceItem(IInvoice invoice, InvoiceItemType invoiceItemType, string sku, string name, int baseQuantity, int unitOfMeasure, decimal amount, int? parentId = null);

        /// <summary>
        /// Saves a single <see cref="IInvoiceItem"/> object
        /// </summary>
        /// <param name="invoiceItem">The <see cref="IInvoiceItem"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Save(IInvoiceItem invoiceItem, bool raiseEvents = true);

        /// <summary>
        /// Saves a collection of <see cref="IInvoiceItem"/> objects
        /// </summary>
        /// <param name="invoiceItemList">Collection of <see cref="IInvoiceItem"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Save(IEnumerable<IInvoiceItem> invoiceItemList, bool raiseEvents = true);

        /// <summary>
        /// Deletes a single <see cref="IInvoiceItem"/> object
        /// </summary>
        /// <param name="invoiceItem"><see cref="IInvoiceItem"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Delete(IInvoiceItem invoiceItem, bool raiseEvents = true);

        /// <summary>
        /// Deletes a collection of <see cref="IInvoiceItem"/> objects
        /// </summary>
        /// <param name="invoiceItemList">Collection of <see cref="IInvoiceItem"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Delete(IEnumerable<IInvoiceItem> invoiceItemList, bool raiseEvents = true);

        /// <summary>
        /// Gets an <see cref="IInvoiceItem"/> object by its 'UniqueId'
        /// </summary>
        /// <param name="id">int Id of the InvoiceItem to retrieve</param>
        /// <returns><see cref="IInvoiceItem"/></returns>
        IInvoiceItem GetById(int id);

        /// <summary>
        /// Gets list of <see cref="IInvoiceItem"/> objects given a list of Unique keys
        /// </summary>
        /// <param name="ids">List of int Id for InvoiceItem objects to retrieve</param>
        /// <returns>List of <see cref="IInvoiceItem"/></returns>
        IEnumerable<IInvoiceItem> GetByIds(IEnumerable<int> ids);

        /// <summary>
        /// Gets a list of <see cref="IInvoiceItem"/> objects for a given invoice id
        /// </summary>
        /// <param name="invoiceId">The id of the invoice</param>
        /// <returns>A collection of <see cref="IInvoiceItem"/></returns>
        IEnumerable<IInvoiceItem> GetInvoiceItemsForInvoice(int invoiceId);
    }
}
