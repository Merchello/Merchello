using Merchello.Core.Persistence.Repositories;
using Merchello.Tests.Base.DataMakers;
using Merchello.Tests.Base.Respositories.UnitOfWork;
using NUnit.Framework;

namespace Merchello.Tests.UnitTests.Repository
{
    [TestFixture]
    [Category("Persistence")]
    public class AddressRepositoryTests
    {
        private MockDatabaseUnitOfWork _uow;
        private CustomerAddressRepository _repository;

        [SetUp]
        public void Setup()
        {
            _uow = new MockDatabaseUnitOfWork();
            _repository = new CustomerAddressRepository(_uow);
        }

        [Test]
        public void Save_Calls_Insert()
        {
            //// Arrange
            var address = MockAddressDataMaker.AddressForInserting();

            //// Act
            _repository.AddOrUpdate(address);

            //// Assert
            Assert.IsTrue(_uow.InsertCalled);
        }

        [Test]
        public void Save_Calls_Update()
        {
            //// Arrange
            var id = 111;
            var address = MockAddressDataMaker.AddressForInserting().MockSavedWithId(id);

            //// Act
            _repository.AddOrUpdate(address);

            //// Assert
            Assert.IsTrue(_uow.UpdateCalled);
        }

        [Test]
        public void Delete_Calls_Delete()
        {
            //// Arrange
            var id = 111;
            var address = MockAddressDataMaker.AddressForInserting().MockSavedWithId(id);

            //// Act
            _repository.Delete(address);

            //// Assert
            Assert.IsTrue(_uow.DeleteCalled);
        }

    }
}
