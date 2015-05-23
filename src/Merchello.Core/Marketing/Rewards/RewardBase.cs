namespace Merchello.Core.Marketing.Rewards
{
    using Merchello.Core.Marketing.Offer;

    /// <summary>
    /// The reward base.
    /// </summary>
    public abstract class RewardBase : OfferComponentBase, IReward
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RewardBase"/> class.
        /// </summary>
        /// <param name="definition">
        /// The <see cref="OfferComponentDefinition"/>.
        /// </param>
        protected RewardBase(OfferComponentDefinition definition)
            : base(definition)
        {
        }

        /// <summary>
        /// Awards the reward.
        /// </summary>
        /// <returns>
        /// A value indicating whether or not the awarding process was successful.
        /// </returns>
        public abstract bool Award();
    }
}