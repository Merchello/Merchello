using System;
using System.Collections.Generic;
using Merchello.Core.Models;
using Merchello.Core.Models.TypeFields;
using Umbraco.Core.Services;

namespace Merchello.Core.Services
{
    /// <summary>
    /// Defines the CustomerRegistryService, which provides access to operations involving <see cref="ICustomerRegistry"/>
    /// </summary>
    public interface ICustomerRegistryService : IService
    {
        /// <summary>
        /// Creates a Basket
        /// </summary>
        ICustomerRegistry CreateCustomerRegistry(IConsumer consumer, CustomerRegistryType customerRegistryType);

        /// <summary>
        /// Saves a single <see cref="ICustomerRegistry"/> object
        /// </summary>
        /// <param name="customerRegistry">The <see cref="ICustomerRegistry"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Save(ICustomerRegistry customerRegistry, bool raiseEvents = true);

        /// <summary>
        /// Saves a collection of <see cref="ICustomerRegistry"/> objects
        /// </summary>
        /// <param name="customerRegistries"></param>
        /// <param name="raiseEvents"></param>
        void Save(IEnumerable<ICustomerRegistry> customerRegistries, bool raiseEvents = true);

        /// <summary>
        /// Deletes a single <see cref="ICustomerRegistry"/> object
        /// </summary>
        /// <param name="customerRegistry"><see cref="ICustomerRegistry"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Delete(ICustomerRegistry customerRegistry, bool raiseEvents = true);

        /// <summary>
        /// Deletes a collection of <see cref="IAddress"/> objects
        /// </summary>
        /// <param name="customerRegistries">Collection of <see cref="ICustomerRegistry"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Delete(IEnumerable<ICustomerRegistry> customerRegistries, bool raiseEvents = true);

        /// <summary>
        /// Gets an <see cref="ICustomerRegistry"/> object by its Id
        /// </summary>
        /// <param name="id">int Id of the Address to retrieve</param>
        /// <returns><see cref="ICustomerRegistry"/></returns>
        ICustomerRegistry GetById(int id);

        /// <summary>
        /// Gets a collection of <see cref="ICustomerRegistry"/> objects by the <see cref="IConsumer"/>
        /// </summary>
        /// <param name="consumer"></param>
        /// <returns></returns>
        IEnumerable<ICustomerRegistry> GetBasketsByConsumer(IConsumer consumer);

        /// <summary>
        /// Returns the consumer's registry of a given type
        /// </summary>
        /// <param name="consumer"><see cref="IConsumer"/></param>
        /// <param name="customerRegistryBasketTfKey"><see cref="ITypeField"/>.TypeKey</param>
        /// <returns><see cref="ICustomerRegistry"/></returns>
        /// <remarks>
        /// Public use of this method is intended to access CustomerRegistryType.Custom records
        /// </remarks>
        ICustomerRegistry GetBasketByConsumer(IConsumer consumer, Guid customerRegistryBasketTfKey);

        /// <summary>
        /// Gets an <see cref="ICustomerRegistry"/> object by the <see cref="IConsumer"/>
        /// </summary>
        /// <param name="consumer">The <see cref="IConsumer"/> object</param>
        /// <param name="customerRegistryType"></param>
        /// <returns><see cref="ICustomerRegistry"/></returns>
        ICustomerRegistry GetBasketByConsumer(IConsumer consumer, CustomerRegistryType customerRegistryType);
            
        /// <summary>
        /// Gets list of <see cref="ICustomerRegistry"/> objects given a list of Ids
        /// </summary>
        /// <param name="ids">List of int Id for customer registries to retrieve</param>
        /// <returns>List of <see cref="ICustomerRegistry"/></returns>
        IEnumerable<ICustomerRegistry> GetByIds(IEnumerable<int> ids);

    }
}
