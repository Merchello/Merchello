namespace Merchello.Core.Models
{
    using System;
    

    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// Represents an entity collection.
    /// </summary>
    public interface IEntityCollection : IHasParent, IHasEntityTypeField, IHasExtendedData, IEntity
    {
        /// <summary>
        /// Gets or sets the parent key.
        /// </summary>
        
        new Guid? ParentKey { get; set; }

        /// <summary>
        /// Gets the entity type.
        /// </summary>
        EntityType EntityType { get; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        
        string Name { get; set; }

        /// <summary>
        /// Gets the sort order.
        /// </summary>
        /// <remarks>
        /// Zero based sort order
        /// </remarks>
        int SortOrder { get; }

        /// <summary>
        /// Gets or sets the collection provider key.
        /// </summary>
        
        Guid ProviderKey { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is filter.
        /// </summary>
        
        bool IsFilter { get; set; }
    }
}