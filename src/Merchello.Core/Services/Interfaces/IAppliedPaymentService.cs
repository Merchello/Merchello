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
    /// Defines the AddressService, which provides access to operations involving <see cref="IAppliedPayment"/>
    /// </summary>
    public interface IAppliedPaymentService : IService
    {

        /// <summary>
        /// Creates a Transaction
        /// </summary>
        IAppliedPayment CreateAppliedPayment(IPayment payment, IInvoice invoice, AppliedPaymentType appliedPaymentType, decimal amount);

        /// <summary>
        /// Saves a single <see cref="IAppliedPayment"/> object
        /// </summary>
        /// <param name="appliedPayment">The <see cref="IAppliedPayment"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Save(IAppliedPayment appliedPayment, bool raiseEvents = true);

        /// <summary>
        /// Saves a collection of <see cref="IAppliedPayment"/> objects
        /// </summary>
        /// <param name="appliedPaymentList">Collection of <see cref="IAppliedPayment"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Save(IEnumerable<IAppliedPayment> appliedPaymentList, bool raiseEvents = true);

        /// <summary>
        /// Deletes a single <see cref="IAppliedPayment"/> object
        /// </summary>
        /// <param name="appliedPayment"><see cref="IAppliedPayment"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Delete(IAppliedPayment appliedPayment, bool raiseEvents = true);

        /// <summary>
        /// Deletes a collection of <see cref="IAppliedPayment"/> objects
        /// </summary>
        /// <param name="appliedPaymentList">Collection of <see cref="IAppliedPayment"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Delete(IEnumerable<IAppliedPayment> appliedPaymentList, bool raiseEvents = true);

        /// <summary>
        /// Gets an <see cref="IAppliedPayment"/> object by its 'UniqueId'
        /// </summary>
        /// <param name="id">int Id of the Transaction to retrieve</param>
        /// <returns><see cref="IAppliedPayment"/></returns>
        IAppliedPayment GetById(int id);

        /// <summary>
        /// Gets list of <see cref="IAppliedPayment"/> objects given a list of Unique keys
        /// </summary>
        /// <param name="ids">List of int Id for Transaction objects to retrieve</param>
        /// <returns>List of <see cref="IAppliedPayment"/></returns>
        IEnumerable<IAppliedPayment> GetByIds(IEnumerable<int> ids);

    }
}
