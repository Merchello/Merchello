namespace Merchello.Core.Marketing.Discounts.Rules
{
    using System;

    using Merchello.Core.Discounts;

    /// <summary>
    /// Defines a Discount Rule.
    /// </summary>
    public interface IDiscountRule : IDiscountConstraint
    {
        /// <summary>
        /// Gets the key.
        /// </summary>
        Guid Key { get; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the description.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets the category.
        /// </summary>
        DiscountRuleCategory Category { get; }
    }
}