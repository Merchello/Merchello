﻿namespace Merchello.Core.Models.Rdbms
{
    using NPoco;

    /// <summary>
    /// The table definition and "POCO" for database operations associated with the "merchShipmentStatus" table.
    /// </summary>
    [TableName("merchShipmentStatus")]
    [PrimaryKey("pk", AutoIncrement = false)]
    [ExplicitColumns]
    internal class ShipmentStatusDto : EntityDto
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [Column("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the alias.
        /// </summary>
        [Column("alias")]
        public string Alias { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether reportable.
        /// </summary>
        [Column("reportable")]
        public bool Reportable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether active.
        /// </summary>
        [Column("active")]
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets the sort order.
        /// </summary>
        [Column("sortOrder")]
        public int SortOrder { get; set; }
    }
}