namespace Merchello.Core.Services
{
    using System;
    using System.Collections.Generic;
    using Models;
    using Umbraco.Core.Services;

    /// <summary>
    /// The AnonymousCustomerService interface.
    /// </summary>
    public interface IAnonymousCustomerService : IService
    {
        /// <summary>
        /// Crates an <see cref="IAnonymousCustomer"/> and saves it to the database
        /// </summary>
        /// <returns><see cref="IAnonymousCustomer"/></returns>
        IAnonymousCustomer CreateAnonymousCustomerWithKey();

        /// <summary>
        /// Saves a single <see cref="IAnonymousCustomer"/>
        /// </summary>
        /// <param name="anonymous">The <see cref="IAnonymousCustomer"/> to save</param>
        void Save(IAnonymousCustomer anonymous);


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
        /// The get anonymous customers created before a certain date.
        /// </summary>
        /// <param name="createdDate">
        /// The created Date.
        /// </param>
        /// <returns>
        /// The collection of <see cref="IAnonymousCustomer"/> older than a certain number of days.
        /// </returns>
        IEnumerable<IAnonymousCustomer> GetAnonymousCustomersCreatedBefore(DateTime createdDate);
    }
}