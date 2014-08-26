namespace Merchello.Core.Models.Rdbms
{
    using System;

    /// <summary>
    /// The PageableDto interface.
    /// </summary>
    public interface IPageableDto
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        Guid Key { get; set; }  
    }
}