namespace Merchello.Web.Models.ContentEditing.Collections
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Represents an entity collection provider.
    /// </summary>
    public class EntityCollectionProviderDisplay
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
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether manages unique collection.
        /// </summary>
        public bool ManagesUniqueCollection { get; set; }

        /// <summary>
        /// Gets or sets the entity type.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public EntityType EntityType { get; set; }

        /// <summary>
        /// Gets or sets the managed collections.
        /// </summary>
        public IEnumerable<EntityCollectionDisplay> ManagedCollections { get; set;   }
    }
}