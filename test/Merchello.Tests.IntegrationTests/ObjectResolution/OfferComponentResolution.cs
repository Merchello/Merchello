namespace Merchello.Tests.IntegrationTests.ObjectResolution
{
    using System;
    using System.Linq;

    using Merchello.Core.Marketing.Offer;
    using Merchello.Tests.Base.TestHelpers;
    using Merchello.Web.Discounts.Coupons.Constraints;

    using NUnit.Framework;

    [TestFixture]
    public class OfferComponentResolution : MerchelloAllInTestBase
    {
        private OfferComponentResolver _resolver;

        private readonly Guid _couponProviderkey = new Guid("1EED2CCB-4146-44BE-A5EB-DA3D2E3992A7");

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

        [Test]
        public void Can_Get_OfferComponents_For_The_Coupon_Provider()
        {
            var components = _resolver.GetOfferComponentsByProviderKey(_couponProviderkey);

            Assert.IsTrue(components.Any());
        }

        [Test]
        public void Can_Get_OfferComponents_For_The_Test_Provider()
        {
            // these should not include components restricted to the coupon provider
            var key = new Guid("AD4E890D-9D60-442A-A19A-6FE9EE3A1454"); // this it the TestProviders key

            var components = _resolver.GetOfferComponentsByProviderKey(key).ToArray();

            Assert.IsTrue(components.Any());
            Assert.IsFalse(components.Any(x => x.GetType() == typeof(OneCouponPerCustomerConstraint)));
        }
    }
}