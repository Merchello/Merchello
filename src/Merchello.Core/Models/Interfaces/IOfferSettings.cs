namespace Merchello.Core.Models.Interfaces
{
    using System;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// The DiscountOfferSettings interface.
    /// </summary>
    public interface IOfferSettings : IEntity
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [DataMember]
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the offer code.
        /// </summary>
        /// <remarks>
        /// This must be unique
        /// </remarks>
        [DataMember]
        string OfferCode { get; set; }

        /// <summary>
        /// Gets or sets the provider key.
        /// </summary>
        [DataMember]
        Guid OfferProviderKey { get; set; }

        /// <summary>
        /// Gets or sets the offer starts date.
        /// </summary>
        [DataMember]
        DateTime OfferStartsDate { get; set; }

        /// <summary>
        /// Gets or sets the offer ends date.
        /// </summary>
        [DataMember]
        DateTime OfferEndsDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether active.
        /// </summary>
        [DataMember]
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
        [DataMember]
        OfferComponentDefinitionCollection ComponentDefinitions { get; set; }
    }
}