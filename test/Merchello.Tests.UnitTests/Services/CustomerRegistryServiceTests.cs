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
    public class CustomerRegistryServiceTests : ServiceTestsBase<IItemCache>
    {

        private ItemCacheService _itemCacheService;
        private IAnonymousCustomer _anonymous;

        [TestFixtureSetUp]
        public override void FixtureSetup()
        {
            base.FixtureSetup();

            _itemCacheService = new ItemCacheService(new MockUnitOfWorkProvider(), new RepositoryFactory());
            Before = null;
            After = null;

            _anonymous = MockAnonymousCustomerDataMaker
                         .AnonymousCustomerForInserting()
                         .MockSavedWithKey(Guid.NewGuid());

            ItemCacheService.Saving += delegate(IItemCacheService sender, SaveEventArgs<IItemCache> args)
            {
                BeforeTriggered = true;
                Before = args.SavedEntities.FirstOrDefault();
            };

            ItemCacheService.Saved += delegate(IItemCacheService sender, SaveEventArgs<IItemCache> args)
            {
                AfterTriggered = true;
                After = args.SavedEntities.FirstOrDefault();
            };


            ItemCacheService.Created += delegate(IItemCacheService sender, Core.Events.NewEventArgs<IItemCache> args)
            {
                AfterTriggered = true;
                After = args.Entity;
            };

            ItemCacheService.Deleting += delegate(IItemCacheService sender, DeleteEventArgs<IItemCache> args)
            {
                BeforeTriggered = true;
                Before = args.DeletedEntities.FirstOrDefault();
            };

            ItemCacheService.Deleted += delegate(IItemCacheService sender, DeleteEventArgs<IItemCache> args)
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
            var basket = MockCustomerItemCacheDataMaker.AnonymousBasket(_anonymous, ItemCacheType.Basket);

            _itemCacheService.Save(basket);

            Assert.IsTrue(BeforeTriggered);
            Assert.AreEqual(basket.EntityKey, Before.EntityKey);

            Assert.IsTrue(AfterTriggered);
            Assert.AreEqual(basket.ItemCacheType, After.ItemCacheType);
        }

        [Test]
        public void Save_Is_Committed()
        {

            var basket = MockCustomerItemCacheDataMaker.AnonymousBasket(_anonymous, ItemCacheType.Basket);
            _itemCacheService.Save(basket);


            Assert.IsTrue(CommitCalled);

        }

        [Test]
        public void Delete_Triggers_Events_And_Basket_Is_Passed()
        {
            var basket = MockCustomerItemCacheDataMaker.AnonymousBasket(_anonymous, ItemCacheType.Basket);

            _itemCacheService.Delete(basket);
            
            Assert.IsTrue(BeforeTriggered);
            Assert.AreEqual(basket.EntityKey, Before.EntityKey);

            Assert.IsTrue(AfterTriggered);
            Assert.AreEqual(basket.ItemCacheTfKey, After.ItemCacheTfKey);
        }

        [Test]
        public void Delete_Is_Committed()
        {
            var basket = MockCustomerItemCacheDataMaker.AnonymousBasket(_anonymous, ItemCacheType.Basket);

            _itemCacheService.Delete(basket);

            Assert.IsTrue(CommitCalled);
        }

    }
}
