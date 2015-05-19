namespace Merchello.Core.Models.Interfaces
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Marketing.Rewards;
    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// The DiscountOfferSettings interface.
    /// </summary>
    public interface IOfferSettings : IEntity
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the offer code.
        /// </summary>
        /// <remarks>
        /// This must be unique
        /// </remarks>
        string OfferCode { get; set; }

        /// <summary>
        /// Gets or sets the provider key.
        /// </summary>
        Guid OfferProviderKey { get; set; }

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
        /// Gets or sets the component configurations.
        /// </summary>
        IEnumerable<OfferComponentConfiguration> ComponentConfigurations { get; set; }

        /// <summary>
        /// Gets the reward.
        /// </summary>
        IReward Reward { get; }
    }
}