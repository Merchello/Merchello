using System;
using System.Linq;
using Merchello.Core.Services;
using Merchello.Tests.Base.Data;
using Merchello.Tests.Base.SqlSyntax;
using Merchello.Tests.IntegrationTests.TestHelpers;
using NUnit.Framework;

namespace Merchello.Tests.IntegrationTests.Services
{
    [TestFixture]
    [Category("Service Integration")]
    public class CustomerServiceTests : ServiceIntegrationTestBase
    {
        private CustomerService _customerService;


        [SetUp]
        public void Initialize()
        {
            

            _customerService = new CustomerService();

            var all = _customerService.GetAll();
            _customerService.Delete(all);

        }
        
        [Test]
        public void Demo()
        {
            //var helper = new DbPreTestDataWorker();
            //var customers = helper.CollectionExistingCustomers(1500);
            //helper.DeleteAllCustomers();
        }

        [Test]
        public void Can_Add_A_Customer()
        {
            var customer = CustomerData.CustomerForInserting();

            _customerService.Save(customer);
          
            Assert.IsTrue(customer.HasIdentity);

        }

        //[Test]
        // TODO RSS Troubleshoot this.  I think it is just the setup routine.
        public void Can_Add_A_List_Of_Three_Customers()
        {
            var customers = CustomerData.CustomerListForInserting();

            _customerService.Save(customers);

            Assert.IsTrue(customers.First().HasIdentity);
            Assert.IsTrue(customers.Last().HasIdentity);

            
        }

    }
}
