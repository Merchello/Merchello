﻿using System;
using System.Linq;
using Merchello.Core.Events;
using Merchello.Core.Models;
using Merchello.Core.Persistence;
using Merchello.Core.Services;
using Merchello.Tests.Base.DataMakers;
using Merchello.Tests.Base.Respositories;
using Merchello.Tests.Base.Respositories.UnitOfWork;
using Merchello.Tests.Base.Services;
using NUnit.Framework;

namespace Merchello.Tests.UnitTests.Services
{
    [TestFixture]
    [Category("Services")]
    public class CustomerServiceTests : ServiceTestsBase<ICustomer>
    {

        private CustomerService _customerService;


        protected override void Initialize()
        {
            _customerService = new CustomerService(new MockUnitOfWorkProvider(), new RepositoryFactory());
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


            CustomerService.Created += delegate(ICustomerService sender, NewEventArgs<ICustomer> args)
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
            var customer = _customerService.CreateCustomer("Jo", "Jo", "jo@test.com");

            Assert.IsTrue(AfterTriggered);
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

        [Test]
        public void Delete_Triggers_Events_And_Customer_Is_Passed()
        {
            //// Arrange
            var key = Guid.NewGuid();
            var customer =  MockCustomerDataMaker
                            .CustomerForInserting()
                            .MockSavedWithKey(key);

            //// Act
            _customerService.Delete(customer);


            Assert.IsTrue(BeforeTriggered);
            Assert.AreEqual(customer, Before);

            Assert.IsTrue(AfterTriggered);
            Assert.AreEqual(customer, After);    
        }

        [Test]
        public void Delete_Is_Committed()
        {
            //// Arrange
            var key = Guid.NewGuid();
            var customer = MockCustomerDataMaker.CustomerForInserting().MockSavedWithKey(key);

            //// Act
            _customerService.Delete(customer);
   
            //// Assert
            Assert.IsTrue(CommitCalled);
        }

    }
}
