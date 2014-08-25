namespace Merchello.Core.Services
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Models;

    using Umbraco.Core.Services;

    /// <summary>
    /// Defines the CustomerService, which provides access to operations involving <see cref="ICustomer"/>
    /// </summary>
    public interface ICustomerService : IPageCachedService<ICustomer>
    {
        /// <summary>
        /// Creates a customer without saving to the database
        /// </summary>
        /// <param name="loginName">
        /// The login Name.
        /// </param>
        /// <param name="firstName">
        /// The first name of the customer
        /// </param>
        /// <param name="lastName">
        /// The last name of the customer
        /// </param>
        /// <param name="email">
        /// the email address of the customer
        /// </param>
        /// <returns>
        /// The new <see cref="ICustomer"/>
        /// </returns>
        ICustomer CreateCustomer(string loginName, string firstName, string lastName, string email);

        /// <summary>
        /// Creates a customer and saves the record to the database
        /// </summary>
        /// <param name="loginName">
        /// The login Name.
        /// </param>
        /// <param name="firstName">
        /// The first name of the customer
        /// </param>
        /// <param name="lastName">
        /// The last name of the customer
        /// </param>
        /// <param name="email">
        /// the email address of the customer
        /// </param>
        /// <returns>
        /// <see cref="ICustomer"/>
        /// </returns>
        ICustomer CreateCustomerWithKey(string loginName, string firstName, string lastName, string email);

        /// <summary>
        /// Creates a customer with the Umbraco member id passed
        /// </summary>
        /// <param name="loginName">
        /// The login Name.
        /// </param>
        /// <returns>
        /// <see cref="ICustomer"/>
        /// </returns>
        ICustomer CreateCustomerWithKey(string loginName);

        /// <summary>
        /// Saves a single <see cref="ICustomer"/> object
        /// </summary>
        /// <param name="customer">The <see cref="ICustomer"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Save(ICustomer customer, bool raiseEvents = true);

        /// <summary>
        /// Saves a collection of <see cref="ICustomer"/> objects
        /// </summary>
        /// <param name="customers">The collection of customers to be saved</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
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
        /// Gets an <see cref="ICustomer"/> or <see cref="IAnonymousCustomer"/> object by its 'UniqueId'
        /// </summary>
        /// <param name="entityKey">GUID key of either object to retrieve</param>
        /// <returns><see cref="ICustomerBase"/></returns>
        ICustomerBase GetAnyByKey(Guid entityKey);

        /// <summary>
        /// Gets an <see cref="ICustomer"/> object by its Umbraco login name
        /// </summary>
        /// <param name="loginName">
        /// The login Name.
        /// </param>
        /// <returns>
        /// <see cref="ICustomer"/> object or null if not found
        /// </returns>
        ICustomer GetByLoginName(string loginName);

        /// <summary>
        /// Gets list of <see cref="ICustomer"/> objects given a list of Unique keys
        /// </summary>
        /// <param name="keys">List of GUID keys for customers to retrieve</param>
        /// <returns>List of <see cref="ICustomer"/></returns>
        IEnumerable<ICustomer> GetByKeys(IEnumerable<Guid> keys);

        /// <summary>
        /// Gets the total customer count.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        int CustomerCount();

        #region Anonymous Customer

        /// <summary>
        /// Creates an <see cref="IAnonymousCustomer"/> and saves it to the database
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

        #endregion

        #region Customer Address

        /// <summary>
        /// Saves a single <see cref="ICustomerAddress"/>
        /// </summary>
        /// <param name="address">
        /// The address.
        /// </param>
        void Save(ICustomerAddress address);


        /// <summary>
        /// Deletes a single instance of the <see cref="ICustomerAddress"/>
        /// </summary>
        /// <param name="address">
        /// The address to be deleted
        /// </param>
        void Delete(ICustomerAddress address);


        /// <summary>
        /// The get by key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="ICustomerAddress"/>.
        /// </returns>
        ICustomerAddress GetAddressByKey(Guid key);

        /// <summary>
        /// Gets a collection of <see cref="ICustomerAddress"/> by the customer key
        /// </summary>
        /// <param name="customerKey">
        /// The customer key.
        /// </param>
        /// <returns>
        /// A collection of <see cref="ICustomerAddress"/>.
        /// </returns>
        IEnumerable<ICustomerAddress> GetByCustomerKey(Guid customerKey);

        /// <summary>
        /// Gets a collection of <see cref="ICustomerAddress"/> by the customer key filtered by an <see cref="AddressType"/>
        /// </summary>
        /// <param name="customerKey">
        /// The customer key.
        /// </param>
        /// <param name="addressType">
        /// The address type.
        /// </param>
        /// <returns>
        /// A collection of <see cref="ICustomerAddress"/>.
        /// </returns>
        IEnumerable<ICustomerAddress> GetByCustomerKey(Guid customerKey, AddressType addressType);

        /// <summary>
        /// Gets the default customer address of a certain type
        /// </summary>
        /// <param name="customerKey">
        /// The customer key.
        /// </param>
        /// <param name="addressType">
        /// The address type.
        /// </param>
        /// <returns>
        /// The <see cref="ICustomerAddress"/>.
        /// </returns>
        ICustomerAddress GetDefaultCustomerAddress(Guid customerKey, AddressType addressType);

        #endregion
    }
}
