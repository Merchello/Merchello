namespace Merchello.Web.Models.ContentEditing.Collections
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// The entity specification collection display.
    /// </summary>
    [DataContract(Name = "entitySpecificationCollectionDisplay", Namespace = "")]
    public class EntitySpecificationCollectionDisplay : EntityCollectionDisplay
    {
        /// <summary>
        /// Gets or sets the attribute collections.
        /// </summary>
        [DataMember(Name = "attributeCollections")]
        public IEnumerable<EntityCollectionDisplay> AttributeCollections { get; set; } 
    }
}