using System;
using System.Collections.Generic;
using Merchello.Core.Models;
using Merchello.Core.Models.TypeFields;
using Umbraco.Core.Services;

namespace Merchello.Core.Services
{
    /// <summary>
    /// Defines the CustomerRegistryService, which provides access to operations involving <see cref="ICustomerItemRegister"/>
    /// </summary>
    public interface ICustomerItemRegisterService : IService
    {
        /// <summary>
        /// Creates a Basket
        /// </summary>
        ICustomerItemRegister CreateCustomerItemRegister(IConsumer consumer, CustomerItemRegisterType customerItemRegisterType);

        /// <summary>
        /// Saves a single <see cref="ICustomerItemRegister"/> object
        /// </summary>
        /// <param name="customerItemRegister">The <see cref="ICustomerItemRegister"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Save(ICustomerItemRegister customerItemRegister, bool raiseEvents = true);

        /// <summary>
        /// Saves a collection of <see cref="ICustomerItemRegister"/> objects
        /// </summary>
        /// <param name="customerRegistries"></param>
        /// <param name="raiseEvents"></param>
        void Save(IEnumerable<ICustomerItemRegister> customerRegistries, bool raiseEvents = true);

        /// <summary>
        /// Deletes a single <see cref="ICustomerItemRegister"/> object
        /// </summary>
        /// <param name="customerItemRegister"><see cref="ICustomerItemRegister"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Delete(ICustomerItemRegister customerItemRegister, bool raiseEvents = true);

        /// <summary>
        /// Deletes a collection of <see cref="IAddress"/> objects
        /// </summary>
        /// <param name="customerRegistries">Collection of <see cref="ICustomerItemRegister"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Delete(IEnumerable<ICustomerItemRegister> customerRegistries, bool raiseEvents = true);

        /// <summary>
        /// Gets an <see cref="ICustomerItemRegister"/> object by its Id
        /// </summary>
        /// <param name="id">int Id of the Address to retrieve</param>
        /// <returns><see cref="ICustomerItemRegister"/></returns>
        ICustomerItemRegister GetById(int id);

        /// <summary>
        /// Gets a collection of <see cref="ICustomerItemRegister"/> objects by the <see cref="IConsumer"/>
        /// </summary>
        /// <param name="consumer"></param>
        /// <returns></returns>
        IEnumerable<ICustomerItemRegister> GetRegisterByConsumer(IConsumer consumer);

        /// <summary>
        /// Returns the consumer's registry of a given type
        /// </summary>
        /// <param name="consumer"><see cref="IConsumer"/></param>
        /// <param name="registerTfKey"><see cref="ITypeField"/>.TypeKey</param>
        /// <returns><see cref="ICustomerItemRegister"/></returns>
        /// <remarks>
        /// Public use of this method is intended to access CustomerRegistryType.Custom records
        /// </remarks>
        ICustomerItemRegister GetRegisterByConsumer(IConsumer consumer, Guid registerTfKey);

        /// <summary>
        /// Gets an <see cref="ICustomerItemRegister"/> object by the <see cref="IConsumer"/>
        /// </summary>
        /// <param name="consumer">The <see cref="IConsumer"/> object</param>
        /// <param name="customerItemRegisterType"></param>
        /// <returns><see cref="ICustomerItemRegister"/></returns>
        ICustomerItemRegister GetRegisterByConsumer(IConsumer consumer, CustomerItemRegisterType customerItemRegisterType);
            
        /// <summary>
        /// Gets list of <see cref="ICustomerItemRegister"/> objects given a list of Ids
        /// </summary>
        /// <param name="ids">List of int Id for customer registries to retrieve</param>
        /// <returns>List of <see cref="ICustomerItemRegister"/></returns>
        IEnumerable<ICustomerItemRegister> GetByIds(IEnumerable<int> ids);

    }
}
