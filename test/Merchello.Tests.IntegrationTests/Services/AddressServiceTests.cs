using Merchello.Core.Services;
using Merchello.Tests.Base.DataMakers;
using NUnit.Framework;

namespace Merchello.Tests.IntegrationTests.Services
{
    [TestFixture]
    [Category("Service Integration")]
    public class AddressServiceTests : ServiceIntegrationTestBase
    {
        private IAddressService _addressService;

        [SetUp]
        public void Setup()
        {
            _addressService = PreTestDataWorker.AddressService;
        }

        /// <summary>
        /// Verifies that an address can be saved
        /// </summary>
        [Test]
        public void Can_Save_An_Address()
        {
            //// Arrange
            var customer = PreTestDataWorker.MakeExistingCustomer();
            var address = MockAddressDataMaker.RandomAddress(customer, "Studio");
            var hasIdentity = address.HasIdentity;

            Assert.IsFalse(hasIdentity);

            //// Act
            _addressService.Save(address);

            //// Assert
            Assert.IsTrue(address.HasIdentity);
        }

    }
}
