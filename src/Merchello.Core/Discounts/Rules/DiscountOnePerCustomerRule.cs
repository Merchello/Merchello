namespace Merchello.Core.Discounts.Rules
{
    using System;

    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// A rule to enforce one discount per customer.
    /// </summary>
    public class DiscountOnePerCustomerRule : DiscountRuleBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DiscountOnePerCustomerRule"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public DiscountOnePerCustomerRule(IDiscountRuleSettings settings)
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
                return new Guid("A035E592-5D09-40BD-BFF6-73C3A4E9DDA2");
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public override string Name
        {
            get
            {
                return "One per customer";
            }
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        public override string Description
        {
            get
            {
                return "The customer may only ever use this coupon once.";
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
            if (customer.IsAnonymous)
            {
                var anonymousException = new Exception("Customer must be signed in to use this discount.");
                return Attempt<ILineItemContainer>.Fail(collection, anonymousException);
            }

            throw new NotImplementedException();
        }        
    }
}