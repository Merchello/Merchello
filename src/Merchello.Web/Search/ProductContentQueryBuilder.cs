namespace Merchello.Web.Search
{
    using Merchello.Core;
    using Merchello.Core.Persistence.Querying;
    using Merchello.Web.Models;
    using Merchello.Web.Models.Ui.Rendering;
    using Models.VirtualContent;

    /// <summary>
    /// The product content query builder.
    /// </summary>
    internal class ProductContentQueryBuilder : CmsContentQueryBuilderBase<IProductCollection, IProductFilter, IProductContent>, IProductContentQueryBuilder
    {
        /// <summary>
        /// The <see cref="MerchelloHelper"/>.
        /// </summary>
        private readonly ICachedProductQuery _cachedQuery;

        /// <summary>
        /// The minimum price range.
        /// </summary>
        private decimal _minPrice = 0M;

        /// <summary>
        /// The maximum price range.
        /// </summary>
        private decimal _maxPrice = 0M;

        /// <summary>
        /// A value indicating the builder should setup a price range filter..
        /// </summary>
        private bool _hasPriceRangeFilter = false;

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
            this.Initialize();
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
            _hasPriceRangeFilter = true;
        }

        /// <inheritdoc/>
        public void ClearPriceRange()
        {
            _hasPriceRangeFilter = false;
            _minPrice = 0M;
            _maxPrice = 0M;
        }

        /// <summary>
        /// The reset.
        /// </summary>
        public override void Reset()
        {
            this.SearchTerm = string.Empty;
            this.Page = 1;
            this.ItemsPerPage = 10;
            this.SortBy = ProductSortField.Name;
            this.SortDirection = SortDirection.Ascending;
            this.CollectionClusivity = CollectionClusivity.ExistsInAllCollectionsAndFilters;
            this.ClearPriceRange();
        }

        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <returns>
        /// The <see cref="PagedCollection"/>.
        /// </returns>
        public override PagedCollection<IProductContent> Execute()
        {
            return Build().Execute();
        }

        /// <summary>
        /// Builds the <see cref="ICmsContentQuery{TResult}"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="ICmsContentQuery{TResult}"/>.
        /// </returns>
        protected override ICmsContentQuery<IProductContent> Build()
        {
            var sortBy = SortBy.ToString().ToLowerInvariant();

            var query = new ProductContentQuery(_cachedQuery)
            {
                Page = Page,
                ItemsPerPage = ItemsPerPage,
                SortBy = sortBy,
                SortDirection = SortDirection,
                CollectionKeys = this.CollectionKeys,
                CollectionClusivity = this.CollectionClusivity
            };

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                query.SearchTerm = SearchTerm;
            }

            if (_hasPriceRangeFilter)
            {
                query.SetPriceRange(_minPrice, _maxPrice);
            }

            return query;
        }


        /// <summary>
        /// Initializes the builder.
        /// </summary>
        private void Initialize()
        {
            Reset();
        }
    }
}