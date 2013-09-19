using System.Linq;
using System.Runtime.Remoting;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Tests.Base.DataMakers;
using NUnit.Framework;

namespace Merchello.Tests.IntegrationTests.Services
{
    [TestFixture]
    [Category("Service Integration")]
    public class AddressServiceTests : ServiceIntegrationTestBase
    {
        private ICustomer _customer;
        private IAddressService _addressService;

        [SetUp]
        public void Setup()
        {
            _addressService = PreTestDataWorker.AddressService;
            _customer = PreTestDataWorker.MakeExistingCustomer();
        }

        /// <summary>
        /// Verifies that an address can be saved
        /// </summary>
        [Test]
        public void Can_Save_An_Address()
        {
            //// Arrange            
            var address = MockAddressDataMaker.RandomAddress(_customer, "Studio");
            var hasIdentity = address.HasIdentity;

            Assert.IsFalse(hasIdentity);

            //// Act
            _addressService.Save(address);

            //// Assert
            Assert.IsTrue(address.HasIdentity);
        }

        /// <summary>
        /// Test verifies that an address can be retrieved by it's id
        /// </summary>
        [Test]
        public void Can_Retrieve_An_Address_By_Id()
        {
            //// Arrange
            var expected = PreTestDataWorker.MakeExistingAddress(_customer, "Home");
            var id = expected.Id;

            //// Act
            var retrieved = _addressService.GetById(id);

            //// Assume
            Assert.NotNull(retrieved);
            Assert.AreEqual(expected.Id, retrieved.Id);
        }

        /// <summary>
        /// Test verifies that an collection of addresses can be retrieved for a customer
        /// </summary>
        [Test]
        public void Can_Retrieve_A_List_Of_Addresses_For_A_Customer()
        {
            //// Arrange
            PreTestDataWorker.DeleteAllAddresses();
            var expected = 10;
            var generated = PreTestDataWorker.MakeExistingAddressCollection(_customer, "Home", expected);

            //// Act
            var retrieved = _addressService.GetAddressesForCustomer(_customer.Key);

            //// Assert
            Assert.IsTrue(retrieved.Any());
            Assert.AreEqual(expected, retrieved.Count());
        }

        /// <summary>
        /// Verifies that an address can be deleted
        /// </summary>
        [Test]
        public void Can_Delete_An_Address()
        {
            //// Arrange
            var address = PreTestDataWorker.MakeExistingAddress(_customer, "Delete");
            var id = address.Id;

            //// Act
            _addressService.Delete(address);

            //// Assert
            var retrieved = _addressService.GetById(id);
            Assert.IsNull(retrieved);
        }

    }
}
