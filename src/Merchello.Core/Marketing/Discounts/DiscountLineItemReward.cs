namespace Merchello.Core.Marketing.Discounts
{
    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Marketing.Rewards;
    using Merchello.Core.Models;

    public abstract class DiscountLineItemReward : RewardBase
    {
        protected DiscountLineItemReward(OfferComponentDefinition definition)
            : base(definition)
        {
        }

        public abstract void Apply(ICustomerBase customer, ILineItemContainer container);

    }
}