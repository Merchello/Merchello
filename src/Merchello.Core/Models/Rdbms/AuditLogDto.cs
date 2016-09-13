namespace Merchello.Core.Models.Rdbms
{
    using System;

    using Merchello.Core.Acquired.Persistence.DatabaseAnnotations;

    using NPoco;

    /// <summary>
    /// The table definition and "POCO" for database operations associated with the "merchAuditLog" table.
    /// </summary>
    [TableName("merchAuditLog")]
    [PrimaryKey("pk", AutoIncrement = false)]
    [ExplicitColumns]
    internal class AuditLogDto : EntityDto, IPageableDto
    {
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
    }
}