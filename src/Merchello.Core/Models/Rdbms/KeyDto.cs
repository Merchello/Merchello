namespace Merchello.Core.Models.Rdbms
{
    using System;

    /// <summary>
    /// A POCO for Key queries and Key based table definitions.
    /// </summary>
    public class KeyDto : IKeyDto
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public Guid Key { get; set; }
    }
}
