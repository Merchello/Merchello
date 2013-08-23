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
    /// Defines the anonymous customer service
    /// </summary>
    interface IAnonymousCustomerService : IService
    {

        /// <summary>
        /// Creates an anonymous customer
        /// </summary>
        /// <returns></returns>
        IAnonymousCustomer CreateAnonymousCustomer(bool raiseEvents = true);

        /// <summary>
        /// Saves a single <see cref="IAnonymousCustomer"/> object
        /// </summary>
        /// <param name="anonymous">The <see cref="IAnonymousCustomer"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Save(IAnonymousCustomer anonymous, bool raiseEvents = true);


        /// <summary>
        /// Deletes a single <see cref="IAnonymousCustomer"/> object
        /// </summary>
        /// <param name="anonymous"><see cref="IAnonymousCustomer"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Delete(IAnonymousCustomer anonymous, bool raiseEvents = true);


        /// <summary>
        /// Deletes a collection of <see cref="IAnonymousCustomer"/> objects
        /// </summary>
        /// <param name="anonymousList">Collection of <see cref="IAnonymousCustomer"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Delete(IEnumerable<IAnonymousCustomer> anonymousList, bool raiseEvents = true);


        /// <summary>
        /// Gets an <see cref="IAnonymousCustomer"/> object by its 'UniqueId' (key)
        /// </summary>
        /// <param name="key">Guid pk of the AnonymousCustomer to retrieve</param>
        /// <returns><see cref="IAnonymousCustomer"/></returns>
        IAnonymousCustomer GetByKey(Guid key);


        /// <summary>
        /// Gets list of <see cref="IAnonymousCustomer"/> objects given a list of Unique keys
        /// </summary>
        /// <param name="keys">List of Guid keys for AnonymousCustomer objects to retrieve</param>
        /// <returns>List of <see cref="Guid"/> keys</returns>
        IEnumerable<IAnonymousCustomer> GetByKeys(IEnumerable<Guid> keys);


        ICustomer ConvertToCustomer(IAnonymousCustomer anonymous, string firstName, string lastName, string email, int? memberId = null, bool raiseEvents = true);

    }
}
