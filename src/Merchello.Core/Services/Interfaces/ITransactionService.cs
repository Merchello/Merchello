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
    /// Defines the AddressService, which provides access to operations involving <see cref="ITransaction"/>
    /// </summary>
    public interface ITransactionService : IService
    {

        /// <summary>
        /// Creates a Transaction
        /// </summary>
        ITransaction CreateTransaction(IPayment payment, IInvoice invoice, TransactionType transactionType, decimal amount);

        /// <summary>
        /// Saves a single <see cref="ITransaction"/> object
        /// </summary>
        /// <param name="transaction">The <see cref="ITransaction"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Save(ITransaction transaction, bool raiseEvents = true);

        /// <summary>
        /// Saves a collection of <see cref="ITransaction"/> objects
        /// </summary>
        /// <param name="transactionList">Collection of <see cref="ITransaction"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Save(IEnumerable<ITransaction> transactionList, bool raiseEvents = true);

        /// <summary>
        /// Deletes a single <see cref="ITransaction"/> object
        /// </summary>
        /// <param name="transaction"><see cref="ITransaction"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Delete(ITransaction transaction, bool raiseEvents = true);

        /// <summary>
        /// Deletes a collection of <see cref="ITransaction"/> objects
        /// </summary>
        /// <param name="transactionList">Collection of <see cref="ITransaction"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Delete(IEnumerable<ITransaction> transactionList, bool raiseEvents = true);

        /// <summary>
        /// Gets an <see cref="ITransaction"/> object by its 'UniqueId'
        /// </summary>
        /// <param name="id">int Id of the Transaction to retrieve</param>
        /// <returns><see cref="ITransaction"/></returns>
        ITransaction GetById(int id);

        /// <summary>
        /// Gets list of <see cref="ITransaction"/> objects given a list of Unique keys
        /// </summary>
        /// <param name="ids">List of int Id for Transaction objects to retrieve</param>
        /// <returns>List of <see cref="ITransaction"/></returns>
        IEnumerable<ITransaction> GetByIds(IEnumerable<int> ids);

    }
}
