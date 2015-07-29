namespace Merchello.Core.EntityCollections
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Models.Interfaces;
    using Merchello.Core.Models.TypeFields;
    using Merchello.Core.Persistence.Querying;

    using Umbraco.Core.Persistence;

    /// <summary>
    /// The entity collection provider base.
    /// </summary>
    /// <typeparam name="T">
    /// The type of entity
    /// </typeparam>
    public abstract class EntityCollectionProviderBase<T> : EntityCollectionProviderBase, IEntityCollectionProvider<T>
        where T : IEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityCollectionProviderBase{T}"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        protected EntityCollectionProviderBase(IMerchelloContext merchelloContext, Guid collectionKey)
            : base(merchelloContext, collectionKey)
        {
        }

        /// <summary>
        /// Gets the provider key.
        /// </summary>
        public abstract Guid ProviderKey { get; }

        /// <summary>
        /// Gets the entity type.
        /// </summary>
        public abstract ITypeField EntityType { get; }

        /// <summary>
        /// Gets a value indicating whether this provider manages a single (unique) <see cref="IEntityCollection"/>.
        /// </summary>
        /// <remarks>
        /// If true, the boot manager will automatically add the collection to the merchEntityCollection table if it does not exist.
        /// Likewise, if the provider is removed, it will remove itself from the merchEntityCollection table
        /// </remarks>
        public virtual bool ManagesUniqueCollection
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// The get entities.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        public abstract IEnumerable<T> GetEntities();

        /// <summary>
        /// The get entities.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{T}"/>.
        /// </returns>
        public abstract Page<T> GetEntities(
            long page,
            long itemsPerPage,
            string sortBy = "name",
            SortDirection sortDirection = SortDirection.Ascending);

    }
}