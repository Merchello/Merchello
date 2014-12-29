namespace Merchello.Core.Models.Rdbms
{
    using System;

    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.DatabaseAnnotations;

    /// <summary>
    /// The audit dto.
    /// </summary>
    [TableName("merchAuditLog")]
    [PrimaryKey("pk", autoIncrement = false)]
    [ExplicitColumns]
    internal class AuditLogDto : IPageableDto
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        [Column("pk")]
        [PrimaryKeyColumn(AutoIncrement = false)]
        [Constraint(Default = "newid()")]
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the entity key.
        /// </summary>
        [Column("entityKey")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public Guid? EntityKey { get; set; }

        /// <summary>
        /// Gets or sets the reference type.
        /// </summary>
        [Column("entityTfKey")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public Guid? EntityTfKey { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        [Column("message")]
        [NullSetting(NullSetting = NullSettings.Null)]
        [SpecialDbType(SpecialDbTypes.NTEXT)]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the verbosity level. 
        /// </summary>
        [Column("verbosity")]
        [Constraint(Default = "0")]
        public int Verbosity { get; set; }

        /// <summary>
        /// Gets or sets the extended data collection.
        /// </summary>
        [Column("extendedData")]
        [NullSetting(NullSetting = NullSettings.Null)]
        [SpecialDbType(SpecialDbTypes.NTEXT)]
        public string ExtendedData { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this is an error record.
        /// </summary>
        [Column("isError")]
        public bool IsError { get; set; }

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