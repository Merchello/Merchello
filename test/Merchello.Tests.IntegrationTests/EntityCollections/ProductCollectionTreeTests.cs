namespace Merchello.Tests.IntegrationTests.EntityCollections
{
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Services;
    using Merchello.Tests.Base.TestHelpers;
    using Merchello.Web;
    using Merchello.Web.Models.Ui.Rendering;

    using NUnit.Framework;

    [TestFixture]
    public class ProductCollectionTreeTests : MerchelloAllInTestBase
    {
        private IEntityCollectionService _service;

        private IEnumerable<IProductCollection> _all;

        private MerchelloHelper _merchello;

        [TestFixtureSetUp]
        public override void FixtureSetup()
        {
            base.FixtureSetup();

            DbPreTestDataWorker.DeleteAllEntityCollections();

            _service = MerchelloContext.Current.Services.EntityCollectionService;

            var providerKey = Constants.ProviderKeys.EntityCollection.StaticProductCollectionProviderKey;

            var root1 = _service.CreateEntityCollectionWithKey(EntityType.Product, providerKey, "root1", false);

            var root2 = _service.CreateEntityCollectionWithKey(EntityType.Product, providerKey, "root2", false);

            var a = _service.CreateEntityCollection(EntityType.Product, providerKey, "a", false);
            a.ParentKey = root1.Key;

            var b = _service.CreateEntityCollection(EntityType.Product, providerKey, "b", false);
            b.ParentKey = root1.Key;

            var c = _service.CreateEntityCollection(EntityType.Product, providerKey, "c", false);
            c.ParentKey = root1.Key;

            _service.Save(new [] { a, b, c });

            var a1 = _service.CreateEntityCollection(EntityType.Product, providerKey, "a1", false);
            a1.ParentKey = a.Key;

            var a2 = _service.CreateEntityCollection(EntityType.Product, providerKey, "a2", false);
            a2.ParentKey = a.Key;

            _service.Save(new[] { a1, a2 });

            var a1a = _service.CreateEntityCollection(EntityType.Product, providerKey, "a1a", false);
            a1a.ParentKey = a1.Key;


            var a1b = _service.CreateEntityCollection(EntityType.Product, providerKey, "a1b", false);
            a1b.ParentKey = a1.Key;

            _service.Save(new[] { a1a, a1b });


            var c1 = _service.CreateEntityCollection(EntityType.Product, providerKey, "c1", false);
            c1.ParentKey = c.Key;

            var c2 = _service.CreateEntityCollection(EntityType.Product, providerKey, "c2", false);
            c2.ParentKey = c.Key;

            _service.Save(new[] { c1, c2 });

            var c2a = _service.CreateEntityCollection(EntityType.Product, providerKey, "c2a", false);
            c2a.ParentKey = c2.Key;

            _service.Save(c2a);

            _merchello = new MerchelloHelper();
        }

        [Test]
        public void Can_Query_All_RootNodes()
        {
            //// Arrange
            const int expected = 2;

            //// Act
            var collections = _merchello.Collections.Product.GetRootLevelCollections();

            //// Assert
            Assert.AreEqual(expected, collections.Count());
        }

        [Test]
        public void Can_GetAll_Collections()
        {
            //// Arrange
            const int expected = 12;

            //// Act
            var collections = _merchello.Collections.Product.GetAll();

            //// Assert
            Assert.AreEqual(expected, collections.Count());
        }
    }
}