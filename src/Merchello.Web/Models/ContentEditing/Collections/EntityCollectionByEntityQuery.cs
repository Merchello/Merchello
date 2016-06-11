namespace Merchello.Web.Models.ContentEditing.Collections
{
    using System;
    using System.Runtime.Serialization;

    using Merchello.Core;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// The entity collection by entity query.
    /// </summary>
    [DataContract(Name = "entityCollectionByEntityQuery", Namespace = "")]
    public class EntityCollectionByEntityQuery
    {
        /// <summary>
        /// Gets or sets the entity key.
        /// </summary>
        [DataMember(Name = "key")]
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the entity type.
        /// </summary>
        [DataMember(Name = "entityType")]
        [JsonConverter(typeof(StringEnumConverter))]
        public EntityType EntityType { get; set; }
    }
}