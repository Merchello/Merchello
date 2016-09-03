namespace Merchello.Tests.IntegrationTests.EntityCollections
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Models.Interfaces;
    using Merchello.Core.Services;
    using Merchello.Core.Trees;
    using Merchello.Tests.Base.TestHelpers;
    using Merchello.Web;
    using Merchello.Web.Models;
    using Merchello.Web.Models.Ui.Rendering;
    using Merchello.Web.Search;

    using NUnit.Framework;

    [TestFixture]
    public class ProductCollectionTreeTests : MerchelloAllInTestBase
    {
        private IEntityCollectionService _service;

        private IProductCollectionTreeQuery _query;

        private IEnumerable<IProductCollection> _all;

        private MerchelloHelper _merchello;

        private IEntityCollection root2;

        private IEntityCollection           root1, 
                                    a,          b,      c, 
                                  a1,    a2,           c1, c2, 
                               a1a, a1b,                      c2a;

        [TestFixtureSetUp]
        public override void FixtureSetup()
        {
            base.FixtureSetup();

            DbPreTestDataWorker.DeleteAllEntityCollections();

            _service = MerchelloContext.Current.Services.EntityCollectionService;

            this._query = ProxyQueryManager.Current.Instance<ProductCollectionTreeQuery>();

            var providerKey = Constants.ProviderKeys.EntityCollection.StaticProductCollectionProviderKey;

            root1 = _service.CreateEntityCollectionWithKey(EntityType.Product, providerKey, "root1", false);

            root2 = _service.CreateEntityCollectionWithKey(EntityType.Product, providerKey, "root2", false);

            a = _service.CreateEntityCollection(EntityType.Product, providerKey, "a", false);
            a.ParentKey = root1.Key;

            b = _service.CreateEntityCollection(EntityType.Product, providerKey, "b", false);
            b.ParentKey = root1.Key;

            c = _service.CreateEntityCollection(EntityType.Product, providerKey, "c", false);
            c.ParentKey = root1.Key;

            _service.Save(new [] { a, b, c });

            a1 = _service.CreateEntityCollection(EntityType.Product, providerKey, "a1", false);
            a1.ParentKey = a.Key;

            a2 = _service.CreateEntityCollection(EntityType.Product, providerKey, "a2", false);
            a2.ParentKey = a.Key;

            _service.Save(new[] { a1, a2 });

            a1a = _service.CreateEntityCollection(EntityType.Product, providerKey, "a1a", false);
            a1a.ParentKey = a1.Key;


            a1b = _service.CreateEntityCollection(EntityType.Product, providerKey, "a1b", false);
            a1b.ParentKey = a1.Key;

            _service.Save(new[] { a1a, a1b });


            c1 = _service.CreateEntityCollection(EntityType.Product, providerKey, "c1", false);
            c1.ParentKey = c.Key;

            c2 = _service.CreateEntityCollection(EntityType.Product, providerKey, "c2", false);
            c2.ParentKey = c.Key;

            _service.Save(new[] { c1, c2 });

            c2a = _service.CreateEntityCollection(EntityType.Product, providerKey, "c2a", false);
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

        [Test]
        public void Can_Load_Query_Results_Into_Tree()
        {
            //// Arrange
            const int expected = 2;

            //// Act
            var trees = this._query.GetRootTrees();

            //// Assert
            Assert.AreEqual(2, trees.Count());
        }

        [Test]
        public void Can_Find_Tree_Containing_Node()
        {
            //// Arrange
            var rn = "root1";
            var cn = "a1";
            var collection = Convert(a1);
            Assert.AreEqual(cn, collection.Name, "Collection name did not match");

            //// Act
            var root = _query.GetTreeContaining(collection);

            //// Assert
            Assert.NotNull(root, "Tree was not found");
            Assert.AreEqual(rn, root.Value.Name, "Tree name did not match");
        }

        [Test]
        public void Can_Find_Node_Representation_Of_Node()
        {
            //// Arrange
            var cn = "c2";
            var collection = Convert(c2);

            //// Act
            var node = _query.GetTreeByValue(collection);

            //// Assert
            Assert.NotNull(node, "Did not find node");
            Assert.AreEqual(cn, node.Value.Name);
        }

        [Test]
        public void Can_Find_Ancestors_To_Root()
        {
            //// Arrange
            var rn = "root1";
            var cn = "c2";
            var expected = 2;
            var collection = Convert(c2);

            //// Act
            var ancestors = collection.Ancestors();

            //// Assert
            Assert.IsNotEmpty(ancestors, "Ancestors was empty");
            Assert.AreEqual(expected, ancestors.Count());
            Assert.AreEqual(rn, ancestors.Last().Name, "Ancestors root name did not match");
        }

        [Test]
        public void Can_Find_Ancestors_To_Root_With_Lambda()
        {
            //// Arrange
            var rn = "root1";
            var cn = "c2";
            var expected = 1;
            var collection = Convert(c2);

            //// Act
            var ancestors = collection.Ancestors(x => x.Name != "c");

            //// Assert
            Assert.IsNotEmpty(ancestors, "Ancestors was empty");
            Assert.AreEqual(expected, ancestors.Count());
            Assert.AreEqual(rn, ancestors.Last().Name, "Ancestors root name did not match");
        }


        [Test]
        public void Can_Find_AncestorsOrSelf_To_Root()
        {
            //// Arrange
            var rn = "root1";
            var cn = "c2a";
            var expected = 4;
            var collection = Convert(c2a);

            //// Act
            var ancestors = collection.AncestorsOrSelf();

            //// Assert
            Assert.IsNotEmpty(ancestors, "Ancestors was empty");
            Assert.AreEqual(expected, ancestors.Count());
            Assert.AreEqual(rn, ancestors.Last().Name, "Ancestors root name did not match");
        }

        [Test]
        public void Can_Find_Siblings()
        {
            //// Arrange;
            var cn = "a2";
            var expected = 1;
            var collection = Convert(a1);

            //// Act
            var siblings = collection.Siblings();

            //// Assert
            Assert.IsNotEmpty(siblings, "Siblings was empty");
            Assert.AreEqual(expected, siblings.Count());
            Assert.AreEqual(cn, siblings.Last().Name, "Siblings name did not match");
        }

        [Test]
        public void Can_Get_All_Descendants()
        {
            //// Arrange
            var expected = 3;
            var collection = Convert(c);

            //// Act
            var descendants = collection.Descendants();

            //// Assert
            Assert.IsNotEmpty(descendants, "Descendants was empty");
            Assert.AreEqual(expected, descendants.Count(), "Descendants count did not match");
            Console.WriteLine(string.Join(" ", descendants.Select(x => x.Name)));
        }

        private IProductCollection Convert(IEntityCollection ec)
        {
            return new ProductCollection(ec);
        }
    }
}