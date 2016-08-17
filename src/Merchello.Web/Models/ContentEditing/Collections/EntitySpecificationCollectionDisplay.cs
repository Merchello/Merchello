namespace Merchello.Web.Models.ContentEditing.Collections
{
    using System.Collections.Generic;

    /// <summary>
    /// The entity specification collection display.
    /// </summary>
    public class EntitySpecificationCollectionDisplay : EntityCollectionDisplay
    {
        /// <summary>
        /// Gets or sets the attribute collections.
        /// </summary>
        public IEnumerable<EntityCollectionDisplay> AttributeCollections { get; set; } 
    }
}