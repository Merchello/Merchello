namespace Merchello.Core.Models.Interfaces
{
    using System;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// The EntityCollection interface.
    /// </summary>
    public interface IEntityCollection : IHasEntityTypeField, IEntity
    {
        /// <summary>
        /// Gets or sets the parent key.
        /// </summary>
        [DataMember]
        Guid? ParentKey { get; set; }

        /// <summary>
        /// Gets the entity type.
        /// </summary>
        EntityType EntityType { get; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [DataMember]
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
        [DataMember]
        Guid ProviderKey { get; set; }
    }
}