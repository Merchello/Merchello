namespace Merchello.Tests.Base.DataMakers
{
    using System;

    using Merchello.Core.Models;
    using Merchello.Core.Models.Interfaces;
    using Merchello.Web.Discounts.Coupons;

    public class MockCouponDataMaker
    {
        public static ICoupon CouponForInserting()
        {
            return new Coupon(new OfferSettings("Test coupon", "test", CouponManager.Instance.Key));

        }
    }
}