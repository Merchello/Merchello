namespace Merchello.Tests.UnitTests.Discounts
{
    using System;

    using Merchello.Core.Models;

    using Umbraco.Core;

    public abstract class DiscountRuleBase : IDiscountRule
    {

        public abstract Attempt<ILineItemContainer> Validate(ILineItemContainer collection);

        public abstract Guid Key { get;}

        public abstract string Name { get; }

        public abstract string Description { get; }
    }
}