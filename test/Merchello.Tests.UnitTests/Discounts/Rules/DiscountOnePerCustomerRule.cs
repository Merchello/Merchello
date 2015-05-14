namespace Merchello.Tests.UnitTests.Discounts
{
    using System;

    using Merchello.Core.Models;

    using Umbraco.Core;

    [DiscountRuleEditor("")]
    public class DiscountOnePerCustomerRule : DiscountRuleBase
    {

        public override Attempt<ILineItemContainer> Validate(ILineItemContainer collection)
        {
            throw new NotImplementedException();
        }

        public override Guid Key
        {
            get
            {
                return new Guid("A035E592-5D09-40BD-BFF6-73C3A4E9DDA2");
            }
        }

        public override string Name
        {
            get
            {
                return "One per customer";
            }
        }

        public override string Description
        {
            get
            {
                return "The customer may only ever use this coupon once.";
            }
        }
    }
}