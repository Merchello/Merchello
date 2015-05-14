namespace Merchello.Core.Discounts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Discounts.Rewards;
    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// A base class for DiscountOffer classes
    /// </summary>
    public abstract class DiscountOfferBase : IDiscountOffer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DiscountOfferBase"/> class.
        /// </summary>
        /// <param name="reward">
        /// The reward.
        /// </param>
        /// <param name="constraints">
        /// The constraints.
        /// </param>
        protected DiscountOfferBase(IDiscountReward reward, IEnumerable<IDiscountConstraint> constraints = null)
        {
            Mandate.ParameterNotNull(reward, "reward");

            this.Constraints = constraints ?? Enumerable.Empty<IDiscountConstraint>();
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the discount provider key.
        /// </summary>
        public Guid DiscountProviderKey { get; set; }

        /// <summary>
        /// Gets or sets the offer starts date.
        /// </summary>
        public DateTime OfferStartsDate { get; set; }

        /// <summary>
        /// Gets or sets the offer ends date.
        /// </summary>
        public DateTime OfferEndsDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether active.
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Gets the collection of discount constraints.
        /// </summary>
        protected IEnumerable<IDiscountConstraint> Constraints { get; private set; }

        protected abstract Attempt<IDiscountReward> ApplyReward(ILineItemContainer collection);

        public Attempt<IDiscountReward> TryApply(ILineItemContainer collection)
        {
            var applied = this.ApplyReward(collection);

            // TODO record this discount application for reporting
            throw new NotImplementedException();
        }
    }
}