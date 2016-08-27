namespace Merchello.Web.Models.ContentEditing.Collections
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// The entity specification collection display.
    /// </summary>
    [DataContract(Name = "entityFilterGroupDisplay", Namespace = "")]
    public class EntityFilterGroupDisplay : EntityCollectionDisplay
    {
        /// <summary>
        /// Gets or sets the attribute collections.
        /// </summary>
        [DataMember(Name = "filters")]
        public IEnumerable<EntityCollectionDisplay> Filters { get; set; } 
    }
}