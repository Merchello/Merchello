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
        protected CustomerService Service;
        protected bool BeforeTriggered;
        protected bool AfterTriggered;
        protected ICustomer BeforeCustomer;
        protected ICustomer AfterCustomer;
        protected bool CommitCalled;

        [SetUp]
        public void Setup()
        {
            Service = new CustomerService(new MockUnitOfWorkProvider(), new RepositoryFactory());

            BeforeTriggered = false;
            BeforeCustomer = null;
            AfterCustomer = null;
            AfterTriggered = false;
            CommitCalled = false;

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

            MockDatabaseUnitOfWork.Committed += delegate(object sender)
            {
                CommitCalled = true;
            };
        }
    }
}
