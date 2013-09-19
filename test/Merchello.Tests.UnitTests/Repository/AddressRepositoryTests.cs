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
        private AddressRepository _repository;

        [SetUp]
        public void Setup()
        {
            _uow = new MockDatabaseUnitOfWork();
            _repository = new AddressRepository(_uow);
        }

        [Test]
        public void Save_Calls_Insert()
        {
            var address = MockAddressDataMaker.AddressForInserting();

            _repository.AddOrUpdate(address);

            Assert.IsTrue(_uow.InsertCalled);
        }

        [Test]
        public void Save_Calls_Update()
        {
            var address = MockAddressDataMaker.AddressForUpdating();
            _repository.AddOrUpdate(address);

            Assert.IsTrue(_uow.UpdateCalled);

        }

        [Test]
        public void Delete_Calls_Delete()
        {
            var address = MockAddressDataMaker.AddressForUpdating();

            _repository.Delete(address);

            Assert.IsTrue(_uow.DeleteCalled);
        }

    }
}
