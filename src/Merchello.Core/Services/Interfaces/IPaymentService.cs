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
    /// Defines the AddressService, which provides access to operations involving <see cref="IPayment"/>
    /// </summary>
    public interface IPaymentService : IService
    {

        /// <summary>
        /// Creates a Payment
        /// </summary>
        IPayment CreatePayment(ICustomer customer, Guid providerKey, PaymentMethodType paymentMethodType, string paymentMethodName, string referenceNumber, decimal amount);

        /// <summary>
        /// Saves a single <see cref="IPayment"/> object
        /// </summary>
        /// <param name="payment">The <see cref="IPayment"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Save(IPayment payment, bool raiseEvents = true);

        /// <summary>
        /// Saves a collection of <see cref="IPayment"/> objects
        /// </summary>
        /// <param name="paymentList">Collection of <see cref="IPayment"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Save(IEnumerable<IPayment> paymentList, bool raiseEvents = true);

        /// <summary>
        /// Deletes a single <see cref="IPayment"/> object
        /// </summary>
        /// <param name="payment"><see cref="IPayment"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Delete(IPayment payment, bool raiseEvents = true);

        /// <summary>
        /// Deletes a collection of <see cref="IPayment"/> objects
        /// </summary>
        /// <param name="paymentList">Collection of <see cref="IPayment"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Delete(IEnumerable<IPayment> paymentList, bool raiseEvents = true);

        /// <summary>
        /// Gets an <see cref="IPayment"/> object by its 'UniqueId'
        /// </summary>
        /// <param name="id">int Id of the Payment to retrieve</param>
        /// <returns><see cref="IPayment"/></returns>
        IPayment GetById(int id);

        /// <summary>
        /// Gets a collection of <see cref="IPayment"/> object given a customers key
        /// </summary>
        /// <param name="customerKey">The unique <see cref="Guid"/> of the customer record</param>
        /// <returns>A collection of <see cref="IPayment"/> objects</returns>
        IEnumerable<IPayment> GetPaymentsByCustomer(Guid customerKey);
        
            
        /// <summary>
        /// Gets list of <see cref="IPayment"/> objects given a list of Unique keys
        /// </summary>
        /// <param name="ids">List of int Id for Payment objects to retrieve</param>
        /// <returns>List of <see cref="IPayment"/></returns>
        IEnumerable<IPayment> GetByIds(IEnumerable<int> ids);

    }
}
