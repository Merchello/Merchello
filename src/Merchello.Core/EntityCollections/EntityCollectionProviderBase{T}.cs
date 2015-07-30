namespace Merchello.Core.EntityCollections
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Models.EntityBase;
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
        /// The get entities.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        public IEnumerable<T> GetEntities()
        {
            return this.PerformGetEntities();
        }

        /// <summary>
        /// The get paged entities.
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
        public Page<T> GetPagedEntities(long page, long itemsPerPage, string sortBy = "name", SortDirection sortDirection = SortDirection.Ascending)
        {
            return this.PerformGetPagedEntities(page, itemsPerPage, sortBy, sortDirection);
        }

        /// <summary>
        /// The get entities.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        protected abstract IEnumerable<T> PerformGetEntities();

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
        protected abstract Page<T> PerformGetPagedEntities(
            long page,
            long itemsPerPage,
            string sortBy = "name",
            SortDirection sortDirection = SortDirection.Ascending);        
    }
}