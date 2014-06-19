namespace Merchello.Core.Services
{
    using System;
    using System.Collections.Generic;
    using Models;
    using Models.TypeFields;
    using Umbraco.Core.Services;

    /// <summary>
    /// Defines the CustomerRegistryService, which provides access to operations involving <see cref="IItemCache"/>
    /// </summary>
    public interface IItemCacheService : IService
    {
        /// <summary>
        /// Creates an item cache (or retrieves an existing one) based on type and saves it to the database
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <param name="itemCache">
        /// The item Cache.
        /// </param>
        /// <returns>
        /// The <see cref="IItemCache"/>.
        /// </returns>
        IItemCache GetItemCacheWithKey(ICustomerBase customer, ItemCacheType itemCache);

        /// <summary>
        /// Creates an item cache (or retrieves an existing one) based on type and saves it to the database
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <param name="itemCacheType">
        /// The item Cache Type.
        /// </param>
        /// <param name="versionKey">
        /// The version Key.
        /// </param>
        /// <returns>
        /// The <see cref="IItemCache"/>.
        /// </returns>
        IItemCache GetItemCacheWithKey(ICustomerBase customer, ItemCacheType itemCacheType, Guid versionKey);

        /// <summary>
        /// Saves a single <see cref="IItemCache"/> object
        /// </summary>
        /// <param name="itemCache">The <see cref="IItemCache"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Save(IItemCache itemCache, bool raiseEvents = true);

        /// <summary>
        /// Saves a collection of <see cref="IItemCache"/> objects
        /// </summary>
        /// <param name="itemCaches">The <see cref="IItemCache"/>s to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Save(IEnumerable<IItemCache> itemCaches, bool raiseEvents = true);

        /// <summary>
        /// Deletes a single <see cref="IItemCache"/> object
        /// </summary>
        /// <param name="itemCache"><see cref="IItemCache"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Delete(IItemCache itemCache, bool raiseEvents = true);

        /// <summary>
        /// Deletes a collection of <see cref="IItemCache"/> objects
        /// </summary>
        /// <param name="itemCaches">Collection of <see cref="IItemCache"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Delete(IEnumerable<IItemCache> itemCaches, bool raiseEvents = true);

        /// <summary>
        /// Gets an <see cref="IItemCache"/> object by its Id
        /// </summary>
        /// <param name="key">unique 'key' of the Address to retrieve</param>
        /// <returns><see cref="IItemCache"/></returns>
        IItemCache GetByKey(Guid key);

        /// <summary>
        /// Gets a collection of <see cref="IItemCache"/> objects by the <see cref="ICustomerBase"/>
        /// </summary>
        /// <param name="customer">The customer associated with the <see cref="IItemCache"/></param>
        /// <returns>A collection of <see cref="IItemCache"/></returns>
        IEnumerable<IItemCache> GetItemCacheByCustomer(ICustomerBase customer);

        /// <summary>
        /// Returns the consumer's registry of a given type
        /// </summary>
        /// <param name="customer">The <see cref="ICustomerBase"/></param>
        /// <param name="itemCacheTfKey"><see cref="ITypeField"/>.TypeKey</param>
        /// <returns><see cref="IItemCache"/></returns>
        /// <remarks>
        /// Public use of this method is intended to access ItemCacheType.Custom records
        /// </remarks>
        IItemCache GetItemCacheByCustomer(ICustomerBase customer, Guid itemCacheTfKey);

        /// <summary>
        /// Gets an <see cref="IItemCache"/> object by the <see cref="ICustomerBase"/>
        /// </summary>
        /// <param name="customer">The <see cref="ICustomerBase"/> object</param>
        /// <param name="itemCacheType">The type of the <see cref="IItemCache"/></param>
        /// <returns><see cref="IItemCache"/></returns>
        IItemCache GetItemCacheByCustomer(ICustomerBase customer, ItemCacheType itemCacheType);
            
        /// <summary>
        /// Gets list of <see cref="IItemCache"/> objects given a list of Keys
        /// </summary>
        /// <param name="keys">List of guid 'key' for customer registries to retrieve</param>
        /// <returns>List of <see cref="IItemCache"/></returns>
        IEnumerable<IItemCache> GetByKeys(IEnumerable<Guid> keys);

    }
}
