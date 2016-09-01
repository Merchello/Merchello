namespace Merchello.Web.Search
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Persistence.Querying;
    using Merchello.Web.Models;
    using Merchello.Web.Models.Ui.Rendering;
    using Merchello.Web.Models.VirtualContent;

    /// <summary>
    /// A base class for creating CachedQueryBuilders.
    /// </summary>
    /// <typeparam name="TCollectionProxy">
    /// The type of the collection proxy
    /// </typeparam>
    /// <typeparam name="TFilterProxy">
    /// The type of the filter proxy
    /// </typeparam>
    /// <typeparam name="TResult">
    /// The type of the paged collection result
    /// </typeparam>
    public abstract class CmsContentQueryBuilderBase<TCollectionProxy, TFilterProxy, TResult> : ICmsContentQueryBuilder<TCollectionProxy, TFilterProxy, TResult>
        where TCollectionProxy : IEntityProxy
        where TFilterProxy : IEntityProxy
        where TResult : ICmsContent
    {

        /// <summary>
        /// The collection keys.
        /// </summary>
        private readonly HashSet<Guid> _collectionKeys = new HashSet<Guid>();

        /// <summary>
        /// The search term.
        /// </summary>
        private string _searchTerm;

        /// <summary>
        /// Gets or sets the page.
        /// </summary>
        public long Page { get; set; }

        /// <summary>
        /// Gets or sets the items per page.
        /// </summary>
        public long ItemsPerPage { get; set; }

        /// <summary>
        /// Gets or sets the sort by field.
        /// </summary>
        public ProductSortField SortBy { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="SortDirection"/>.
        /// </summary>
        public SortDirection SortDirection { get; set; }

        /// <summary>
        /// Gets or sets a setting for specifying how the query should treat collection clusivity in specified collections and filters.
        /// </summary>
        public CollectionClusivity CollectionClusivity { get; set; }

        /// <summary>
        /// Gets the collection keys.
        /// </summary>
        protected virtual Guid[] CollectionKeys
        {
            get
            {
                return _collectionKeys.ToArray();
            }
        }

        /// <summary>
        /// Adds a search term parameter.
        /// </summary>
        /// <param name="searchTerm">
        /// The search term.
        /// </param>
        public void AddConstraint(string searchTerm)
        {
            _searchTerm = searchTerm;
        }

        /// <summary>
        /// Adds the <see cref="IEntityProxy"/> key to the collections hash.
        /// </summary>
        /// <param name="collection">
        /// The <see cref="IEntityProxy"/>.
        /// </param>
        public void AddConstraint(TCollectionProxy collection)
        {
            Add(collection);
        }

        /// <summary>
        /// Adds the <see cref="IEntityProxy"/> key to the collections hash.
        /// </summary>
        /// <param name="collections">
        /// The <see cref="IEntityProxy"/>.
        /// </param>
        public void AddConstraint(IEnumerable<TCollectionProxy> collections)
        {
            Add((IEnumerable<IEntityProxy>)collections);
        }

        /// <summary>
        /// Adds the <see cref="IEntityProxy"/> key to the collections hash.
        /// </summary>
        /// <param name="filter">
        /// The <see cref="IEntityProxy"/>.
        /// </param>
        public void AddConstraint(TFilterProxy filter)
        {
            Add(filter);
        }

        /// <summary>
        /// Adds the <see cref="IEntityProxy"/> key to the collections hash.
        /// </summary>
        /// <param name="filters">
        /// The <see cref="IEntityProxy"/>.
        /// </param>
        public void AddConstraint(IEnumerable<TFilterProxy> filters)
        {
            Add((IEnumerable<IEntityProxy>)filters);
        }

        /// <summary>
        /// Adds a collection of keys to the collection hash.
        /// </summary>
        /// <param name="key">
        /// The keys.
        /// </param>
        public void AddCollectionKeyConstraint(Guid key)
        {
            EnsureAdd(key);
        }

        /// <summary>
        /// Adds a collection of keys to the collection hash.
        /// </summary>
        /// <param name="keys">
        /// The keys.
        /// </param>
        public void AddCollectionKeyConstraint(IEnumerable<Guid> keys)
        {
            Add(keys);
        }


        /// <summary>
        /// Resets the query build to default values
        /// </summary>
        public abstract void Reset();

        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <returns>
        /// The <see cref="PagedCollection{TResult}"/>.
        /// </returns>
        public abstract PagedCollection<TResult> Execute();

        /// <summary>
        /// Builds the <see cref="ICmsContentQuery{TResult}"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="ICmsContentQuery{TResult}"/>.
        /// </returns>
        protected abstract ICmsContentQuery<TResult> Build();

        /// <summary>
        /// Adds the <see cref="IEntityProxy"/> key to the collections hash.
        /// </summary>
        /// <param name="entity">
        /// The <see cref="IEntityProxy"/>.
        /// </param>
        private void Add(IEntityProxy entity)
        {
            EnsureAdd(entity.Key);
        }

        /// <summary>
        /// Adds the collection <see cref="IEntityProxy"/> key to the collections hash.
        /// </summary>
        /// <param name="entities">
        /// The <see cref="IEntityProxy"/>.
        /// </param>
        private void Add(IEnumerable<IEntityProxy> entities)
        {
            Add(entities.Select(x => x.Key));
        }

        /// <summary>
        /// Adds a collection of keys to the collection hash.
        /// </summary>
        /// <param name="keys">
        /// The keys.
        /// </param>
        private void Add(IEnumerable<Guid> keys)
        {
            foreach (var key in keys)
            {
                EnsureAdd(key);
            }
        }

        /// <summary>
        /// Ensures the hash set does not contain a key before it's added.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        private void EnsureAdd(Guid key)
        {
            if (!_collectionKeys.Contains(key)) _collectionKeys.Add(key);
        }
    }
}