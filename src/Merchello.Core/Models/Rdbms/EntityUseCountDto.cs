namespace Merchello.Core.Models.Rdbms
{
    using System;

    using NPoco;

    /// <summary>
    /// A DTO object used for querying use counts of entities.
    /// </summary>
    internal class EntityUseCountDto : KeyDto
    {
        /// <summary>
        /// Gets or sets the use count.
        /// </summary>
        [Column("useCount")]
        public int UseCount { get; set; }
    }
}