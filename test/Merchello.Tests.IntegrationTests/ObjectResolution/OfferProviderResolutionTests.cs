namespace Merchello.Tests.IntegrationTests.ObjectResolution
{
    using System;
    using System.Linq;

    using Merchello.Core.Marketing.Offer;
    using Merchello.Tests.Base.Offers;
    using Merchello.Tests.Base.TestHelpers;

    using NUnit.Framework;

    [TestFixture]
    public class OfferProviderResolutionTests : MerchelloAllInTestBase
    {

        [Test]
        public void OfferProviderResolver_Singleton_Is_Setup_In_BootManager()
        {
            //// Arrange
            // Handled in the base class (CoreBootManager)

            //// Act
            // nothing to do

            //// Assert
            Assert.IsTrue(OfferProviderResolver.HasCurrent); 
        }

        [Test]
        public void Can_Resolve_OfferProviders()
        {
            //// Arrange
            // Handled in the base class (CoreBootManager)

            //// Act
            var providers = OfferProviderResolver.Current.GetOfferProviders();

            //// Assert
            Assert.IsTrue(providers.Any());
        }


        [Test]
        public void Can_Get_A_ResolvedProvider_By_Its_Key()
        {
            //// Arrange
            var key = new Guid("AD4E890D-9D60-442A-A19A-6FE9EE3A1454"); // this it the TestProviders key

            //// Act
            var testProvider = OfferProviderResolver.Current.GetByKey(key);

            //// Assert
            Assert.NotNull(testProvider);
            Assert.AreEqual(typeof(TestOfferManager), testProvider.GetType());

        }

        [Test]
        public void Can_Get_A_ResolvedProvider_By_Its_Type()
        {
            //// Arrange
            
            //// Act
            var testProvider = OfferProviderResolver.Current.GetOfferProvider<TestOfferManager>();

            //// Assert
            Assert.NotNull(testProvider);
            Assert.AreEqual(typeof(TestOfferManager), testProvider.GetType());
        }
    }
}