using System.Linq;
using Merchello.Core.Services;
using Merchello.Tests.Base.DataMakers;
using NUnit.Framework;

namespace Merchello.Tests.IntegrationTests.Services
{
    [TestFixture]
    [Category("Service Integration")]
    public class CustomerServiceTests : ServiceIntegrationTestBase
    {
        private ICustomerService _customerService;

        [SetUp]
        public void Initialize()
        {            
            _customerService = PreTestDataWorker.CustomerService;            
        }
        
        /// <summary>
        /// Test to verify if a customer can be saved to the database
        /// </summary>
        [Test]
        public void Can_Add_A_Customer()
        {
            //// Arrange
            var customer = MockCustomerDataMaker.CustomerForInserting();

            //// Act
            _customerService.Save(customer);
          
            //// Assert
            Assert.IsTrue(customer.HasIdentity);
        }

        /// <summary>
        /// Test to verify that a collection of customers can be saved to the database
        /// </summary>
        [Test]
        public void Can_Save_A_Collection_Customers()
        {
            //// Arrange
            PreTestDataWorker.DeleteAllCustomers();
            var count = 10;
            var customers = MockCustomerDataMaker.CustomerListForInserting(count);

            //// Act
            _customerService.Save(customers);

            //// Assert
            var retrieved = ((CustomerService) _customerService).GetAll();
            Assert.IsTrue(retrieved.Any());
            Assert.AreEqual(count, retrieved.Count());
        }

    }
}
