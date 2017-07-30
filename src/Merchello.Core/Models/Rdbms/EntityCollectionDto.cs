namespace Merchello.Core.Models.Rdbms
{
    using System;

    using JetBrains.Annotations;

    /// <summary>
    /// The product collection dto.
    /// </summary>
    internal class EntityCollectionDto : IEntityDto
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the parent key.
        /// </summary>
        [CanBeNull]
        public Guid? ParentKey { get; set; }

        /// <summary>
        /// Gets or sets the entity type field key.
        /// </summary>
        public Guid EntityTfKey { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the sort order.
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// Gets or sets the key for the collection provider.
        /// </summary>
        public Guid ProviderKey { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is filter.
        /// </summary>
        public bool IsFilter { get; set; }

        /// <summary>
        /// Gets or sets the extended data.
        /// </summary>
        [CanBeNull]
        public string ExtendedData { get; set; }

        /// <inheritdoc/>
        public DateTime UpdateDate { get; set; }

        /// <inheritdoc/>
        public DateTime CreateDate { get; set; }
    }
}