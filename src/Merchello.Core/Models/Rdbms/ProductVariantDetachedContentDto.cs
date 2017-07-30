namespace Merchello.Core.Models.Rdbms
{
    using System;

    using JetBrains.Annotations;

    /// <summary>
    /// The table definition and "POCO" for database operations associated with the "merchProductVariantDetachedContent" table.
    /// </summary>
    internal class ProductVariantDetachedContentDto : IEntityDto
    {
        /// <summary>
        /// Gets or sets the unique key.
        /// </summary>
        /// <remarks>
        /// This is weird situation where we want a Key so that we can use <see cref="IEntity"/> but the actual primary constraint needs to be 
        /// multiple keys.
        /// </remarks>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the product variant key.
        /// </summary>
        public Guid ProductVariantKey { get; set; }

        /// <summary>
        /// Gets or sets the culture name.
        /// </summary>
        public string CultureName { get; set; }

        /// <summary>
        /// Gets or sets the detached content type key.
        /// </summary>
        public Guid DetachedContentTypeKey { get; set; }

        /// <summary>
        /// Gets or sets the property values.
        /// </summary>
        /// <remarks>
        /// Introduced in version 1.12.0
        /// Used to store property value JSON similar to the Nested Content package
        /// </remarks>
        [CanBeNull]
        public string Values { get; set; }

        /// <summary>
        /// Gets or sets the template id.
        /// </summary>
        [CanBeNull]
        public int? TemplateId { get; set; }

        /// <summary>
        /// Gets or sets the slug.
        /// </summary>
        [CanBeNull]
        public string Slug { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether virtual content can be rendered.
        /// </summary>
        public bool CanBeRendered { get; set; }

        /// <inheritdoc/>
        public DateTime UpdateDate { get; set; }

        /// <inheritdoc/>
        public DateTime CreateDate { get; set; }
    }
}