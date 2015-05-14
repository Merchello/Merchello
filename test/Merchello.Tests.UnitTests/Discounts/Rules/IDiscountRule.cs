namespace Merchello.Tests.UnitTests.Discounts
{
    using System;

    public interface IDiscountRule : IDiscountConstraint
    {
        Guid Key { get; }

        string Name { get; }

        string Description { get; }
    }
}