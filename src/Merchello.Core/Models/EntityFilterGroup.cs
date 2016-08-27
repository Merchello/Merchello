namespace Merchello.Core.Models
{
    using Merchello.Core.Models.Interfaces;

    /// <summary>
    /// Represents a specified entity filter collection.
    /// </summary>
    internal sealed class EntityFilterGroup : EntityCollection, IEntityFilterGroup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityFilterGroup"/> class.
        /// </summary>
        /// <param name="collection">
        /// The collection.
        /// </param>
        public EntityFilterGroup(IEntityCollection collection)
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
            this.Filters = new EntityFilterCollection();
            this.ResetDirtyProperties();
        }


        /// <summary>
        /// Gets or sets the attribute collections.
        /// </summary>
        public EntityFilterCollection Filters { get; internal set; }
    }
}