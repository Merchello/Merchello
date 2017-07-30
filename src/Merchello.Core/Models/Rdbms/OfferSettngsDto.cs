namespace Merchello.Core.Models.Rdbms
{
    using System;

    using JetBrains.Annotations;

    /// <summary>
    /// The table definition and "POCO" for database operations associated with the "merchOfferSettings" table.
    /// </summary>
    internal class OfferSettingsDto : IEntityDto, IPageableDto
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the store key.
        /// </summary>
        public Guid StoreKey { get; set; }


        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the offer code.
        /// </summary>
        public string OfferCode { get; set; }

        /// <summary>
        /// Gets or sets the offer provider key.
        /// </summary>
        public Guid OfferProviderKey { get; set; }

        /// <summary>
        /// Gets or sets the offer starts date.
        /// </summary>
        [CanBeNull]
        public DateTime? OfferStartsDate { get; set; }

        /// <summary>
        /// Gets or sets the offer ends date.
        /// </summary>
        [CanBeNull]
        public DateTime? OfferEndsDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether active.
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets the configuration data.
        /// </summary>
        /// <remarks>
        /// This field stores JSON for constraints and reward fields
        /// </remarks>
        [CanBeNull]
        public string ConfigurationData { get; set; }

        /// <inheritdoc/>
        public DateTime UpdateDate { get; set; }

        /// <inheritdoc/>
        public DateTime CreateDate { get; set; }
    }
}