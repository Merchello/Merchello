namespace Merchello.Core.Services
{
    using System;
    using System.Collections.Generic;
    using Models;
    using Umbraco.Core.Services;

    /// <summary>
    /// Defines a CustomerAddressService
    /// </summary>
    public interface ICustomerAddressService : IService
    {
        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="address">
        /// The address.
        /// </param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Save(ICustomerAddress address, bool raiseEvents = true);

        /////// <summary>
        /////// The save.
        /////// </summary>
        /////// <param name="addresses">
        /////// The addresses.
        /////// </param>
        /////// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        ////void Save(IEnumerable<ICustomerAddress> addresses, bool raiseEvents = true);

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="address">
        /// The address.
        /// </param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Delete(ICustomerAddress address, bool raiseEvents = true);

        /////// <summary>
        /////// The delete.
        /////// </summary>
        /////// <param name="addresses">
        /////// The addresses.
        /////// </param>
        /////// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        ////void Delete(IEnumerable<ICustomerAddress> addresses, bool raiseEvents = true);

        /// <summary>
        /// The get by key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="ICustomerAddress"/>.
        /// </returns>
        ICustomerAddress GetByKey(Guid key);

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
        /// Gets a colleciton of all customer addresses
        /// </summary>
        /// <returns>
        /// The colleciton of all customer addresses.
        /// </returns>
        IEnumerable<ICustomerAddress> GetAll();
    }
}