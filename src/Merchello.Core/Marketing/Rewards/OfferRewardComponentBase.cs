namespace Merchello.Core.Marketing.Rewards
{
    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Models;

    /// <summary>
    /// The reward base.
    /// </summary>
    public abstract class OfferRewardComponentBase : OfferComponentBase, IOfferRewardComponent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OfferRewardComponentBase"/> class.
        /// </summary>
        /// <param name="definition">
        /// The <see cref="OfferComponentDefinition"/>.
        /// </param>
        protected OfferRewardComponentBase(OfferComponentDefinition definition)
            : base(definition)
        {
        }

        /// <summary>
        /// Gets the component type.
        /// </summary>
        public override OfferComponentType ComponentType
        {
            get
            {
                return OfferComponentType.Reward;
            }
        }

        /// <summary>
        /// Awards the reward.
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <param name="container">
        /// The container.
        /// </param>
        /// <returns>
        /// A value indicating whether or not the awarding process was successful.
        /// </returns>
        public abstract bool Award(ICustomerBase customer, ILineItemContainer container);
    }
}