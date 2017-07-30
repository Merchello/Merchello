namespace Merchello.Core.Models.Rdbms
{
    using System;

    using JetBrains.Annotations;

    /// <summary>
    /// The table definition and "POCO" for database operations associated with the "merchProductOption" table.
    /// </summary>
    internal class ProductOptionDto : IEntityDto
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the detached content type key.
        /// </summary>
        [CanBeNull]
        public Guid? DetachedContentTypeKey { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether required.
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this represents a shared option.
        /// </summary>
        public bool Shared { get; set; }

        /// <summary>
        /// Gets or sets the UI option.
        /// </summary>
        [CanBeNull]
        public string UiOption { get; set; }

        /// <inheritdoc/>
        public DateTime UpdateDate { get; set; }

        /// <inheritdoc/>
        public DateTime CreateDate { get; set; }
    }
}
