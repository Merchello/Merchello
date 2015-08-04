namespace Merchello.Tests.IntegrationTests.EntityCollections
{
    using System;
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.EntityCollections;
    using Merchello.Core.EntityCollections.Providers;
    using Merchello.Core.Models;
    using Merchello.Core.Services;
    using Merchello.Tests.Base.TestHelpers;

    using NUnit.Framework;

    [TestFixture]
    public class StaticCustomerCollectionProviderTests : MerchelloAllInTestBase
    {
        private IEntityCollectionService _entityCollectionService;

        private ICustomerService _customerService;

        private Guid _providerKey;

        private IEntityCollectionProviderResolver _resolver;


        [TestFixtureSetUp]
        public override void FixtureSetup()
        {
            base.FixtureSetup();

            _entityCollectionService = DbPreTestDataWorker.EntityCollectionService;
            _customerService = DbPreTestDataWorker.CustomerService;
            _resolver = EntityCollectionProviderResolver.Current;
            _providerKey = _resolver.GetProviderKey<StaticCustomerCollectionProvider>();
        }

        [SetUp]
        public void Setup()
        {
            DbPreTestDataWorker.DeleteAllEntityCollections();
            DbPreTestDataWorker.DeleteAllCustomers();
        }

        [Test]
        public void Can_Create_An_Entity_Collection()
        {
            //// Arrage


            //// Act
            var collection = _entityCollectionService.CreateEntityCollectionWithKey(
                EntityType.Customer,
                _providerKey,
                "Test Collection");

            //// Assert
            Assert.NotNull(collection);
            Assert.IsTrue(collection.HasIdentity);
            Assert.AreEqual("Test Collection", collection.Name);

            var provider = collection.ResolveProvider();

            Assert.NotNull(provider);
            Assert.IsTrue(provider.GetManagedCollections().Any());
        }

        [Test]
        public void Can_Add_Customers_To_Collections()
        {
            //// Arrange
            var customer1 = DbPreTestDataWorker.MakeExistingCustomer(Guid.NewGuid().ToString());
            var customer2 = DbPreTestDataWorker.MakeExistingCustomer(Guid.NewGuid().ToString());
            var customer3 = DbPreTestDataWorker.MakeExistingCustomer(Guid.NewGuid().ToString());
            var customer4 = DbPreTestDataWorker.MakeExistingCustomer(Guid.NewGuid().ToString());


            var collection1 = _entityCollectionService.CreateEntityCollectionWithKey(
                EntityType.Customer,
                _providerKey,
                "Customer Collection1");

            var provider1 = collection1.ResolveProvider<StaticCustomerCollectionProvider>();
            Assert.NotNull(provider1);
            Assert.AreEqual(typeof(StaticCustomerCollectionProvider), provider1.GetType());

            var collection2 = _entityCollectionService.CreateEntityCollectionWithKey(
                EntityType.Customer,
                _providerKey,
                "Customer Collection2");

            var provider2 = collection2.ResolveProvider<StaticCustomerCollectionProvider>();
            Assert.NotNull(provider2);
            Assert.AreEqual(typeof(StaticCustomerCollectionProvider), provider2.GetType());


            //// Act
            customer1.AddToCollection(collection1);
            customer2.AddToCollection(collection1);
            customer3.AddToCollection(collection1);
            customer4.AddToCollection(collection1);
            customer3.AddToCollection(collection2);
            customer4.AddToCollection(collection2);

            //// Assert
            var c1Customers = collection1.GetEntities<ICustomer>().ToArray();
            var c2Customers = collection2.GetEntities<ICustomer>().ToArray();
            Assert.IsTrue(c1Customers.Any());
            Assert.IsTrue(c2Customers.Any());

            Assert.AreEqual(4, c1Customers.Count());
            Assert.AreEqual(2, c2Customers.Count());

            var c1 = c1Customers.First();
            Assert.IsTrue(c1.GetCollectionsContaining().Any());
        }

        [Test]
        public void Can_Remove_Customers_From_A_Collection()
        {
            //// Arrange
            var customer1 = DbPreTestDataWorker.MakeExistingCustomer(Guid.NewGuid().ToString());
            var customer2 = DbPreTestDataWorker.MakeExistingCustomer(Guid.NewGuid().ToString());
            var customer3 = DbPreTestDataWorker.MakeExistingCustomer(Guid.NewGuid().ToString());
            var customer4 = DbPreTestDataWorker.MakeExistingCustomer(Guid.NewGuid().ToString());


            var collection1 = _entityCollectionService.CreateEntityCollectionWithKey(
                EntityType.Customer,
                _providerKey,
                "Customer Collection1");


            customer1.AddToCollection(collection1);
            customer2.AddToCollection(collection1);
            customer3.AddToCollection(collection1);
            customer4.AddToCollection(collection1);
            var provider = collection1.ResolveProvider<StaticCustomerCollectionProvider>();
            Assert.NotNull(provider);

            //// Act

            var ccustomers = collection1.GetEntities<ICustomer>().ToArray();
            Assert.AreEqual(4, ccustomers.Count());

            var remove = ccustomers.First();
            var key = remove.Key;

            remove.RemoveFromCollection(collection1);

            //// Assert
            var afterRemove = collection1.GetEntities<ICustomer>().ToArray();
            Assert.AreEqual(3, afterRemove.Count());
            Assert.False(afterRemove.Any(x => x.Key == key));
            Assert.IsFalse(collection1.Exists(remove));
            Assert.IsFalse(remove.GetCollectionsContaining().Any());
        }
    }
}