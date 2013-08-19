using Merchello.Tests.Base.Data;
using Merchello.Tests.Base.Services;
using NUnit.Framework;

namespace Merchello.Tests.UnitTests.Services
{
    [TestFixture]
    [Category("Services")]
    public class CustomerServiceTests : ServiceTestsBase
    {
        [Test]
        public void Create_Triggers_Event_Assert_And_Customer_Is_Passed()
        {
            var customer = CustomerService.CreateCustomer("Jo", "Jo");

            Assert.IsTrue(AfterTriggered);
        }

        [Test]
        public void Save_Triggers_Events_And_Customer_Is_Passed()
        {
            var customer = CustomerData.CustomerForInserting();

            CustomerService.Save(customer);

            Assert.IsTrue(BeforeTriggered);
            Assert.AreEqual(customer.FirstName, BeforeCustomer.FirstName);

            Assert.IsTrue(AfterTriggered);
            Assert.AreEqual(customer.LastName, AfterCustomer.LastName);    
        }

        [Test]
        public void Save_Is_Committed()
        {
            var customer = CustomerData.CustomerForInserting();

            CustomerService.Save(customer);

            Assert.IsTrue(CommitCalled);

        }

        [Test]
        public void Delete_Triggers_Events_And_Customer_Is_Passed()
        {
            var customer = CustomerData.CustomerForUpdating();

            CustomerService.Delete(customer);


            Assert.IsTrue(BeforeTriggered);
            Assert.AreEqual(customer.FirstName, BeforeCustomer.FirstName);

            Assert.IsTrue(AfterTriggered);
            Assert.AreEqual(customer.LastName, AfterCustomer.LastName);    
        }

        [Test]
        public void Delete_Is_Committed()
        {
            var customer = CustomerData.CustomerForUpdating();

            CustomerService.Delete(customer);
   
            Assert.IsTrue(CommitCalled);
        }
    }
}
