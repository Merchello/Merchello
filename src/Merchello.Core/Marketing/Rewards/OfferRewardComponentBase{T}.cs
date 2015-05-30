namespace Merchello.Core.Marketing.Rewards
{
    using System;

    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// The reward base.
    /// </summary>
    /// <typeparam name="T">
    /// The type of reward to return
    /// </typeparam>
    public abstract class OfferRewardComponentBase<T> : OfferRewardComponentBase, IOfferRewardComponent<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OfferRewardComponentBase{T}"/> class. 
        /// </summary>
        /// <param name="definition">
        /// The <see cref="OfferComponentDefinition"/>.
        /// </param>
        protected OfferRewardComponentBase(OfferComponentDefinition definition)
            : base(definition)
        {
        }

        /// <summary>
        /// Gets the Type this component is responsible for building
        /// </summary>
        internal override Type BuilderFor
        {
            get
            {
                return typeof(T);
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
        public abstract Attempt<T> Award(ICustomerBase customer, ILineItemContainer container);

    }
}