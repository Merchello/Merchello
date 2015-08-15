namespace Merchello.Web.Models.ContentEditing.Collections
{
    using System;

    /// <summary>
    /// The add entity to collection model.
    /// </summary>
    public class Entity2CollectionModel
    {
        /// <summary>
        /// Gets or sets the entity key.
        /// </summary>
        public Guid EntityKey { get; set; }

        /// <summary>
        /// Gets or sets the collection key.
        /// </summary>
        public Guid CollectionKey { get; set; }
    }
}