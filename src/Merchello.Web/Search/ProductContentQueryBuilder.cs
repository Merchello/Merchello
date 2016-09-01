namespace Merchello.Web.Search
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Models.Interfaces;
    using Merchello.Core.Persistence.Querying;
    using Merchello.Web.Models;
    using Merchello.Web.Models.Ui.Rendering;
    using Models.VirtualContent;

    /// <summary>
    /// The product content query builder.
    /// </summary>
    internal class ProductContentQueryBuilder : IProductContentQueryBuilder
    {
        /// <summary>
        /// The <see cref="MerchelloHelper"/>.
        /// </summary>
        private readonly ICachedProductQuery _cachedQuery;

        /// <summary>
        /// The collection keys.
        /// </summary>
        private readonly HashSet<Guid> _collectionKeys = new HashSet<Guid>();

        /// <summary>
        /// The search term.
        /// </summary>
        private string _searchTerm;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductContentQueryBuilder"/> class.
        /// </summary>
        /// <param name="cachedQuery">
        /// The <see cref="ICachedProductQuery"/>.
        /// </param>
        public ProductContentQueryBuilder(ICachedProductQuery cachedQuery)
        {
            Ensure.ParameterNotNull(cachedQuery, "The ICachedProductQuery was null");
            _cachedQuery = cachedQuery;
            Reset();
        }

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
        public FilterQueryClusivity FilterQueryClusivity { get; set; }

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
        /// Adds a <see cref="IProductCollection"/> parameter.
        /// </summary>
        /// <param name="collection">
        /// The collection.
        /// </param>
        public void AddConstraint(IProductCollection collection)
        {
            this.Add((IEntityProxy)collection);
        }

        /// <summary>
        /// Adds a collection of <see cref="IProductCollection"/> parameters.
        /// </summary>
        /// <param name="collections">
        /// The collections.
        /// </param>
        public void AddConstraint(IEnumerable<IProductCollection> collections)
        {
            this.Add((IEnumerable<IEntityProxy>)collections);
        }

        /// <summary>
        /// Adds a <see cref="IProductFilter"/> parameter.
        /// </summary>
        /// <param name="filter">
        /// The filter.
        /// </param>
        public void AddConstraint(IProductFilter filter)
        {
            this.Add((IEntityProxy)filter);
        }

        /// <summary>
        /// Adds a collection of <see cref="IProductFilter"/> parameters.
        /// </summary>
        /// <param name="filters">
        /// The filters.
        /// </param>
        public void AddConstraint(IEnumerable<IProductFilter> filters)
        {
            this.Add((IEnumerable<IEntityProxy>)filters);
        }

        /// <summary>
        /// Allows for directly adding <see cref="IEntityCollection"/> keys.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        public void AddCollectionKeyConstraint(Guid key)
        {
            EnsureAdd(key);
        }

        /// <summary>
        /// Allows for directly adding a collection of <see cref="IEntityCollection"/> keys.
        /// </summary>
        /// <param name="keys">
        /// The keys.
        /// </param>
        public void AddCollectionKeyConstraint(IEnumerable<Guid> keys)
        {
            Add(keys);
        }

        /// <summary>
        /// The reset.
        /// </summary>
        public void Reset()
        {
            this._searchTerm = string.Empty;
            this.Page = 1;
            this.ItemsPerPage = 10;
            this.SortBy = ProductSortField.Name;
            this.SortDirection = SortDirection.Ascending;
            this.FilterQueryClusivity = FilterQueryClusivity.ExistsInAllCollectionsAndFilters;
        }

        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <returns>
        /// The <see cref="PagedCollection"/>.
        /// </returns>
        public PagedCollection<IProductContent> Execute()
        {
            return Build().Execute();
        }


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

        /// <summary>
        /// Builds the <see cref="IProductContentQuery{IProductContent}"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="IProductContentQuery{IProductContent}"/>.
        /// </returns>
        private IProductContentQuery<IProductContent> Build()
        {
            var sortBy = SortBy.ToString().ToLowerInvariant();

            return new ProductContentQuery(_cachedQuery)
            {
                SearchTerm = _searchTerm,
                Page = Page,
                ItemsPerPage = ItemsPerPage,
                SortBy = sortBy,
                SortDirection = SortDirection,
                CollectionKeys = _collectionKeys.ToArray(),
                FilterQueryClusivity = FilterQueryClusivity
            };
        }
    }
}