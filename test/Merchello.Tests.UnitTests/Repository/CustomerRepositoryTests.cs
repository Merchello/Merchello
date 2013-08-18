using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Models.EntityBase;
using Merchello.Core.Persistence.Repositories;
using Merchello.Tests.Base.Data;
using Merchello.Tests.Base.Respositories.UnitOfWork;
using NUnit.Framework;
using Umbraco.Core.Persistence.UnitOfWork;

namespace Merchello.Tests.UnitTests.Repository
{
    [TestFixture]
    [Category("Persistence")]
    public class CustomerRepositoryTests
    {
        private MockDatabaseUnitOfWork _uow;
        private CustomerRepository _repository;

        [SetUp]
        public void Setup()
        {
            _uow = new MockDatabaseUnitOfWork();
            _repository = new CustomerRepository(_uow);
        }

        [Test]
        public void Save_Calls_Insert()
        {
            var customer = CustomerData.CustomerForInserting();

            _repository.AddOrUpdate(customer);

            Assert.IsTrue(_uow.InsertCalled);
        }

        [Test]
        public void Save_Calls_Update()
        {
            var customer = CustomerData.CustomerForUpdating();
            _repository.AddOrUpdate(customer);

            Assert.IsTrue(_uow.UpdateCalled);

        }

        [Test]
        public void Delete_Calls_Delete()
        {
            var customer = CustomerData.CustomerForUpdating();

            _repository.Delete(customer);

            Assert.IsTrue(_uow.DeleteCalled);
        }

    }
}
