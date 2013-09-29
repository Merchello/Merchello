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
    public class CustomerRegistryServiceTests : ServiceTestsBase<ICustomerRegistry>
    {

        private CustomerRegistryService _customerRegistryService;
        private IAnonymousCustomer _anonymous;

        protected override void Initialize()
        {
            _customerRegistryService = new CustomerRegistryService(new MockUnitOfWorkProvider(), new RepositoryFactory());
            Before = null;
            After = null;

            _anonymous = MockAnonymousCustomerDataMaker
                         .AnonymousCustomerForInserting()
                         .MockSavedWithKey(Guid.NewGuid());

            CustomerRegistryService.Saving += delegate(ICustomerRegistryService sender, SaveEventArgs<ICustomerRegistry> args)
            {
                BeforeTriggered = true;
                Before = args.SavedEntities.FirstOrDefault();
            };

            CustomerRegistryService.Saved += delegate(ICustomerRegistryService sender, SaveEventArgs<ICustomerRegistry> args)
            {
                AfterTriggered = true;
                After = args.SavedEntities.FirstOrDefault();
            };


            CustomerRegistryService.Created += delegate(ICustomerRegistryService sender, Core.Events.NewEventArgs<ICustomerRegistry> args)
            {
                AfterTriggered = true;
                After = args.Entity;
            };

            CustomerRegistryService.Deleting += delegate(ICustomerRegistryService sender, DeleteEventArgs<ICustomerRegistry> args)
            {
                BeforeTriggered = true;
                Before = args.DeletedEntities.FirstOrDefault();
            };

            CustomerRegistryService.Deleted += delegate(ICustomerRegistryService sender, DeleteEventArgs<ICustomerRegistry> args)
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
            var basket = MockBasketDataMaker.AnonymousBasket(_anonymous, CustomerRegistryType.Basket);

            _customerRegistryService.Save(basket);

            Assert.IsTrue(BeforeTriggered);
            Assert.AreEqual(basket.ConsumerKey, Before.ConsumerKey);

            Assert.IsTrue(AfterTriggered);
            Assert.AreEqual(basket.CustomerRegistryType, After.CustomerRegistryType);
        }

        [Test]
        public void Save_Is_Committed()
        {

            var basket = MockBasketDataMaker.AnonymousBasket(_anonymous, CustomerRegistryType.Basket);
            _customerRegistryService.Save(basket);


            Assert.IsTrue(CommitCalled);

        }

        [Test]
        public void Delete_Triggers_Events_And_Basket_Is_Passed()
        {
            var basket = MockBasketDataMaker.AnonymousBasket(_anonymous, CustomerRegistryType.Basket);

            _customerRegistryService.Delete(basket);
            
            Assert.IsTrue(BeforeTriggered);
            Assert.AreEqual(basket.ConsumerKey, Before.ConsumerKey);

            Assert.IsTrue(AfterTriggered);
            Assert.AreEqual(basket.CustomerRegistryTfKey, After.CustomerRegistryTfKey);
        }

        [Test]
        public void Delete_Is_Committed()
        {
            var basket = MockBasketDataMaker.AnonymousBasket(_anonymous, CustomerRegistryType.Basket);

            _customerRegistryService.Delete(basket);

            Assert.IsTrue(CommitCalled);
        }

    }
}
