namespace Merchello.Core.Models.Rdbms
{
    using System;

    using Umbraco.Core.Persistence;

    /// <summary>
    /// The key dto.
    /// </summary>
    public class KeyDto
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        [Column("pk")]
        public Guid Key { get; set; }
    }
}
