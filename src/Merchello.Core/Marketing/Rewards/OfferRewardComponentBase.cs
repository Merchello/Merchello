namespace Merchello.Core.Marketing.Rewards
{
    using Merchello.Core.Marketing.Offer;

    /// <summary>
    /// The offer reward component base.
    /// </summary>
    public abstract class OfferRewardComponentBase : OfferComponentBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OfferRewardComponentBase"/> class.
        /// </summary>
        /// <param name="definition">
        /// The definition.
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
    }
}