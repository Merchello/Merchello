namespace Merchello.Core.Marketing.Discounts.Rules.Sale
{
    using System;

    using Merchello.Core.Marketing.Discounts.Rules;
    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// A discount rule to prohibit a discount from being used with other discounts.
    /// </summary>
    public class NotUsableWithOtherDiscountsRule : DiscountRuleBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotUsableWithOtherDiscountsRule"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public NotUsableWithOtherDiscountsRule(IDiscountRuleSettings settings)
            : base(settings)
        {
        }

        /// <summary>
        /// Gets the key.
        /// </summary>
        public override Guid Key
        {
            get
            {
                return new Guid("BDFEF8AC-B572-43E6-AB42-C07678500C87");
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public override string Name
        {
            get
            {
                return "Not usable with other discounts.";
            }
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        public override string Description
        {
            get
            {
                return "This discount cannot be used with other discounts";
            }
        }

        /// <summary>
        /// Gets the category.
        /// </summary>
        public override DiscountRuleCategory Category
        {
            get
            {
                return DiscountRuleCategory.Sale;                
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
        public override Attempt<ILineItemContainer> Validate(ICustomerBase customer, ILineItemContainer collection)
        {
            throw new NotImplementedException();
        }
    }
}