using System;
using Merchello.Core.Persistence.Caching;
using Merchello.Core.Persistence.Repositories;
using Umbraco.Core.Persistence.UnitOfWork;

namespace Merchello.Core.Persistence
{
    /// <summary>
    /// Used to instantiate each repository type
    /// </summary>
    public class RepositoryFactory
    {

        /// <summary>
        /// Returns <see cref="ICustomerRepository"/>
        /// </summary>        
        public virtual ICustomerRepository CreateCustomerRepository(IDatabaseUnitOfWork uow)
        {
            return new CustomerRepository(uow, RuntimeCacheProvider.Current);
        }

        /// <summary>
        /// Returns <see cref="IAnonymousCustomerRepository"/>
        /// </summary>        
        internal virtual IAnonymousCustomerRepository CreateAnonymousCustomerRepository(IDatabaseUnitOfWork uow)
        {
            return new AnonymousCustomerRepository(uow, NullCacheProvider.Current);
        }

        /// <summary>
        /// Returns <see cref="IAddressRepository"/>
        /// </summary>        
        internal virtual IAddressRepository CreateAddressRepository(IDatabaseUnitOfWork uow)
        {
            return new AddressRepository(uow, NullCacheProvider.Current);
        }


        
    }
}
