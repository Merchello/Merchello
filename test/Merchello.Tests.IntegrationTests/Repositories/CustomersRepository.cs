using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Persistence.Repositories;
using NUnit.Framework;
using Umbraco.Core.Persistence.Repositories;
using Umbraco.Core.Persistence.UnitOfWork;

namespace Merchello.Tests.IntegrationTests.Repositories
{
    [TestFixture]
    [Category("Customers")]
    public class CustomersRepository
    {

        private IDatabaseUnitOfWorkProvider _uowProvider;
        private ICustomerRepository _repository;

        [SetUp]
        public void Setup()
        {
            _uowProvider = new PetaPocoUnitOfWorkProvider();
            _repository = new CustomerRepository(_uowProvider.GetUnitOfWork());
        }


        public void Can_Create_3_Customers()
        {
            
        }
    }
}
