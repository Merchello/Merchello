namespace Merchello.Core.Models.Interfaces
{
    using System;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// The EntityCollection interface.
    /// </summary>
    public interface IEntityCollection : IHasParent, IHasEntityTypeField, IHasExtendedData, IEntity
    {
        /// <summary>
        /// Gets or sets the parent key.
        /// </summary>
        [DataMember]
        new Guid? ParentKey { get; set; }

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

        /// <summary>
        /// Gets or sets a value indicating whether is filter.
        /// </summary>
        [DataMember]
        bool IsFilter { get; set; }
    }
}