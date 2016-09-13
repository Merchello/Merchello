namespace Merchello.Core.Models.Rdbms
{
    using System;

    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// Represents a POCO object that represents an <see cref="IEntity"/>.
    /// </summary>
    internal interface IEntityDto : IKeyDto
    {
        /// <summary>
        /// Gets or sets the update date.
        /// </summary>
        DateTime UpdateDate { get; set; }

        /// <summary>
        /// Gets or sets the create date.
        /// </summary>
        DateTime CreateDate { get; set; }
    }
}