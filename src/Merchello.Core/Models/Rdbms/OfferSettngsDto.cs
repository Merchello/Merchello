namespace Merchello.Core.Models.Rdbms
{
    using System;

    using Merchello.Core.Acquired.Persistence.DatabaseAnnotations;

    using NPoco;

    /// <summary>
    /// The table definition and "POCO" for database operations associated with the "merchOfferSettings" table.
    /// </summary>
    [TableName("merchOfferSettings")]
    [PrimaryKey("pk", AutoIncrement = false)]
    [ExplicitColumns]
    internal class OfferSettingsDto : EntityDto, IPageableDto
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [Column("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the offer code.
        /// </summary>
        [Column("offerCode")]
        [IndexAttribute(IndexTypes.UniqueNonClustered, Name = "IX_merchOfferSettingsOfferCode")]
        public string OfferCode { get; set; }

        /// <summary>
        /// Gets or sets the offer provider key.
        /// </summary>
        [Column("offerProviderKey")]
        public Guid OfferProviderKey { get; set; }

        /// <summary>
        /// Gets or sets the offer starts date.
        /// </summary>
        [Column("offerStartsDate")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public DateTime? OfferStartsDate { get; set; }

        /// <summary>
        /// Gets or sets the offer ends date.
        /// </summary>
        [Column("offerEndsDate")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public DateTime? OfferEndsDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether active.
        /// </summary>
        [Column("active")]
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets the configuration data.
        /// </summary>
        /// <remarks>
        /// This field stores JSON for constraints and reward fields
        /// </remarks>
        [Column("configurationData")]
        [NullSetting(NullSetting = NullSettings.Null)]
        [SpecialDbType(SpecialDbTypes.NTEXT)]
        public string ConfigurationData { get; set; }
    }
}