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
    /// Defines the CustomerService, which provides access to operations involving <see cref="ICustomer"/>
    /// </summary>
    public interface ICustomerService : IService
    {
        /// <summary>
        /// Creates a customer
        /// </summary>
        /// <param name="firstName">The first name of the customer</param>
        /// <param name="lastName">The last name of the customer</param>
        /// <param name="email">the email address of the customer</param>
        /// <param name="memberId">The Umbraco member Id of the customer</param>
        /// <returns><see cref="ICustomer"/></returns>
        ICustomer CreateCustomer(string firstName, string lastName, string email, int? memberId = null);        

        /// <summary>
        /// Saves a single <see cref="ICustomer"/> object
        /// </summary>
        /// <param name="customer">The <see cref="ICustomer"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Save(ICustomer customer, bool raiseEvents = true);

        /// <summary>
        /// Saves a collection of <see cref="ICustomer"/> objects
        /// </summary>
        /// <param name="customers"></param>
        /// <param name="raiseEvents"></param>
        void Save(IEnumerable<ICustomer> customers, bool raiseEvents = true);

        /// <summary>
        /// Deletes a single <see cref="ICustomer"/> object
        /// </summary>
        /// <param name="customer"><see cref="ICustomer"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Delete(ICustomer customer, bool raiseEvents = true);

        /// <summary>
        /// Deletes a collection of <see cref="ICustomer"/> objects
        /// </summary>
        /// <param name="customers">Collection of <see cref="ICustomer"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Delete(IEnumerable<ICustomer> customers, bool raiseEvents = true);

        /// <summary>
        /// Gets an <see cref="ICustomer"/> object by its 'UniqueId'
        /// </summary>
        /// <param name="key">Guid key of the Customer to retrieve</param>
        /// <returns><see cref="ICustomer"/></returns>
        ICustomer GetByKey(Guid key);

        /// <summary>
        /// Gets list of <see cref="ICustomer"/> objects given a list of Unique keys
        /// </summary>
        /// <param name="keys">List of Guid pk for customers to retrieve</param>
        /// <returns>List of <see cref="ICustomer"/></returns>
        IEnumerable<ICustomer> GetByKeys(IEnumerable<Guid> keys);
     
    }
}
