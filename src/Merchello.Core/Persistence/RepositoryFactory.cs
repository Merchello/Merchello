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

        public virtual ICustomerRepository CreateCustomerRepository(IDatabaseUnitOfWork uow)
        {
            return new CustomerRepository(uow, RuntimeCacheProvider.Current);
        }

        public virtual IAddressRepository CreateAddressRepository(IDatabaseUnitOfWork uow)
        {
            return new AddressRepository(uow, RuntimeCacheProvider.Current);
        }
    }
}
