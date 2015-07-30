namespace Merchello.Tests.IntegrationTests.ObjectResolution
{
    using Merchello.Core.EntityCollections;
    using Merchello.Core.EntityCollections.Providers;
    using Merchello.Tests.Base.TestHelpers;

    using NUnit.Framework;

    [TestFixture]
    public class EntityCollectionResolution : MerchelloAllInTestBase
    {

        /// <summary>
        /// Test shows a key can be resolved from the attribute based on the type of provider
        /// </summary>
        [Test]
        public void Can_Resolve_StaticProductCollectionProviders_Key()
        {
            //// Arrange
            var expected = Core.Constants.ProviderKeys.EntityCollection.StaticProductCollectionProviderKey;
            var resolver = EntityCollectionProviderResolver.Current;

            //// Act
            var key = resolver.GetProviderKey<StaticProductCollectionProvider>();

            //// Assert
            Assert.AreEqual(expected, key);
        }
    }
}