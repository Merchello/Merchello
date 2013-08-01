using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Models;
using Merchello.Core.Persistence;
using Umbraco.Core.Persistence.Querying;
using Umbraco.Core.Persistence.UnitOfWork;

namespace Merchello.Core.Services
{
    /// <summary>
    /// Represents the Customer Service, 
    /// </summary>
    public class CustomerService : ICustomerService
    {
        private readonly IDatabaseUnitOfWorkProvider _uowProvider;
        private readonly RepositoryFactory _repositoryFactory;

        public CustomerService()
            : this(new RepositoryFactory())
        { }

        public CustomerService(RepositoryFactory repositoryFactory)
            : this(new PetaPocoUnitOfWorkProvider(), repositoryFactory)
        { }

        public CustomerService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory)
        {
            if (provider == null) throw new ArgumentNullException("provider");
            if (repositoryFactory == null) throw new ArgumentNullException("repositoryFactory");

            _uowProvider = provider;
            _repositoryFactory = repositoryFactory;
        }

        public ICustomer GetById(Guid key)
        {
            using (var repository = _repositoryFactory.CreateCustomerRepository(_uowProvider.GetUnitOfWork()))
            {
                var query = Query<ICustomer>.Builder.Where(x => x.Key == key);
                var customers = repository.GetByQuery(query);
                return customers.FirstOrDefault();
            }
        }
    }
}
