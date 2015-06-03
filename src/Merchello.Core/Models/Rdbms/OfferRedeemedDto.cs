namespace Merchello.Core.Models.Rdbms
{
    using System;

    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.DatabaseAnnotations;

    /// <summary>
    /// The offer settings dto.
    /// </summary>
    [TableName("merchOfferSettings")]
    [PrimaryKey("pk", autoIncrement = false)]
    [ExplicitColumns]
    public class OfferRedeemedDto : IPageableDto
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        [Column("pk")]
        [PrimaryKeyColumn(AutoIncrement = false)]
        [Constraint(Default = "newid()")]
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [Column("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the offer code.
        /// </summary>
        [Column("offerCode")]
        public string OfferCode { get; set; }

        /// <summary>
        /// Gets or sets the offer provider key.
        /// </summary>
        [Column("offerProviderKey")]
        public Guid OfferProviderKey { get; set; }

        /// <summary>
        /// Gets or sets the redeemed date.
        /// </summary>
        [Column("redeemedDate")]
        public DateTime RedeemedDate { get; set; }

        /// <summary>
        /// Gets or sets the extended data serialization.
        /// </summary>
        [NullSetting(NullSetting = NullSettings.Null)]
        [SpecialDbType(SpecialDbTypes.NTEXT)]
        public string ExtendedData { get; set; }

        /// <summary>
        /// Gets or sets the update date.
        /// </summary>
        [Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// Gets or sets the create date.
        /// </summary>
        [Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; }         
    }
}