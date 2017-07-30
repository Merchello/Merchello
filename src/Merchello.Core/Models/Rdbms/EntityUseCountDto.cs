namespace Merchello.Core.Models.Rdbms
{
    using System;

    /// <summary>
    /// A DTO object used for querying use counts of entities.
    /// </summary>
    internal class EntityUseCountDto : IKeyDto
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the use count.
        /// </summary>
        public int UseCount { get; set; }
    }
}