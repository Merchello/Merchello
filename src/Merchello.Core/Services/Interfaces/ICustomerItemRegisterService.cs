using System;
using System.Collections.Generic;
using Merchello.Core.Models;
using Merchello.Core.Models.TypeFields;
using Umbraco.Core.Services;

namespace Merchello.Core.Services
{
    /// <summary>
    /// Defines the CustomerRegistryService, which provides access to operations involving <see cref="ICustomerItemCache"/>
    /// </summary>
    public interface ICustomerItemRegisterService : IService
    {
        /// <summary>
        /// Creates a Basket
        /// </summary>
        ICustomerItemCache CreateCustomerItemRegister(IConsumer consumer, CustomerItemCacheType customerItemCacheType);

        /// <summary>
        /// Saves a single <see cref="ICustomerItemCache"/> object
        /// </summary>
        /// <param name="customerItemCache">The <see cref="ICustomerItemCache"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Save(ICustomerItemCache customerItemCache, bool raiseEvents = true);

        /// <summary>
        /// Saves a collection of <see cref="ICustomerItemCache"/> objects
        /// </summary>
        /// <param name="customerRegistries"></param>
        /// <param name="raiseEvents"></param>
        void Save(IEnumerable<ICustomerItemCache> customerRegistries, bool raiseEvents = true);

        /// <summary>
        /// Deletes a single <see cref="ICustomerItemCache"/> object
        /// </summary>
        /// <param name="customerItemCache"><see cref="ICustomerItemCache"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Delete(ICustomerItemCache customerItemCache, bool raiseEvents = true);

        /// <summary>
        /// Deletes a collection of <see cref="IAddress"/> objects
        /// </summary>
        /// <param name="customerRegistries">Collection of <see cref="ICustomerItemCache"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Delete(IEnumerable<ICustomerItemCache> customerRegistries, bool raiseEvents = true);

        /// <summary>
        /// Gets an <see cref="ICustomerItemCache"/> object by its Id
        /// </summary>
        /// <param name="id">int Id of the Address to retrieve</param>
        /// <returns><see cref="ICustomerItemCache"/></returns>
        ICustomerItemCache GetById(int id);

        /// <summary>
        /// Gets a collection of <see cref="ICustomerItemCache"/> objects by the <see cref="IConsumer"/>
        /// </summary>
        /// <param name="consumer"></param>
        /// <returns></returns>
        IEnumerable<ICustomerItemCache> GetRegisterByConsumer(IConsumer consumer);

        /// <summary>
        /// Returns the consumer's registry of a given type
        /// </summary>
        /// <param name="consumer"><see cref="IConsumer"/></param>
        /// <param name="registerTfKey"><see cref="ITypeField"/>.TypeKey</param>
        /// <returns><see cref="ICustomerItemCache"/></returns>
        /// <remarks>
        /// Public use of this method is intended to access CustomerRegistryType.Custom records
        /// </remarks>
        ICustomerItemCache GetRegisterByConsumer(IConsumer consumer, Guid registerTfKey);

        /// <summary>
        /// Gets an <see cref="ICustomerItemCache"/> object by the <see cref="IConsumer"/>
        /// </summary>
        /// <param name="consumer">The <see cref="IConsumer"/> object</param>
        /// <param name="customerItemCacheType"></param>
        /// <returns><see cref="ICustomerItemCache"/></returns>
        ICustomerItemCache GetRegisterByConsumer(IConsumer consumer, CustomerItemCacheType customerItemCacheType);
            
        /// <summary>
        /// Gets list of <see cref="ICustomerItemCache"/> objects given a list of Ids
        /// </summary>
        /// <param name="ids">List of int Id for customer registries to retrieve</param>
        /// <returns>List of <see cref="ICustomerItemCache"/></returns>
        IEnumerable<ICustomerItemCache> GetByIds(IEnumerable<int> ids);

    }
}
