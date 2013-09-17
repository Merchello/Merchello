using System;
using System.Linq;
using Merchello.Core.Services;
using Merchello.Tests.Base.Data;
using Merchello.Tests.Base.SqlSyntax;
using NUnit.Framework;

namespace Merchello.Tests.IntegrationTests.Services
{
    [TestFixture]
    [Category("Service Integration")]
    public class CustomerServiceTests : BaseUsingSqlServerSyntax
    {
        private CustomerService _customerService;


        [SetUp]
        public override void Initialize()
        {
            base.Initialize();

            _customerService = new CustomerService();

            var all = _customerService.GetAll();
            _customerService.Delete(all);

        }
        

        [Test]
        public void Can_Add_A_Customer()
        {
            var customer = CustomerData.CustomerForInserting();

            _customerService.Save(customer);
          
            Assert.IsTrue(customer.HasIdentity);

        }

        [Test]
        public void Can_Add_A_List_Of_Three_Customers()
        {
            var customers = CustomerData.CustomerListForInserting();

            _customerService.Save(customers);

            Assert.IsTrue(customers.First().HasIdentity);
            Assert.IsTrue(customers.Last().HasIdentity);

            
        }


        //[Test]
        //public void Can_Delete_Every_Customers()
        //{
        //    var all = _customerService.GetAll();

        //    Console.WriteLine("Current count is : " + all.Count().ToString());

            
        //    _customerService.Delete(all);

        //    var count = _customerService.GetAll().Count();

        //    Assert.AreEqual(0, count);
        //}

        [TearDown]
        public override void TearDown()
        {
            base.TearDown();

            _customerService = null;
        }
    }
}
