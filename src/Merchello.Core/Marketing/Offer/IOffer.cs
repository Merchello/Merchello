namespace Merchello.Core.Marketing.Offer
{
    using System;

    /// <summary>
    /// Marker interface for an offer.
    /// </summary>
    public interface IOffer
    {
        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <remarks>
        /// This is actually a reference to the OfferSettings key
        /// </remarks>
        Guid Key { get; }

        /// <summary>
        /// Gets or sets the offer provider key.
        /// </summary>
        Guid OfferProviderKey { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the offer code.
        /// </summary>
        string OfferCode { get; set; }

        /// <summary>
        /// Gets or sets the offer starts date.
        /// </summary>
        DateTime OfferStartsDate { get; set; }

        /// <summary>
        /// Gets or sets the offer ends date.
        /// </summary>
        DateTime OfferEndsDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the offer is active.
        /// </summary>
        bool Active { get; set; }

        /// <summary>
        /// Adds an <see cref="OfferComponentBase"/> such as a constraint or a reward
        /// </summary>
        /// <param name="component">
        /// The <see cref="OfferComponentBase"/>
        /// </param>
        void AddComponent(OfferComponentBase component);

        /// <summary>
        /// Adds an offer component by it's component definition
        /// </summary>
        /// <param name="definition">
        /// The <see cref="OfferComponentDefinition"/>
        /// </param>
        void AddComponent(OfferComponentDefinition definition);
    }
}