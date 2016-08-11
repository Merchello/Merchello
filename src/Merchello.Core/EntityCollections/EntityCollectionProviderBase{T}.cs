namespace Merchello.Core.EntityCollections
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

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
        where T : class, IEntity
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
        /// The <see cref="IEnumerable{Object}"/>.
        /// </returns>
        public override IEnumerable<object> GetEntities()
        {
            return this.PerformGetPagedEntities(1, long.MaxValue).Items;
        }

        /// <summary>
        /// Returns true if the entity exists in the collection.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public virtual bool Exists(T entity)
        {
            var cacheKey = GetExistsCacheKey(entity);
            var exists = Cache.GetCacheItem(cacheKey);

            if (exists != null) return (bool)exists;


            return (bool)Cache.GetCacheItem(cacheKey, () => this.PerformExists(entity));
        }

        /// <summary>
        /// Gets a generic page of entities.
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
        /// The <see cref="Page{Object}"/>.
        /// </returns>
        public override Page<object> GetPagedEntities(long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Ascending)
        {
            var p = this.PerformGetPagedEntities(page, itemsPerPage, sortBy, sortDirection);

            return new Page<object>()
                {
                    Context = p.Context,
                    CurrentPage = p.CurrentPage,
                    ItemsPerPage = p.ItemsPerPage,
                    TotalItems = p.TotalItems,
                    TotalPages = p.TotalPages,
                    Items = p.Items.Select(x => x as object).ToList()
                };
        }

        /// <summary>
        /// Returns true if the entity exists in the collection.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        protected abstract bool PerformExists(T entity);

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
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Ascending);

        /// <summary>
        /// Gets the request cache key for the exists query.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetExistsCacheKey(T entity)
        {
            return string.Format("{0}.{1}.exists", typeof(T), entity.Key);
        }
    }
}