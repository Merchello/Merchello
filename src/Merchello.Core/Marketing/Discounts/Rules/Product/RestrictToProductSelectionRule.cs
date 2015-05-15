namespace Merchello.Core.Marketing.Discounts.Rules.Product
{
    using System;

    using Merchello.Core.Marketing.Discounts.Rules;
    using Merchello.Core.Models;

    using Umbraco.Core;

    public class RestrictToProductSelectionRule : DiscountRuleBase
    {
        public RestrictToProductSelectionRule(IDiscountRuleSettings settings)
            : base(settings)
        {
        }

        public override Guid Key
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override string Name
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override string Description
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override DiscountRuleCategory Category
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override Attempt<ILineItemContainer> Validate(ICustomerBase customer, ILineItemContainer collection)
        {
            throw new NotImplementedException();
        }
    }
}