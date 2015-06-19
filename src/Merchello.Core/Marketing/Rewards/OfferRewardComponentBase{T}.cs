namespace Merchello.Core.Marketing.Rewards
{
    using System;

    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// The reward base.
    /// </summary>
    /// <typeparam name="TConstraint">
    /// The type to be passed to the constraints collection to validate if the reward should be awarded
    /// </typeparam>
    /// <typeparam name="TReward">
    /// The type of award to be returned
    /// </typeparam>
    public abstract class OfferRewardComponentBase<TConstraint, TReward> : OfferRewardComponentBase, IOfferRewardComponent<TConstraint, TReward>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OfferRewardComponentBase{TConstraint,TReward}"/> class.
        /// </summary>
        /// <param name="definition">
        /// The definition.
        /// </param>
        protected OfferRewardComponentBase(OfferComponentDefinition definition)
            : base(definition)
        {
        }

        /// <summary>
        /// Gets the Type of object this group uses to validate constraints.
        /// The to which this component can be grouped with
        /// </summary>
        internal override Type TypeGrouping
        {
            get
            {
                return typeof(TConstraint);
            }
        }

        /// <summary>
        /// Gets the reward type.
        /// This is used by the OfferProcessorFactory
        /// </summary>
        internal override Type RewardType
        {
            get
            {
                return typeof(TReward);
            }
        }

        /// <summary>
        /// Awards the reward.
        /// </summary>
        /// <param name="validate">
        /// The object to pass to the constraints collection
        /// </param>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <returns>
        /// A value indicating whether or not the awarding process was successful.
        /// </returns>
        public abstract Attempt<TReward> TryAward(TConstraint validate, ICustomerBase customer);
    }
}