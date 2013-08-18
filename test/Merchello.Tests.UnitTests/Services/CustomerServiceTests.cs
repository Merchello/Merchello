using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Events;
using Merchello.Core.Models;
using Merchello.Core.Persistence;
using Merchello.Core.Services;
using Merchello.Tests.Base.Data;
using Merchello.Tests.Base.Respositories.UnitOfWork;
using Merchello.Tests.UnitTests.Repository;
using NUnit.Framework;

namespace Merchello.Tests.UnitTests.Services
{
    [TestFixture]
    [Category("Services")]
    public class CustomerServiceTests
    {
        
        private CustomerService _service;
        private bool _beforeTriggered;
        private bool _afterTriggered;
        private ICustomer _beforeCustomer;
        private ICustomer _afterCustomer;
        private bool _commitCalled;

        [SetUp]
        public void Setup()
        {
            _service = new CustomerService(new MockUnitOfWorkProvider(), new RepositoryFactory());

            _beforeTriggered = false;
            _beforeCustomer = null;
            _afterCustomer = null;
            _afterTriggered = false;
            _commitCalled = false;

            CustomerService.Saving += delegate(ICustomerService sender, SaveEventArgs<ICustomer> args)
                {
                    _beforeTriggered = true;
                    _beforeCustomer = args.SavedEntities.FirstOrDefault();
                };

            CustomerService.Saved += delegate(ICustomerService sender, SaveEventArgs<ICustomer> args)
                {
                    _afterTriggered = true;
                    _afterCustomer = args.SavedEntities.FirstOrDefault();
                };


            CustomerService.Created += delegate(ICustomerService sender, NewEventArgs<ICustomer> args)
                {
                    _afterTriggered = true;
                    _afterCustomer = args.Entity;
                };

            CustomerService.Deleting += delegate(ICustomerService sender, DeleteEventArgs<ICustomer> args)
                {
                    _beforeTriggered = true;
                    _beforeCustomer = args.DeletedEntities.FirstOrDefault();
                };

            CustomerService.Deleted += delegate(ICustomerService sender, DeleteEventArgs<ICustomer> args)
                {
                    _afterTriggered = true;
                    _afterCustomer = args.DeletedEntities.FirstOrDefault();
                };

            MockDatabaseUnitOfWork.Committed += delegate(object sender)
                {
                    _commitCalled = true;
                };
        }

        [Test]
        public void Create_Triggers_Event_Assert_And_Customer_Is_Passed()
        {
            var customer = _service.CreateCustomer("Jo", "Jo");

            Assert.IsTrue(_afterTriggered);
        }

        [Test]
        public void Save_Triggers_Events_And_Customer_Is_Passed()
        {
            var customer = CustomerData.CustomerForInserting();

            _service.Save(customer);

            Assert.IsTrue(_beforeTriggered);
            Assert.AreEqual(customer.FirstName, _beforeCustomer.FirstName);

            Assert.IsTrue(_afterTriggered);
            Assert.AreEqual(customer.LastName, _afterCustomer.LastName);    
        }

        [Test]
        public void Save_Is_Committed()
        {
            var customer = CustomerData.CustomerForInserting();

            _service.Save(customer);

            Assert.IsTrue(_commitCalled);

        }

        [Test]
        public void Delete_Triggers_Events_And_Customer_Is_Passed()
        {
            var customer = CustomerData.CustomerForUpdating();

            _service.Delete(customer);


            Assert.IsTrue(_beforeTriggered);
            Assert.AreEqual(customer.FirstName, _beforeCustomer.FirstName);

            Assert.IsTrue(_afterTriggered);
            Assert.AreEqual(customer.LastName, _afterCustomer.LastName);    
        }

        [Test]
        public void Delete_Is_Committed()
        {
            var customer = CustomerData.CustomerForUpdating();

            _service.Delete(customer);
   
            Assert.IsTrue(_commitCalled);
        }
    }
}
