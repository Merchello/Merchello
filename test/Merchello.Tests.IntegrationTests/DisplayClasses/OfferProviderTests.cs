namespace Merchello.Tests.IntegrationTests.DisplayClasses
{
    using Merchello.Core.Marketing.Offer;
    using Merchello.Tests.Base.TestHelpers;
    using Merchello.Web.Discounts.Coupons;
    using Merchello.Web.Models.ContentEditing;

    using NUnit.Framework;

    [TestFixture]
    public class OfferProviderTests : MerchelloAllInTestBase 
    {
        [Test]
        public void Can_Map_An_OfferProvider_To_OfferProviderDisplay()
        {
            //// Arrange
            var provider = OfferProviderResolver.Current.GetOfferProvider<CouponManager>();

            //// Act
            var display = provider.ToOfferProviderDisplay();

            //// Assert
            Assert.NotNull(display);
            Assert.AreEqual("Coupon", display.BackOfficeTree.Title);
        }
    }
}