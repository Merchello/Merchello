namespace Merchello.Tests.UnitTests.Discounts
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Models;

    using Umbraco.Core;

    public class Coupon : DiscountOfferBase, ICoupon
    {
        public Coupon(IEnumerable<IDiscountConstraint> constraints)
            : base(constraints)
        {
        }

        public string RedeemCode { get; set; }

        protected override Attempt<IDiscountReward> ApplyReward(ILineItemContainer collection)
        {
            throw new NotImplementedException();
        }
    }
}