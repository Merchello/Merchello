namespace Merchello.Web.Models.ContentEditing.Content
{
    using System;

    using Merchello.Core.Models.TypeFields;

    /// <summary>
    /// The detached content type display.
    /// </summary>
    public class DetachedContentTypeDisplay
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
        /// Gets or sets the <see cref="UmbContentTypeDisplay"/>.
        /// </summary>
        public UmbContentTypeDisplay UmbContentType { get; set; }

        /// <summary>
        /// Gets or sets the entity type field key.
        /// </summary>
        public Guid EntityTfKey { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="EntityTypeField"/>.
        /// </summary>
        public EntityTypeField EntityTypeField { get; set; }
    }
}