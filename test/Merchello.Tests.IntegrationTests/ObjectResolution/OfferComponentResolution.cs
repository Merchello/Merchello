namespace Merchello.Tests.IntegrationTests.ObjectResolution
{
    using System.Linq;

    using Merchello.Core.Marketing.Offer;
    using Merchello.Tests.Base.TestHelpers;

    using NUnit.Framework;

    [TestFixture]
    public class OfferComponentResolution : MerchelloAllInTestBase
    {
        private OfferComponentResolver _resolver;

        [SetUp]
        public void Init()
        {
            _resolver = OfferComponentResolver.Current;
        }

        [Test]
        public void Can_Resolve_InstanceTypes_Of_OfferComponents()
        {
            var types = _resolver.InstanceTypes;

            Assert.IsTrue(types.Any());
        }
    }
}