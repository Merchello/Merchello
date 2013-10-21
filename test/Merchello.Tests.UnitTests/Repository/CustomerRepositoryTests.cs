using System;
using Merchello.Core.Persistence.Repositories;
using Merchello.Tests.Base.DataMakers;
using Merchello.Tests.Base.Respositories.UnitOfWork;
using NUnit.Framework;

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
            //// Arrange
            var customer = MockCustomerDataMaker.CustomerForInserting();

            //// Act
            _repository.AddOrUpdate(customer);

            //// Assert
            Assert.IsTrue(_uow.InsertCalled);
        }

        [Test]
        public void Save_Calls_Update()
        {
            //// Arrange
            var customer = MockCustomerDataMaker.CustomerForInserting().MockSavedWithId(111);

            //// Act
            _repository.AddOrUpdate(customer);

            //// Assert
            Assert.IsTrue(_uow.UpdateCalled);
        }

        [Test]
        public void Delete_Calls_Delete()
        {
            //// Arrange
            var customer = MockCustomerDataMaker.CustomerForInserting().MockSavedWithId(111);

            //// Act
            _repository.Delete(customer);

            //// Assert
            Assert.IsTrue(_uow.DeleteCalled);
        }

    }
}
