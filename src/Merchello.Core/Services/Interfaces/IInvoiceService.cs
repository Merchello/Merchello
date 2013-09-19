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
    /// Defines the AddressService, which provides access to operations involving <see cref="IInvoice"/>
    /// </summary>
    public interface IInvoiceService : IService
    {

        /// <summary>
        /// Creates an Invoice
        /// </summary>
        IInvoice CreateInvoice(ICustomer customer, IInvoiceStatus invoiceStatus, string invoiceNumber, string billToName, string billToAddress1, string billToAddress2, string billToLocality, string billToRegion, string billToPostalCode, string billToCountryCode, string billToEmail, string billToPhone, string billToCompany);

        /// <summary>
        /// Creates an Invoice
        /// </summary>        
        IInvoice CreateInvoice(ICustomer customer, IAddress address, IInvoiceStatus invoiceStatus, string invoiceNumber);

        /// <summary>
        /// Saves a single <see cref="IInvoice"/> object
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Save(IInvoice invoice, bool raiseEvents = true);

        /// <summary>
        /// Saves a collection of <see cref="IInvoice"/> objects
        /// </summary>
        /// <param name="invoiceList">Collection of <see cref="IInvoice"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Save(IEnumerable<IInvoice> invoiceList, bool raiseEvents = true);

        /// <summary>
        /// Deletes a single <see cref="IInvoice"/> object
        /// </summary>
        /// <param name="invoice"><see cref="IInvoice"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Delete(IInvoice invoice, bool raiseEvents = true);

        /// <summary>
        /// Deletes a collection of <see cref="IInvoice"/> objects
        /// </summary>
        /// <param name="invoiceList">Collection of <see cref="IInvoice"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Delete(IEnumerable<IInvoice> invoiceList, bool raiseEvents = true);

        /// <summary>
        /// Gets an <see cref="IInvoice"/> object by its 'UniqueId'
        /// </summary>
        /// <param name="id">int Id of the Invoice to retrieve</param>
        /// <returns><see cref="IInvoice"/></returns>
        IInvoice GetById(int id);

        /// <summary>
        /// Gets list of <see cref="IInvoice"/> objects given a list of Unique keys
        /// </summary>
        /// <param name="ids">List of int Id for Invoice objects to retrieve</param>
        /// <returns>List of <see cref="IInvoice"/></returns>
        IEnumerable<IInvoice> GetByIds(IEnumerable<int> ids);

        /// <summary>
        /// Gets a list of <see cref="IInvoice"/> objects given a customer key
        /// </summary>
        /// <param name="key">Unique customer key (Guid)</param>
        /// <returns>A collection of <see cref="IInvoice"/></returns>
        IEnumerable<IInvoice> GetInvoicesForCustomer(Guid key);

        /// <summary>
        /// Gets a list of <see cref="IInvoice"/> objects given an invoice status id
        /// </summary>
        /// <param name="invoiceStatusId">The id of the <see cref="IInvoiceStatus"/></param>
        /// <returns>A collection of <see cref="IInvoiceStatus"/></returns>
        IEnumerable<IInvoice> GetInvoicesByInvoiceStatus(int invoiceStatusId);
    }
}
