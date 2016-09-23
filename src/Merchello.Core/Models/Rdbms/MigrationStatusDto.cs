namespace Merchello.Core.Models.Rdbms
{
    using System;

    using Merchello.Core.Acquired.Persistence.DatabaseAnnotations;

    using NPoco;

    /// <summary>
    /// The table definition for the migration status DTO.
    /// </summary>
    [TableName("merchMigrationStatus")]
    [PrimaryKey("pk", AutoIncrement = false)]
    [ExplicitColumns]
    internal sealed class MigrationStatusDto : EntityDto
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        [PrimaryKeyColumn(AutoIncrement = false)]
        [Column("pk")]
        [Constraint(Default = "newid()")]
        public override Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [Column("name")]
        [Length(255)]
        [Index(IndexTypes.UniqueNonClustered, ForColumns = "name,version", Name = "IX_merchelloMigrationStatus")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        [Column("version")]
        [Length(50)]
        public string Version { get; set; }
    }
}