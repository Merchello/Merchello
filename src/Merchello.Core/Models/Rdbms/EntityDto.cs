namespace Merchello.Core.Models.Rdbms
{
    using System;

    /// <inheritdoc/>
    internal abstract class EntityDto : KeyDto, IEntityDto
    {
        /// <inheritdoc/>
        public DateTime UpdateDate { get; set; }

        /// <inheritdoc/>
        public DateTime CreateDate { get; set; }
    }
}