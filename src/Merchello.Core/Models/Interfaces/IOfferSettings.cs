namespace Merchello.Core.Models.Interfaces
{
    using System;

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
        /// Gets a value indicating whether the offer has expired.
        /// </summary>
        bool Expired { get; }

        /// <summary>
        /// Gets a value indicating whether the offer has started.
        /// </summary>
        bool HasStarted { get; }

        /// <summary>
        /// Gets or sets the component configurations.
        /// </summary>
        OfferComponentDefinitionCollection ComponentDefinitions { get; set; }
    }
}