namespace Merchello.Core.Marketing.Discounts.Rules
{
    using System;

    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// A base class for Discount Rules
    /// </summary>
    public abstract class DiscountRuleBase : IDiscountRule
    {
        /// <summary>
        /// The <see cref="IDiscountRuleSettings"/>.
        /// </summary>
        private readonly IDiscountRuleSettings _settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiscountRuleBase"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        protected DiscountRuleBase(IDiscountRuleSettings settings)
        {
            Mandate.ParameterNotNull(settings, "settings");

            this._settings = settings;
        }

        /// <summary>
        /// Gets the key.
        /// </summary>
        public abstract Guid Key { get; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the description.
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// Gets the category.
        /// </summary>
        public abstract DiscountRuleCategory Category { get; }

        /// <summary>
        /// Gets the <see cref="IDiscountRuleSettings"/>.
        /// </summary>
        protected IDiscountRuleSettings Settings
        {
            get
            {
                return this._settings;
            }
        }

        /// <summary>
        /// Validates the constraint against the <see cref="ILineItemContainer"/>
        /// </summary>
        /// <param name="customer">
        /// The <see cref="ICustomerBase"/>.
        /// </param>
        /// <param name="collection">
        /// The collection.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt{ILineItemContainer}"/> indicating whether or not the constraint can be enforced.
        /// </returns>
        public abstract Attempt<ILineItemContainer> Validate(ICustomerBase customer, ILineItemContainer collection);
    }
}