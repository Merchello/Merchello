namespace Merchello.Core.Models.Interfaces
{
    using System;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// The EntityCollection interface.
    /// </summary>
    public interface IEntityCollection : IEntity
    {
        /// <summary>
        /// Gets or sets the parent key.
        /// </summary>
        [DataMember]
        Guid? ParentKey { get; set; }

        /// <summary>
        /// Gets or sets the entity type field key.
        /// </summary>
        [DataMember]
        Guid EntityTfKey { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [DataMember]
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the collection provider key.
        /// </summary>
        [DataMember]
        Guid ProviderKey { get; set; }
    }
}