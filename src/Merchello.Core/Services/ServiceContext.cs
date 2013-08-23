using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Persistence;
using Umbraco.Core.Persistence.UnitOfWork;

namespace Merchello.Core.Services
{
    /// <summary>
    /// The Merchello ServiceContext, which provides access to the following services:
    /// <see cref="ICustomerService"/>
    /// </summary>
    public class ServiceContext
    {
        private Lazy<CustomerService> _customerService;
        private Lazy<AnonymousCustomerService> _anonymousCustomerService; 
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dbUnitOfWorkProvider"></param>
        internal ServiceContext(IDatabaseUnitOfWorkProvider dbUnitOfWorkProvider)
        {
            BuildServiceContext(dbUnitOfWorkProvider, new Lazy<RepositoryFactory>(() => new RepositoryFactory()));
        }

        /// <summary>
        /// Builds the various services
        /// </summary>
        /// <param name="dbDatabaseUnitOfWorkProvider">Database unit of work provider used by the various services</param>
        /// <param name="repositoryFactory"><see cref="RepositoryFactory"/></param>
        private void BuildServiceContext(IDatabaseUnitOfWorkProvider dbDatabaseUnitOfWorkProvider,
            Lazy<RepositoryFactory> repositoryFactory)
        {
            if(_customerService == null)
                _customerService = new Lazy<CustomerService>(() => new CustomerService(dbDatabaseUnitOfWorkProvider, repositoryFactory.Value));

            if (_anonymousCustomerService == null)
                _anonymousCustomerService = new Lazy<AnonymousCustomerService>(() => new AnonymousCustomerService(dbDatabaseUnitOfWorkProvider, repositoryFactory.Value, _customerService.Value));

        }


        /// <summary>
        /// Gets the <see cref="ICustomerService"/>
        /// </summary>
        public ICustomerService CustomerService
        {
            get { return _customerService.Value;  }
        }
    }
}
