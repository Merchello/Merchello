namespace Merchello.Core.Models.Rdbms
{
    using Merchello.Core.Acquired.Persistence.DatabaseAnnotations;

    using NPoco;

    /// <summary>
    /// The table definition and "POCO" for the merchelloLock table.
    /// </summary>
    [TableName("merchLock")]
    [PrimaryKey("id")]
    [ExplicitColumns]
    internal sealed class LockDto
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [Column("id")]
        [PrimaryKeyColumn(Name = "PK_merchelloLock")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        [Column("value")]
        [NullSetting(NullSetting = NullSettings.NotNull)]
        public int Value { get; set; } = 1;

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [Column("name")]
        [NullSetting(NullSetting = NullSettings.NotNull)]
        [Length(64)]
        public string Name { get; set; }
    }
}