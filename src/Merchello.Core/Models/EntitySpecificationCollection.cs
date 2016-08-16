namespace Merchello.Core.Models
{
    using System;
    using System.Collections.ObjectModel;

    using Merchello.Core.Models.Interfaces;

    /// <summary>
    /// The entity specification collection.
    /// </summary>
    internal class EntitySpecificationCollection : EntityCollection, IEntitySpecificationCollection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntitySpecificationCollection"/> class.
        /// </summary>
        /// <param name="entityTfKey">
        /// The entity type field key.
        /// </param>
        /// <param name="providerKey">
        /// The provider key.
        /// </param>
        public EntitySpecificationCollection(Guid entityTfKey, Guid providerKey)
            : base(entityTfKey, providerKey)
        {
        }

        /// <summary>
        /// Gets or sets the attribute collections.
        /// </summary>
        public EntitySpecificationAttributeCollection AttributeCollections { get; internal set; }
    }
}