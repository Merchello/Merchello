namespace Merchello.Core.Marketing.Discounts
{
    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Marketing.Rewards;
    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// Represents a discount line item reward.
    /// </summary>
    public abstract class DiscountLineItemReward : RewardBase
    {
        /// <summary>
        /// The <see cref="ICustomerBase"/>.
        /// </summary>
        private readonly ICustomerBase _customer;

        /// <summary>
        /// The <see cref="ILineItemContainer"/>.
        /// </summary>
        private readonly ILineItemContainer _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiscountLineItemReward"/> class.
        /// </summary>
        /// <param name="definition">
        /// The <see cref="OfferComponentDefinition"/>.
        /// </param>
        /// <param name="customer">
        /// The <see cref="ICustomerBase"/>.
        /// </param>
        /// <param name="container">
        /// The <see cref="ILineItemContainer"/>.
        /// </param>
        protected DiscountLineItemReward(OfferComponentDefinition definition, ICustomerBase customer, ILineItemContainer container)
            : base(definition)
        {
            Mandate.ParameterNotNull(customer, "customer");
            Mandate.ParameterNotNull(container, "container");

            _customer = customer;
            _container = container;
        }
    }
}