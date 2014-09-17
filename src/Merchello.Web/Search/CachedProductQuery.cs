namespace Merchello.Web.Search
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Core;
    using Core.Models;
    using Core.Persistence.Querying;
    using Core.Services;
    using Examine;
    using global::Examine;
    using global::Examine.Providers;

    using log4net.Util;

    using Models.ContentEditing;
    using Models.Querying;

    /// <summary>
    /// Represents a CachedProductQuery
    /// </summary>
    internal class CachedProductQuery : CachedQueryBase<IProduct, ProductDisplay>, ICachedProductQuery
    {
        /// <summary>
        /// The product service.
        /// </summary>
        private readonly ProductService _productService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedProductQuery"/> class.
        /// </summary>
        public CachedProductQuery()
            : this(MerchelloContext.Current.Services.ProductService)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedProductQuery"/> class.
        /// </summary>
        /// <param name="productService">
        /// The product service.
        /// </param>
        public CachedProductQuery(IProductService productService)
            : this(
            productService,
            ExamineManager.Instance.IndexProviderCollection["MerchelloProductIndexer"],
            ExamineManager.Instance.SearchProviderCollection["MerchelloProductSearcher"])
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedProductQuery"/> class.
        /// </summary>
        /// <param name="service">
        /// The service.
        /// </param>
        /// <param name="indexProvider">
        /// The index provider.
        /// </param>
        /// <param name="searchProvider">
        /// The search provider.
        /// </param>
        public CachedProductQuery(IPageCachedService<IProduct> service, BaseIndexProvider indexProvider, BaseSearchProvider searchProvider) 
            : base(service, indexProvider, searchProvider)
        {
            _productService = (ProductService)service;
        }

        /// <summary>
        /// Gets the key field in index.
        /// </summary>
        protected override string KeyFieldInIndex
        {
            get { return "productKey"; }
        }

        /// <summary>
        /// Gets a <see cref="ProductDisplay"/> by it's unique key
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="ProductDisplay"/>.
        /// </returns>
        public override ProductDisplay GetByKey(Guid key)
        {
            return GetDisplayObject(key);
        }

        /// <summary>
        /// Gets a <see cref="ProductVariantDisplay"/> by it's key
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="ProductVariantDisplay"/>.
        /// </returns>
        public ProductVariantDisplay GetProductVariantByKey(Guid key)
        {
            var criteria = SearchProvider.CreateSearchCriteria();
            criteria.Field("productVariantKey", key.ToString());

            var result = CachedSearch(criteria, ExamineDisplayExtensions.ToProductVariantDisplay).FirstOrDefault();

            if (result != null) return result;

            var variant = _productService.GetProductVariantByKey(key);

            if (variant != null) this.ReindexEntity(variant);

            return variant.ToProductVariantDisplay();
        }


        /// <summary>
        /// Searches all products
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
        /// The <see cref="QueryResultDisplay"/>.
        /// </returns>
        public QueryResultDisplay Search(long page, long itemsPerPage, string sortBy = "name", SortDirection sortDirection = SortDirection.Descending)
        {
            return GetQueryResultDisplay(_productService.GetPagedKeys(page, itemsPerPage, sortBy, sortDirection));
        }

        /// <summary>
        /// Searches all products for a term
        /// </summary>
        /// <param name="term">
        /// The term.
        /// </param>
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
        /// The <see cref="QueryResultDisplay"/>.
        /// </returns>
        public QueryResultDisplay Search(string term, long page, long itemsPerPage, string sortBy = "name", SortDirection sortDirection = SortDirection.Ascending)
        {
            return GetQueryResultDisplay(_productService.GetPagedKeys(term, page, itemsPerPage, sortBy, sortDirection));
        }

        /// <summary>
        /// Gets the <see cref="ProductVariantDisplay"/> for a product
        /// </summary>
        /// <param name="productKey">
        /// The product key.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{ProductVariantDisplay}"/>.
        /// </returns>
        public IEnumerable<ProductVariantDisplay> GetVariantsByProduct(Guid productKey)
        {
            var criteria = SearchProvider.CreateSearchCriteria();
            criteria.Field(KeyFieldInIndex, productKey.ToString()).Not().Field("master", "True");

            var results = SearchProvider.Search(criteria);

            return results.Select(x => x.ToProductVariantDisplay());
        }

        /// <summary>
        /// Re-indexes the <see cref="IProduct"/>
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        internal override void ReindexEntity(IProduct entity)
        {
            IndexProvider.ReIndexNode(entity.SerializeToXml().Root, IndexTypes.ProductVariant);
        }

        internal void ReindexEntity(IProductVariant entity)
        {
            IndexProvider.ReIndexNode(entity.SerializeToXml().Root, IndexTypes.ProductVariant);
        }

        /// <summary>
        /// Maps a <see cref="SearchResult"/> to <see cref="ProductDisplay"/>
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <returns>
        /// The <see cref="ProductDisplay"/>.
        /// </returns>
        protected override ProductDisplay PerformMapSearchResultToDisplayObject(SearchResult result)
        {
            return result.ToProductDisplay(GetVariantsByProduct);
        }
    }
}