namespace Merchello.Tests.Unit.Configurations.MerchelloExtensibility
{
    using System.Linq;

    using NUnit.Framework;

    [TestFixture]
    public class BackOfficeTreesElementTests : MerchelloExtensibilityTests
    {
       
        [Test]
        public void SelfManagedProvidersBeforeStaticProviders()
        {
            //// Arrange
            var trees = ExtensibilitySection.BackOffice.Trees.ToArray();

            const bool notSales = false;
            const bool sales = true;

            //// Act
            var salesTree = trees.FirstOrDefault(x => x.RouteId == "sales");
            var others = trees.Where(x => x.RouteId != "sales");

            //// Assert
            Assert.NotNull(salesTree, "sales tree was null");
            Assert.NotNull(others, "others collection was null");
            Assert.AreEqual(5, others.Count(), "others collection count was not 5");

            Assert.AreEqual(sales, salesTree.SelfManagedProvidersBeforeStaticProviders);
            Assert.IsTrue(others.All(x => x.SelfManagedProvidersBeforeStaticProviders == notSales));
        }

        [Test]
        public void SalesTreeHasSelfManagedEntityCollectionProviders()
        {
            //// Arrange
            var trees = ExtensibilitySection.BackOffice.Trees;
            const int providerCount = 6;

            //// Act
            var treesWithProviders = trees.Where(x => x.SelfManagedEntityCollectionProviders.Any());
            Assert.NotNull(treesWithProviders);

            //// Assert 
            Assert.AreEqual(1, treesWithProviders.Count());
            Assert.AreEqual("sales", treesWithProviders.First().RouteId);
            Assert.AreEqual(providerCount, treesWithProviders.First().SelfManagedEntityCollectionProviders.Count());
        }
    }
}