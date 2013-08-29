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
    /// Defines the AddressService, which provides access to operations involving <see cref="IInvoiceStatus"/>
    /// </summary>
    public interface IInvoiceStatusService : IService
    {

        /// <summary>
        /// Creates a InvoiceStatus
        /// </summary>
        IInvoiceStatus CreateInvoiceStatus(string name, string alias, bool reportable, bool active, int sortOrder);

        /// <summary>
        /// Saves a single <see cref="IInvoiceStatus"/> object
        /// </summary>
        /// <param name="invoiceStatus">The <see cref="IInvoiceStatus"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Save(IInvoiceStatus invoiceStatus, bool raiseEvents = true);

        /// <summary>
        /// Saves a collection of <see cref="IInvoiceStatus"/> objects
        /// </summary>
        /// <param name="invoiceStatusList">Collection of <see cref="IInvoiceStatus"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Save(IEnumerable<IInvoiceStatus> invoiceStatusList, bool raiseEvents = true);

        /// <summary>
        /// Deletes a single <see cref="IInvoiceStatus"/> object
        /// </summary>
        /// <param name="invoiceStatus"><see cref="IInvoiceStatus"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Delete(IInvoiceStatus invoiceStatus, bool raiseEvents = true);

        /// <summary>
        /// Deletes a collection of <see cref="IInvoiceStatus"/> objects
        /// </summary>
        /// <param name="invoiceStatusList">Collection of <see cref="IInvoiceStatus"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Delete(IEnumerable<IInvoiceStatus> invoiceStatusList, bool raiseEvents = true);

        /// <summary>
        /// Gets an <see cref="IInvoiceStatus"/> object by its 'UniqueId'
        /// </summary>
        /// <param name="id">int Id of the InvoiceStatus to retrieve</param>
        /// <returns><see cref="IInvoiceStatus"/></returns>
        IInvoiceStatus GetById(int id);

        /// <summary>
        /// Gets list of <see cref="IInvoiceStatus"/> objects given a list of Unique keys
        /// </summary>
        /// <param name="ids">List of int Id for InvoiceStatus objects to retrieve</param>
        /// <returns>List of <see cref="IInvoiceStatus"/></returns>
        IEnumerable<IInvoiceStatus> GetByIds(IEnumerable<int> ids);


        /// <summary>
        /// Returns a list of all <see cref="IInvoiceStatus"/> objects
        /// </summary>
        /// <returns></returns>
        IEnumerable<IInvoiceStatus> GetAll();
    }
}
