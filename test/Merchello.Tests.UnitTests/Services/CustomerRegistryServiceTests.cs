using System;
using System.Linq;
using Merchello.Core;
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
    public class CustomerRegistryServiceTests : ServiceTestsBase<ICustomerItemRegister>
    {

        private CustomerItemRegisterService _customerItemRegisterService;
        private IAnonymousCustomer _anonymous;

        protected override void Initialize()
        {
            _customerItemRegisterService = new CustomerItemRegisterService(new MockUnitOfWorkProvider(), new RepositoryFactory());
            Before = null;
            After = null;

            _anonymous = MockAnonymousCustomerDataMaker
                         .AnonymousCustomerForInserting()
                         .MockSavedWithKey(Guid.NewGuid());

            CustomerItemRegisterService.Saving += delegate(ICustomerItemRegisterService sender, SaveEventArgs<ICustomerItemRegister> args)
            {
                BeforeTriggered = true;
                Before = args.SavedEntities.FirstOrDefault();
            };

            CustomerItemRegisterService.Saved += delegate(ICustomerItemRegisterService sender, SaveEventArgs<ICustomerItemRegister> args)
            {
                AfterTriggered = true;
                After = args.SavedEntities.FirstOrDefault();
            };


            CustomerItemRegisterService.Created += delegate(ICustomerItemRegisterService sender, Core.Events.NewEventArgs<ICustomerItemRegister> args)
            {
                AfterTriggered = true;
                After = args.Entity;
            };

            CustomerItemRegisterService.Deleting += delegate(ICustomerItemRegisterService sender, DeleteEventArgs<ICustomerItemRegister> args)
            {
                BeforeTriggered = true;
                Before = args.DeletedEntities.FirstOrDefault();
            };

            CustomerItemRegisterService.Deleted += delegate(ICustomerItemRegisterService sender, DeleteEventArgs<ICustomerItemRegister> args)
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
        public void Save_Triggers_Events_And_Basket_Is_Passed()
        {
            var basket = MockBasketDataMaker.AnonymousBasket(_anonymous, CustomerItemRegisterType.Basket);

            _customerItemRegisterService.Save(basket);

            Assert.IsTrue(BeforeTriggered);
            Assert.AreEqual(basket.ConsumerKey, Before.ConsumerKey);

            Assert.IsTrue(AfterTriggered);
            Assert.AreEqual(basket.CustomerItemRegisterType, After.CustomerItemRegisterType);
        }

        [Test]
        public void Save_Is_Committed()
        {

            var basket = MockBasketDataMaker.AnonymousBasket(_anonymous, CustomerItemRegisterType.Basket);
            _customerItemRegisterService.Save(basket);


            Assert.IsTrue(CommitCalled);

        }

        [Test]
        public void Delete_Triggers_Events_And_Basket_Is_Passed()
        {
            var basket = MockBasketDataMaker.AnonymousBasket(_anonymous, CustomerItemRegisterType.Basket);

            _customerItemRegisterService.Delete(basket);
            
            Assert.IsTrue(BeforeTriggered);
            Assert.AreEqual(basket.ConsumerKey, Before.ConsumerKey);

            Assert.IsTrue(AfterTriggered);
            Assert.AreEqual(basket.RegisterTfKey, After.RegisterTfKey);
        }

        [Test]
        public void Delete_Is_Committed()
        {
            var basket = MockBasketDataMaker.AnonymousBasket(_anonymous, CustomerItemRegisterType.Basket);

            _customerItemRegisterService.Delete(basket);

            Assert.IsTrue(CommitCalled);
        }

    }
}
