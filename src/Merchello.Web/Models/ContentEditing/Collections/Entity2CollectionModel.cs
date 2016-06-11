namespace Merchello.Web.Models.ContentEditing.Collections
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// The add entity to collection model.
    /// </summary>
    [DataContract(Name = "entity2CollectionModel", Namespace = "")]
    public class Entity2CollectionModel
    {
        /// <summary>
        /// Gets or sets the entity key.
        /// </summary>
        [DataMember(Name = "entityKey")]
        public Guid EntityKey { get; set; }

        /// <summary>
        /// Gets or sets the collection key.
        /// </summary>
        [DataMember(Name = "collectionKey")]
        public Guid CollectionKey { get; set; }
    }
}