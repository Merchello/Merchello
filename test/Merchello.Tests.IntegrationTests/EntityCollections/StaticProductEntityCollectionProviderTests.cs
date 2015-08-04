namespace Merchello.Tests.IntegrationTests.EntityCollections
{
    using System;
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.EntityCollections;
    using Merchello.Core.EntityCollections.Providers;
    using Merchello.Core.Models;
    using Merchello.Core.Services;
    using Merchello.Tests.Base.DataMakers;
    using Merchello.Tests.Base.TestHelpers;

    using NUnit.Framework;

    [TestFixture]
    public class StaticProductEntityCollectionProviderTests : MerchelloAllInTestBase
    {
        private IEntityCollectionService _entityCollectionService;

        private IProductService _productService;

        private Guid _providerKey;

        private IEntityCollectionProviderResolver _resolver;

        [TestFixtureSetUp]
        public override void FixtureSetup()
        {
            base.FixtureSetup();

            _entityCollectionService = DbPreTestDataWorker.EntityCollectionService;
            _productService = DbPreTestDataWorker.ProductService;
            _resolver = EntityCollectionProviderResolver.Current;
            _providerKey = _resolver.GetProviderKey<StaticProductCollectionProvider>();
        }

        [SetUp]
        public void Setup()
        {
            DbPreTestDataWorker.DeleteAllEntityCollections();
        }

        [Test]
        public void Can_Create_An_Entity_Collection()
        {
            //// Arrage
            

            //// Act
            var collection = _entityCollectionService.CreateEntityCollectionWithKey(
                EntityType.Product,
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
        public void Can_Add_Products_To_Collections()
        {
            //// Arrange
            var products = MockProductDataMaker.MockProductCollectionForInserting(10).ToArray();
            _productService.Save(products);
            
            var collection1 = _entityCollectionService.CreateEntityCollectionWithKey(
                EntityType.Product,
                _providerKey,
                "Product Collection1");

            var provider1 = collection1.ResolveProvider<StaticProductCollectionProvider>();
            Assert.NotNull(provider1);
            Assert.AreEqual(typeof(StaticProductCollectionProvider), provider1.GetType());

            var collection2 = _entityCollectionService.CreateEntityCollectionWithKey(
                EntityType.Product,
                _providerKey,
                "Product Collection2");

            var provider2 = collection2.ResolveProvider<StaticProductCollectionProvider>();
            Assert.NotNull(provider2);
            Assert.AreEqual(typeof(StaticProductCollectionProvider), provider2.GetType());


            //// Act
            var odd = false;
            foreach (var p in products)
            {
                odd = !odd;
                p.AddToCollection(collection1.Key);
                if (odd) p.AddToCollection(collection2.Key);
            }

            //// Assert
            var c1Products = collection1.GetEntities<IProduct>().ToArray();
            var c2Products = collection2.GetEntities<IProduct>().ToArray();
            Assert.IsTrue(c1Products.Any());
            Assert.IsTrue(c2Products.Any());

            Assert.Greater(c1Products.Count(), c2Products.Count());

            var p1 = c1Products.First();
            Assert.IsTrue(p1.GetCollectionsContaining().Any());

        }

        [Test]
        public void Can_Remove_Products_From_A_Collection()
        {
            //// Arrange
            var products = MockProductDataMaker.MockProductCollectionForInserting(4).ToArray();
            _productService.Save(products);

            var collection1 = _entityCollectionService.CreateEntityCollectionWithKey(
               EntityType.Product,
               _providerKey,
               "Product Collection1");

            //// Act
            foreach (var p in products)
            {
                p.AddToCollection(collection1.Key);
            }

            var provider = collection1.ResolveProvider<StaticProductCollectionProvider>();
            Assert.NotNull(provider);

            var cproducts = collection1.GetEntities<IProduct>().ToArray();
            Assert.AreEqual(4, cproducts.Count());

            var remove = cproducts.First();
            var key = remove.Key;

            remove.RemoveFromCollection(collection1);

            //// Assert
            var afterRemove = collection1.GetEntities<IProduct>().ToArray();
            Assert.AreEqual(3, afterRemove.Count());
            Assert.False(afterRemove.Any(x => x.Key == key));
            Assert.IsFalse(collection1.Exists(remove));
            Assert.IsFalse(remove.GetCollectionsContaining().Any());

            Assert.IsFalse(collection1.ChildCollections().Any());
        }
    }
}