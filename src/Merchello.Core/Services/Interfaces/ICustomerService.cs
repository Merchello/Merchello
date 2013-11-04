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
        /// Crates an <see cref="IAnonymousCustomer"/> and saves it to the database
        /// </summary>
        /// <returns><see cref="IAnonymousCustomer"/></returns>
        IAnonymousCustomer CreateAnonymousCustomerWithKey();

        /// <summary>
        /// Creates a customer without saving to the database
        /// </summary>
        /// <param name="firstName">The first name of the customer</param>
        /// <param name="lastName">The last name of the customer</param>
        /// <param name="email">the email address of the customer</param>
        /// <param name="memberId">The Umbraco member Id of the customer</param>
        /// <returns><see cref="ICustomer"/></returns>
        ICustomer CreateCustomer(string firstName, string lastName, string email, int? memberId = null);

        /// <summary>
        /// Creates a customer and saves the record to the database
        /// </summary>
        /// <param name="firstName">The first name of the customer</param>
        /// <param name="lastName">The last name of the customer</param>
        /// <param name="email">the email address of the customer</param>
        /// <param name="memberId">The Umbraco member Id of the customer</param>
        /// <returns><see cref="ICustomer"/></returns>
        ICustomer CreateCustomerWithId(string firstName, string lastName, string email, int? memberId = null);

        /// <summary>
        /// Creates a customer with the Umbraco member id passed
        /// </summary>
        /// <param name="memberId">The Umbraco member id (int)</param>
        /// <returns><see cref="ICustomer"/></returns>
        ICustomer CreateCustomerWithId(int memberId);

        /// <summary>
        /// Saves a single <see cref="IAnonymousCustomer"/>
        /// </summary>
        /// <param name="anonymous">The <see cref="IAnonymousCustomer"/> to save</param>
        void Save(IAnonymousCustomer anonymous);

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
        /// Deletes a single <see cref="IAnonymousCustomer"/>
        /// </summary>
        /// <param name="anonymous">The <see cref="IAnonymousCustomer"/> to delete</param>
        void Delete(IAnonymousCustomer anonymous);

        /// <summary>
        /// Deletes a collection of <see cref="IAnonymousCustomer"/> objects
        /// </summary>
        /// <param name="anonymouses">Collection of <see cref="IAnonymousCustomer"/> to delete</param>
        void Delete(IEnumerable<IAnonymousCustomer> anonymouses);

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
        /// <param name="id">Integer id of the customer to retrieve</param>
        /// <returns><see cref="ICustomer"/></returns>
        ICustomer GetById(int id);

        /// <summary>
        /// Gets an <see cref="ICustomer"/> or <see cref="IAnonymousCustomer"/> object by its 'UniqueId'
        /// </summary>
        /// <param name="entityKey">Guid key of either object to retrieve</param>
        /// <returns><see cref="ICustomerBase"/></returns>
        ICustomerBase GetAnyByKey(Guid entityKey);

        /// <summary>
        /// Gets an <see cref="ICustomer"/> object by its Umbraco MemberId
        /// </summary>
        /// <param name="memberId">The Umbraco MemberId of the customer to return</param>
        /// <returns><see cref="ICustomer"/> object or null if not found</returns>
        ICustomer GetByMemberId(int? memberId);

        /// <summary>
        /// Gets list of <see cref="ICustomer"/> objects given a list of Unique keys
        /// </summary>
        /// <param name="ids">List of Guid pk for customers to retrieve</param>
        /// <returns>List of <see cref="ICustomer"/></returns>
        IEnumerable<ICustomer> GetByIds(IEnumerable<int> ids);

     
    }
}
