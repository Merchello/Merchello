namespace Merchello.Core.Models
{
    using System;
    using System.Collections.ObjectModel;

    using Merchello.Core.Models.Interfaces;

    /// <summary>
    /// Represents a specified entity filter collection.
    /// </summary>
    internal sealed class EntitySpecifiedFilterCollection : EntityCollection, IEntitySpecifiedFilterCollection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntitySpecifiedFilterCollection"/> class.
        /// </summary>
        /// <param name="collection">
        /// The collection.
        /// </param>
        public EntitySpecifiedFilterCollection(IEntityCollection collection)
            : base(collection.EntityTfKey, collection.ProviderKey)
        {
            this.ParentKey = collection.ParentKey;
            this.Key = collection.Key;
            this.CreateDate = collection.CreateDate;
            this.UpdateDate = collection.UpdateDate;
            this.Name = collection.Name;
            this.IsFilter = collection.IsFilter;
            this.ExtendedData = collection.ExtendedData;
            this.SortOrder = collection.SortOrder;
            this.AttributeCollections = new SpecifiedFilterAttributeCollection();
            this.ResetDirtyProperties();
        }


        /// <summary>
        /// Gets or sets the attribute collections.
        /// </summary>
        public SpecifiedFilterAttributeCollection AttributeCollections { get; internal set; }
    }
}