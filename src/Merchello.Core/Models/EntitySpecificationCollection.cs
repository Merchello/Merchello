namespace Merchello.Core.Models
{
    using System;
    using System.Collections.ObjectModel;

    using Merchello.Core.Models.Interfaces;

    /// <summary>
    /// The entity specification collection.
    /// </summary>
    internal sealed class EntitySpecificationCollection : EntityCollection, IEntitySpecificationCollection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntitySpecificationCollection"/> class.
        /// </summary>
        /// <param name="collection">
        /// The collection.
        /// </param>
        public EntitySpecificationCollection(IEntityCollection collection)
            : base(collection.EntityTfKey, collection.ProviderKey)
        {
            this.ParentKey = collection.ParentKey;
            this.Key = collection.Key;
            this.CreateDate = collection.CreateDate;
            this.UpdateDate = collection.UpdateDate;
            this.Name = collection.Name;
            this.AttributeCollections = new EntitySpecificationAttributeCollection();
            this.ResetDirtyProperties();
        }


        /// <summary>
        /// Gets or sets the attribute collections.
        /// </summary>
        public EntitySpecificationAttributeCollection AttributeCollections { get; internal set; }
    }
}