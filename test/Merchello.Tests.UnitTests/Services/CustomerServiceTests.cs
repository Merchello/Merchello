using System;
using System.Linq;
using Merchello.Core.Models;
using Merchello.Core.Persistence;
using Merchello.Core.Services;
using Merchello.Tests.Base.DataMakers;
using Merchello.Tests.Base.Respositories;
using Merchello.Tests.Base.Respositories.UnitOfWork;
using Merchello.Tests.Base.Services;
using NUnit.Framework;
using Umbraco.Core.Events;

namespace Merchello.Tests.UnitTests.Services
{
    [TestFixture]
    [Category("Services")]
    public class CustomerServiceTests : ServiceTestsBase<ICustomer>
    {

        private CustomerService _customerService;

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            _customerService = new CustomerService(new MockUnitOfWorkProvider(), new RepositoryFactory(), new AnonymousCustomerService(), new CustomerAddressService(), new InvoiceService(), new PaymentService());
            Before = null;
            After = null;

            CustomerService.Saving += delegate(ICustomerService sender, SaveEventArgs<ICustomer> args)
            {
                BeforeTriggered = true;
                Before = args.SavedEntities.FirstOrDefault();
            };

            CustomerService.Saved += delegate(ICustomerService sender, SaveEventArgs<ICustomer> args)
            {
                AfterTriggered = true;
                After = args.SavedEntities.FirstOrDefault();
            };

            CustomerService.Creating += delegate(ICustomerService sender, Core.Events.NewEventArgs<ICustomer> args)
            {
                BeforeTriggered = true;
                Before = args.Entity;
            };

            CustomerService.Created += delegate(ICustomerService sender, Core.Events.NewEventArgs<ICustomer> args)
            {
                AfterTriggered = true;
                After = args.Entity;
            };

            CustomerService.Deleting += delegate(ICustomerService sender, DeleteEventArgs<ICustomer> args)
            {
                BeforeTriggered = true;
                Before = args.DeletedEntities.FirstOrDefault();
            };

            CustomerService.Deleted += delegate(ICustomerService sender, DeleteEventArgs<ICustomer> args)
            {
                AfterTriggered = true;
                After = args.DeletedEntities.FirstOrDefault();
            };

            // General tests
            MockDatabaseUnitOfWork.Committed += delegate(object sender)
            {
                CommitCalled = true;
            };


        }


        [Test]
        public void Create_Triggers_Event_Assert_And_Customer_Is_Passed()
        {
            var customer = _customerService.CreateCustomer("rusty", "Rusty", "Swayne", "test@test.com");

            Assert.IsTrue(BeforeTriggered);
        }

        [Test]
        public void Save_Triggers_Events_And_Customer_Is_Passed()
        {
            var customer = MockCustomerDataMaker.CustomerForInserting();

            _customerService.Save(customer);

            Assert.IsTrue(BeforeTriggered);
            Assert.AreEqual(customer.FirstName, Before.FirstName);

            Assert.IsTrue(AfterTriggered);
            Assert.AreEqual(customer.LastName, After.LastName);    
        }

        [Test]
        public void Save_Is_Committed()
        {
            var customer = MockCustomerDataMaker.CustomerForInserting();

            _customerService.Save(customer);

            Assert.IsTrue(CommitCalled);

        }

        // TODO THESE should be moved to integration tests now that the InvoiceService and PaymentService have been injected into the CustomerService

        ////[Test]
        ////public void Delete_Triggers_Events_And_Customer_Is_Passed()
        ////{
        ////    //// Arrange
        ////    var customer =  MockCustomerDataMaker
        ////                    .CustomerForInserting()
        ////                    .MockSavedWithKey(Guid.NewGuid());

        ////    //// Act
        ////    _customerService.Delete(customer);


        ////    Assert.IsTrue(BeforeTriggered);
        ////    Assert.AreEqual(customer, Before);

        ////    Assert.IsTrue(AfterTriggered);
        ////    Assert.AreEqual(customer, After);    
        ////}

        ////[Test]
        ////public void Delete_Is_Committed()
        ////{
        ////    //// Arrange
        ////    var customer = MockCustomerDataMaker.CustomerForInserting().MockSavedWithKey(Guid.NewGuid());

        ////    //// Act
        ////    _customerService.Delete(customer);
   
        ////    //// Assert
        ////    Assert.IsTrue(CommitCalled);
        ////}

    }
}
