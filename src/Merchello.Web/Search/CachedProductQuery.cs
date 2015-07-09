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

    using Merchello.Core.Chains;
    using Merchello.Examine.Providers;
    using Merchello.Web.DataModifiers;

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
        /// The data modifier.
        /// </summary>
        private Lazy<IDataModifierChain<IProductVariantDataModifierData>> _dataModifier; 

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedProductQuery"/> class.
        /// </summary>
        public CachedProductQuery()
            : this(MerchelloContext.Current.Services.ProductService, true)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedProductQuery"/> class.
        /// </summary>
        /// <param name="productService">
        /// The product service.
        /// </param>
        /// <param name="enableDataModifiers">
        /// A value indicating whether or not data modifiers are enabled.
        /// </param>
        public CachedProductQuery(IProductService productService, bool enableDataModifiers)
            : this(
            productService,
            ExamineManager.Instance.IndexProviderCollection["MerchelloProductIndexer"],
            ExamineManager.Instance.SearchProviderCollection["MerchelloProductSearcher"],
            enableDataModifiers)
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
        /// <param name="enableDataModifiers">
        /// A value indicating whether or not data modifiers are enabled.
        /// </param>
        public CachedProductQuery(IPageCachedService<IProduct> service, BaseIndexProvider indexProvider, BaseSearchProvider searchProvider, bool enableDataModifiers) 
            : base(service, indexProvider, searchProvider, enableDataModifiers)
        {
            _productService = (ProductService)service;
            this.Initialize();
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
            return this.ModifyData(GetDisplayObject(key));
        }

        /// <summary>
        /// Gets a <see cref="ProductDisplay"/> by it's SKU.
        /// </summary>
        /// <param name="sku">
        /// The SKU.
        /// </param>
        /// <returns>
        /// The <see cref="ProductDisplay"/>.
        /// </returns>
        public ProductDisplay GetBySku(string sku)
        {
            var criteria = SearchProvider.CreateSearchCriteria();
            criteria.Field("sku", sku).And().Field("master", "True");

            var display = SearchProvider.Search(criteria).Select(PerformMapSearchResultToDisplayObject).FirstOrDefault();

            if (display != null) return this.ModifyData(display);

            var entity = _productService.GetBySku(sku);

            if (entity == null) return null;

            ReindexEntity(entity);

            return this.ModifyData(AutoMapper.Mapper.Map<ProductDisplay>(entity));
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

            if (result != null) return this.ModifyData(result);

            var variant = _productService.GetProductVariantByKey(key);

            if (variant != null) this.ReindexEntity(variant);

            return this.ModifyData(variant.ToProductVariantDisplay());
        }

        /// <summary>
        /// Gets a <see cref="ProductVariantDisplay"/> by it's unique SKU
        /// </summary>
        /// <param name="sku">
        /// The SKU.
        /// </param>
        /// <returns>
        /// The <see cref="ProductVariantDisplay"/>.
        /// </returns>
        public ProductVariantDisplay GetProductVariantBySku(string sku)
        {
            var criteria = SearchProvider.CreateSearchCriteria();
            criteria.Field("sku", sku).Not().Field("master", "True");

            var result = CachedSearch(criteria, ExamineDisplayExtensions.ToProductVariantDisplay).FirstOrDefault();

            if (result != null) return this.ModifyData(result);

            var variant = _productService.GetProductVariantBySku(sku);

            if (variant != null) this.ReindexEntity(variant);

            return this.ModifyData(variant.ToProductVariantDisplay());
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

            return results.Select(x => this.ModifyData(x.ToProductVariantDisplay()));
        }

        /// <summary>
        /// Re-indexes the <see cref="IProduct"/>
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        internal override void ReindexEntity(IProduct entity)
        {
            ((ProductIndexer)IndexProvider).AddProductToIndex(entity);
        }

        /// <summary>
        /// Re-indexes entity document via Examine.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        internal void ReindexEntity(IProductVariant entity)
        {
            IndexProvider.ReIndexNode(entity.SerializeToXml().Root, IndexTypes.ProductVariant);
        }

        /// <summary>
        /// Modifies Product Data with configured DataModifier Chain.
        /// </summary>
        /// <param name="product">
        /// The product.
        /// </param>
        /// <returns>
        /// The <see cref="ProductDisplay"/>.
        /// </returns>
        internal ProductDisplay ModifyProductData(ProductDisplay product)
        {
            if (!EnableDataModifiers) return product;
            var modified = this.ModifyData(product);
            modified.ProductVariants = product.ProductVariants.Select(this.ModifyData);
            return modified;
        }

        /// <summary>
        /// The modify data.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <typeparam name="T">
        /// The type of data to be modified
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        internal T ModifyData<T>(T data)
            where T : class, IProductVariantDataModifierData
        {
            if (!EnableDataModifiers) return data;
            var attempt = _dataModifier.Value.Modify(data);
            if (!attempt.Success) return data;

            var modified = attempt.Result as T;
            return modified ?? data;
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


        /// <summary>
        /// Initializes the lazy
        /// </summary>
        private void Initialize()
        {
            if (MerchelloContext.HasCurrent)
            _dataModifier = new Lazy<IDataModifierChain<IProductVariantDataModifierData>>(() => new ProductVariantDataModifierChain(MerchelloContext.Current));    
        }
    }
}