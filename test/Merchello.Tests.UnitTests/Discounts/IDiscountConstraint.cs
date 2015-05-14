namespace Merchello.Tests.UnitTests.Discounts
{
    using Merchello.Core.Models;

    using Umbraco.Core;

    public interface IDiscountConstraint
    {
        Attempt<ILineItemContainer> Validate(ILineItemContainer collection);
    }
}
