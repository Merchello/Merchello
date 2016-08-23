namespace Merchello.Core.Models.Rdbms
{
    using System;

    using Umbraco.Core.Persistence;

    /// <summary>
    /// A DTO object used for querying use counts of entities.
    /// </summary>
    internal class EntityUseCountDto
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        [Column("pk")]
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the use count.
        /// </summary>
        [Column("useCount")]
        public int UseCount { get; set; }
    }
}