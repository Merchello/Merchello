namespace Merchello.Core.Services
{
    using System;
    using System.Collections.Generic;
    using Models;
    using Umbraco.Core.Services;

    /// <summary>
    /// Defines a CustomerAddressService
    /// </summary>
    internal interface ICustomerAddressService : IService
    {
        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="address">
        /// The address.
        /// </param>
        void Save(ICustomerAddress address);

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="addresses">
        /// The addresses.
        /// </param>
        void Save(IEnumerable<ICustomerAddress> addresses);

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="address">
        /// The address.
        /// </param>
        void Delete(ICustomerAddress address);

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="addresses">
        /// The addresses.
        /// </param>
        void Delete(IEnumerable<ICustomerAddress> addresses);

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
        /// The get by customer key.
        /// </summary>
        /// <param name="customerKey">
        /// The customer key.
        /// </param>
        /// <returns>
        /// A collection of <see cref="ICustomerAddress"/>.
        /// </returns>
        IEnumerable<ICustomerAddress> GetByCustomerKey(Guid customerKey);

        /// <summary>
        /// The get by customer key.
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
    }
}