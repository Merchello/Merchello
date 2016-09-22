namespace Merchello.Core.Models.Rdbms
{
    using System;

    /// <inheritdoc/>
    public abstract class EntityDto : KeyDto, IEntityDto
    {
        /// <inheritdoc/>
        public DateTime UpdateDate { get; set; }

        /// <inheritdoc/>
        public DateTime CreateDate { get; set; }
    }
}