namespace Merchello.Core.Models.Rdbms
{
    using System;

    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// Defines a DTO object that represents a <see cref="IEntity"/>.
    /// </summary>
    public interface IEntityDto : IKeyDto
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