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
            this.Initialize();
        }

        /// <summary>
        /// The reset.
        /// </summary>
        public override void Reset()
        {
            this._searchTerm = string.Empty;
            this.Page = 1;
            this.ItemsPerPage = 10;
            this.SortBy = ProductSortField.Name;
            this.SortDirection = SortDirection.Ascending;
            this.CollectionClusivity = CollectionClusivity.ExistsInAllCollectionsAndFilters;
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

            return new ProductContentQuery(_cachedQuery)
            {
                SearchTerm = _searchTerm,
                Page = Page,
                ItemsPerPage = ItemsPerPage,
                SortBy = sortBy,
                SortDirection = SortDirection,
                CollectionKeys = this.CollectionKeys,
                CollectionClusivity = this.CollectionClusivity
            };
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