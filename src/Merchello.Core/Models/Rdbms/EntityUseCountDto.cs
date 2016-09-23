namespace Merchello.Core.Models.Rdbms
{
    using System;

    using Merchello.Core.Acquired.Persistence.DatabaseAnnotations;

    using NPoco;

    /// <summary>
    /// A DTO object used for querying use counts of entities.
    /// </summary>
    internal class EntityUseCountDto : IKeyDto
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        [PrimaryKeyColumn(AutoIncrement = false)]
        [Constraint(Default = "newid()")]
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the use count.
        /// </summary>
        [Column("useCount")]
        public int UseCount { get; set; }
    }
}