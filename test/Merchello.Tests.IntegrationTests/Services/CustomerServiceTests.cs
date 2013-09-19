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
            PreTestDataWorker.DeleteAllCustomers();
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
        public void Can_Add_A_List_Of_Three_Customers()
        {
            //// Arrange
            var customers = MockCustomerDataMaker.CustomerListForInserting();

            //// Act
            _customerService.Save(customers);

            //// Assert
            Assert.IsTrue(customers.First().HasIdentity);
            Assert.IsTrue(customers.Last().HasIdentity);            
        }

    }
}
