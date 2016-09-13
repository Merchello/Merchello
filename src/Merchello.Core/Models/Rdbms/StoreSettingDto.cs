namespace Merchello.Core.Models.Rdbms
{
    using System;

    using Merchello.Core.Acquired.Persistence.DatabaseAnnotations;

    using NPoco;

    /// <summary>
    /// The table definition and "POCO" for database operations associated with the "merchStoreSetting" table.
    /// </summary>
    [TableName("merchStoreSetting")]
    [PrimaryKey("pk", AutoIncrement = false)]
    [ExplicitColumns]
    internal class StoreSettingDto : EntityDto
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [Column("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        [Column("value")]
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the type name.
        /// </summary>
        [Column("typeName")]
        public string TypeName { get; set; }      
    }
}