namespace Merchello.Tests.UnitTests.Discounts
{
    public interface ICoupon
    {
        string RedeemCode { get; set; }

        ////IEnumerable<IDiscountRule> Rules { get; set; }
    }
}