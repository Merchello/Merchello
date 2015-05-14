namespace Merchello.Tests.UnitTests.Discounts
{
    using Merchello.Core.Models;

    public interface IDiscountReward
    {
        bool IsLineItem { get; set; }

        ILineItem DiscountLineItem { get; set; }
    }
}