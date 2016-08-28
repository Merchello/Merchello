namespace Merchello.Core.Models.Rdbms
{
    using System;

    /// <summary>
    /// Define a DTO object is identified by a GUID primary key.
    /// </summary>
    public interface IKeyDto
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        Guid Key { get; set; } 
    }
}