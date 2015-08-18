namespace Merchello.Web.Models.ContentEditing.Collections
{
    using System;

    using Merchello.Core;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// The entity collection by entity query.
    /// </summary>
    public class EntityCollectionByEntityQuery
    {
        /// <summary>
        /// Gets or sets the entity key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the entity type.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public EntityType EntityType { get; set; }
    }
}