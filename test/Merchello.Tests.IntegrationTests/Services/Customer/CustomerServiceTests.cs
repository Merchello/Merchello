using Merchello.Core.Models;
using Merchello.Core.Services;
using NUnit.Framework;

namespace Merchello.Tests.IntegrationTests.Services.Customer
{
    [TestFixture]
    [Category("Service Integration")]
    public class CustomerServiceTests : ServiceIntegrationTestBase
    {
        private ICustomerService _customerService;        

        [SetUp]
        public void Initialize()
        {
            PreTestDataWorker.DeleteAllAnonymousCustomers();
            _customerService = PreTestDataWorker.CustomerService;
        }

        /// <summary>
        /// Tests verifies that an anonymous customer can be created and persisted
        /// </summary>
        [Test]
        public void Can_Create_An_AnonymousCustomer()
        {
            //// Arrange

            //// Act
            var anonymous = _customerService.CreateAnonymousCustomerWithKey();

            //// Assert
            Assert.NotNull(anonymous);
            Assert.IsTrue(anonymous.HasIdentity);
        }

        [Test]
        public void Can_Retrieve_An_AnonymousCustomer()
        {
            //// Arrange
            var anonymous = _customerService.CreateAnonymousCustomerWithKey();
            var key = anonymous.Key;

            //// Act
            var retrieved = _customerService.GetAnyByKey(key);

            //// Assert
            Assert.NotNull(retrieved);            
        }

    }
}