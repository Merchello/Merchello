namespace Merchello.Core.Discounts.Rewards
{
    using Merchello.Core.Models;

    public interface IDiscountReward
    {
        bool IsLineItem { get; set; }

        ILineItem DiscountLineItem { get; set; }
    }
}