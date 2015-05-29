namespace Merchello.Tests.IntegrationTests.DisplayClasses
{
    using System;
    using System.Linq;

    using Merchello.Core.Marketing.Offer;
    using Merchello.Tests.Base.TestHelpers;
    using Merchello.Web.Models.ContentEditing;

    using NUnit.Framework;

    [TestFixture]
    public class OfferComponentDefinitionDisplayTests : MerchelloAllInTestBase
    {
        private OfferComponentResolver _resolver;

        private readonly Guid _couponProviderkey = new Guid("1EED2CCB-4146-44BE-A5EB-DA3D2E3992A7");

        [SetUp]
        public void Init()
        {
            _resolver = OfferComponentResolver.Current;
        }

        [Test]
        public void Can_Map_OfferComponentDefinitions_To_OfferComponentDefinitionDisplays()
        {
            //// Arrange
            var components = _resolver.GetOfferComponentsByProviderKey(_couponProviderkey);

            //// Act
            var displays = components.Select(x => x.ToOfferComponentDefinitionDisplay());

            //// Assert
            Assert.IsTrue(displays.Any());
        }
    }
}