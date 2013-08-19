using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Events;
using Merchello.Core.Models;
using Merchello.Core.Persistence;
using Merchello.Core.Services;
using Merchello.Tests.Base.Respositories;
using Merchello.Tests.Base.Respositories.UnitOfWork;
using NUnit.Framework;

namespace Merchello.Tests.Base.Services
{
    public class ServiceTestsBase
    {
        protected CustomerService CustomerService;
        protected ICustomer BeforeCustomer;
        protected ICustomer AfterCustomer;

        protected AddressService AddressService;
        protected IAddress BeforeAddress;
        protected IAddress AfterAddress;

        protected bool BeforeTriggered;
        protected bool AfterTriggered;
        protected bool CommitCalled;

        [SetUp]
        public void Setup()
        {

            // General trigger setup
            BeforeTriggered = false;
            AfterTriggered = false;
            CommitCalled = false;

            // Customer setup
            CustomerSetup();

            // Address setup
            AddressSetup();

            // General tests
            MockDatabaseUnitOfWork.Committed += delegate(object sender)
            {
                CommitCalled = true;
            };
        }

        private void CustomerSetup()
        {
            CustomerService = new CustomerService(new MockUnitOfWorkProvider(), new RepositoryFactory());
            BeforeCustomer = null;
            AfterCustomer = null;

            CustomerService.Saving += delegate(ICustomerService sender, SaveEventArgs<ICustomer> args)
            {
                BeforeTriggered = true;
                BeforeCustomer = args.SavedEntities.FirstOrDefault();
            };

            CustomerService.Saved += delegate(ICustomerService sender, SaveEventArgs<ICustomer> args)
            {
                AfterTriggered = true;
                AfterCustomer = args.SavedEntities.FirstOrDefault();
            };


            CustomerService.Created += delegate(ICustomerService sender, NewEventArgs<ICustomer> args)
            {
                AfterTriggered = true;
                AfterCustomer = args.Entity;
            };

            CustomerService.Deleting += delegate(ICustomerService sender, DeleteEventArgs<ICustomer> args)
            {
                BeforeTriggered = true;
                BeforeCustomer = args.DeletedEntities.FirstOrDefault();
            };

            CustomerService.Deleted += delegate(ICustomerService sender, DeleteEventArgs<ICustomer> args)
            {
                AfterTriggered = true;
                AfterCustomer = args.DeletedEntities.FirstOrDefault();
            };


        }

        private void AddressSetup()
        {
            AddressService = new AddressService(new MockUnitOfWorkProvider(), new RepositoryFactory());
            BeforeAddress = null;
            AfterAddress = null;

            AddressService.Saving += delegate(IAddressService sender, SaveEventArgs<IAddress> args)
            {
                BeforeTriggered = true;
                BeforeAddress = args.SavedEntities.FirstOrDefault();
            };

            AddressService.Saved += delegate(IAddressService sender, SaveEventArgs<IAddress> args)
            {
                AfterTriggered = true;
                AfterAddress = args.SavedEntities.FirstOrDefault();
            };


            AddressService.Created += delegate(IAddressService sender, NewEventArgs<IAddress> args)
            {
                AfterTriggered = true;
                AfterAddress = args.Entity;
            };

            AddressService.Deleting += delegate(IAddressService sender, DeleteEventArgs<IAddress> args)
            {
                BeforeTriggered = true;
                BeforeAddress = args.DeletedEntities.FirstOrDefault();
            };

            AddressService.Deleted += delegate(IAddressService sender, DeleteEventArgs<IAddress> args)
            {
                AfterTriggered = true;
                AfterAddress = args.DeletedEntities.FirstOrDefault();
            };

        }
    }
}
