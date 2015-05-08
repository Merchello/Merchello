namespace Merchello.Core.Models.Rdbms
{
    using System;

    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.DatabaseAnnotations;

    /// <summary>
    /// A marketing campaign offer.
    /// </summary>
    [TableName("merchCampaignActivitySettings")]
    [PrimaryKey("pk", autoIncrement = false)]
    [ExplicitColumns]
    internal class CampaignActivitySettingsDto
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        [Column("pk")]
        [PrimaryKeyColumn(AutoIncrement = false)]
        [Constraint(Default = "newid()")]
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the customer key.
        /// </summary>
        [Column("campaignKey")]
        [ForeignKey(typeof(CampaignSettingsDto), Name = "FK_merchCampaignActivitySettings_merchCampaignSettings", Column = "pk")]
        [Index(IndexTypes.NonClustered, Name = "IX_merchCampgaignActivitySettings_campaignKey")]
        public Guid CampaignKey { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [Column("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the alias.
        /// </summary>
        /// <remarks>
        /// Used for easy JS referencing?
        /// </remarks>
        [Column("code")]
        [Index(IndexTypes.NonClustered, Name = "IX_merchCampgaignActivitySettings_code")]
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [Column("description")]
        [NullSetting(NullSetting = NullSettings.Null)]
        [SpecialDbType(SpecialDbTypes.NTEXT)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the offer type field key.
        /// </summary>
        [Column("campaignActivityTfKey")]
        public Guid CampaignActivityTfKey { get; set; }

        /// <summary>
        /// Gets or sets start date.
        /// </summary>
        [Column("startDate")]
        [Constraint(Default = "getdate()")]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        [Column("endDate")]
        [Constraint(Default = "getdate()")]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Gets or sets the extended data.
        /// </summary>
        [Column("extendedData")]
        [NullSetting(NullSetting = NullSettings.Null)]
        [SpecialDbType(SpecialDbTypes.NTEXT)]
        public string ExtendedData { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether active.
        /// </summary>
        [Column("active")]
        public bool Active { get; set; }

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