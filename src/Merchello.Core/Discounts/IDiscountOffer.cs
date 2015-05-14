namespace Merchello.Core.Discounts
{
    using System;

    using Merchello.Core.Discounts.Rewards;
    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// Defines a discount offer.
    /// </summary>
    public interface IDiscountOffer
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the discount provider key.
        /// </summary>
        Guid DiscountProviderKey { get; set; }

        /// <summary>
        /// Gets or sets the offer starts date.
        /// </summary>
        DateTime OfferStartsDate { get; set; }

        /// <summary>
        /// Gets or sets the offer ends date.
        /// </summary>
        DateTime OfferEndsDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether active.
        /// </summary>
        bool Active { get; set; }

        /// <summary>
        /// The try apply.
        /// </summary>
        /// <param name="collection">
        /// The collection.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt"/>.
        /// </returns>
        Attempt<IDiscountReward> TryApply(ILineItemContainer collection);
    }
}