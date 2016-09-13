namespace Merchello.Core.Models.Rdbms
{
    using System;

    using NPoco;

    /// <summary>
    /// Define a DTO object is identified by a GUID primary key.
    /// </summary>
    internal interface IKeyDto : IDto
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        [Column("pk")]
        Guid Key { get; set; } 
    }
}