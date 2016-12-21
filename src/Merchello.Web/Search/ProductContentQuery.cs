namespace Merchello.Web.Search
{
    using System;
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Logging;
    using Merchello.Core.Persistence.Querying;
    using Merchello.Web.Models;
    using Merchello.Web.Models.VirtualContent;

    /// <summary>
    /// Represents a product filter query.
    /// </summary>
    internal class ProductContentQuery : IProductContentQuery
    {
        /// <summary>
        /// The <see cref="ICachedProductQuery"/>.
        /// </summary>
        private readonly ICachedProductQuery _query;

        /// <summary>
        /// The minimum price in the price range.
        /// </summary>
        private decimal _minPrice = 0M;

        /// <summary>
        /// The maximum price in the price range.
        /// </summary>
        private decimal _maxPrice = 0M;

        /// <summary>
        /// A value indicating that the query should consider price ranges.
        /// </summary>
        private bool _hasRange = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductContentQuery"/> class.
        /// </summary>
        /// <param name="query">
        /// The <see cref="ICachedProductQuery"/>.
        /// </param>
        public ProductContentQuery(ICachedProductQuery query)
        {
            Ensure.ParameterNotNull(query, "The CachedProductQuery cannot be null");
            _query = query;
        }

        /// <inheritdoc/>
        public decimal MinPrice
        {
            get
            {
                return _minPrice;
            }
        }

        /// <inheritdoc/>
        public decimal MaxPrice
        {
            get
            {
                return _maxPrice;
            }
        }

        /// <inheritdoc/>
        public bool HasPriceRange
        {
            get
            {
                return _hasRange;
            }
        }

        /// <summary>
        /// Gets a value indicating whether has the query has a search term.
        /// </summary>
        public bool HasSearchTerm
        {
            get
            {
                return !this.SearchTerm.IsNullOrWhiteSpace();
            }
        }

        /// <summary>
        /// Gets or sets the search term.
        /// </summary>
        public string SearchTerm { get; set; }

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
        public string SortBy { get; set; }

        /// <summary>
        /// Gets or sets the sort direction.
        /// </summary>
        public SortDirection SortDirection { get; set; }

        /// <summary>
        /// Gets or sets the collection keys.
        /// </summary>
        public Guid[] CollectionKeys { get; set; }

        /// <summary>
        /// Gets or sets a setting for specifying how the query should treat collection clusivity in specified collections and filters.
        /// </summary>
        public CollectionClusivity CollectionClusivity { get; set; }

        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <returns>
        /// The <see cref="PagedCollection"/>.
        /// </returns>
        public PagedCollection<IProductContent> Execute()
        {
            var hasCollections = CollectionKeys != null && CollectionKeys.Any();


            if (!hasCollections && !HasPriceRange)
            {
                return HasSearchTerm
                  ? _query.TypedProductContentSearchPaged(SearchTerm, Page, ItemsPerPage, SortBy, SortDirection)
                  : _query.TypedProductContentSearchPaged(Page, ItemsPerPage, SortBy, SortDirection);
            }

            if (!hasCollections && HasPriceRange)
            {
                if (!HasSearchTerm)
                {
                    return _query.TypedProductContentByPriceRange(MinPrice, MaxPrice, Page, ItemsPerPage, SortBy, SortDirection);
                }
                
                // don't have a price range with a search term filter.
                var notImplemented = new NotImplementedException();
                MultiLogHelper.Error<ProductContentQuery>("Typed product content by search term and price range has not been implemented", notImplemented);
                throw notImplemented;
            }
            

            switch (this.CollectionClusivity)
            {
                case CollectionClusivity.DoesNotExistInAnyCollectionsAndFilters:

                    if (!HasPriceRange)
                    {
                        return HasSearchTerm ?
                        _query.TypedProductContentPageThatNotInAnyCollections(CollectionKeys, SearchTerm, Page, ItemsPerPage, SortBy, SortDirection) :
                        _query.TypedProductContentPageThatNotInAnyCollections(CollectionKeys, Page, ItemsPerPage, SortBy, SortDirection);
                    }

                    return HasSearchTerm ?
                    _query.TypedProductContentPageThatNotInAnyCollections(CollectionKeys, SearchTerm, MinPrice, MaxPrice, Page, ItemsPerPage, SortBy, SortDirection) :
                    _query.TypedProductContentPageThatNotInAnyCollections(CollectionKeys, MinPrice, MaxPrice, Page, ItemsPerPage, SortBy, SortDirection);

                case CollectionClusivity.ExistsInAnyCollectionOrFilter:

                    if (!HasPriceRange)
                    {
                        return HasSearchTerm ?
                            _query.TypedProductContentPageThatExistsInAnyCollections(CollectionKeys, SearchTerm, Page, ItemsPerPage, SortBy, SortDirection) :
                            _query.TypedProductContentPageThatExistsInAnyCollections(CollectionKeys, Page, ItemsPerPage, SortBy, SortDirection);
                    }

                    return HasSearchTerm ?
                        _query.TypedProductContentPageThatExistsInAnyCollections(CollectionKeys, SearchTerm, MinPrice, MaxPrice, Page, ItemsPerPage, SortBy, SortDirection) :
                        _query.TypedProductContentPageThatExistsInAnyCollections(CollectionKeys, MinPrice, MaxPrice, Page, ItemsPerPage, SortBy, SortDirection);

                case CollectionClusivity.ExistsInAllCollectionsAndFilters:
                default:

                    if (!HasPriceRange)
                    {
                        return HasSearchTerm ?
                            _query.TypedProductContentPageThatExistInAllCollections(CollectionKeys, SearchTerm, Page, ItemsPerPage, SortBy, SortDirection) :
                            _query.TypedProductContentPageThatExistInAllCollections(CollectionKeys, Page, ItemsPerPage, SortBy, SortDirection);
                    }

                    return HasSearchTerm ?
                        _query.TypedProductContentPageThatExistInAllCollections(CollectionKeys, SearchTerm, MinPrice, MaxPrice, Page, ItemsPerPage, SortBy, SortDirection) :
                        _query.TypedProductContentPageThatExistInAllCollections(CollectionKeys, MinPrice, MaxPrice, Page, ItemsPerPage, SortBy, SortDirection);
            }
        }


        /// <inheritdoc/>
        public void SetPriceRange(decimal min, decimal max)
        {
            if (min > max)
            {
                var tmp = min;
                max = min;
                min = tmp;
            }

            _minPrice = min;
            _maxPrice = max;
            _hasRange = true;
        }

        /// <inheritdoc/>
        public void ClearPriceRange()
        {
            _minPrice = 0M;
            _maxPrice = 0M;
            _hasRange = false;
        }
    }
}