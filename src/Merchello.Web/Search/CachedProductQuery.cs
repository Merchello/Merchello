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

    using Merchello.Core.Chains;
    using Merchello.Core.ValueConverters;
    using Merchello.Examine.Providers;
    using Merchello.Web.DataModifiers;
    using Merchello.Web.DataModifiers.Product;

    using Models.ContentEditing;
    using Models.Querying;

    /// <summary>
    /// Represents a CachedProductQuery
    /// </summary>
    internal class CachedProductQuery : CachedQueryableCollectionQueryBase<IProduct, ProductDisplay>, ICachedProductQuery
    {
        /// <summary>
        /// The product service.
        /// </summary>
        private readonly ProductService _productService;

        /// <summary>
        /// A value indicating whether or not this is being used for back office editors.
        /// </summary>
        private readonly DetachedValuesConversionType _conversionType;

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
            : this(productService, enableDataModifiers, DetachedValuesConversionType.Db)
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
            : this(service, indexProvider, searchProvider, enableDataModifiers, DetachedValuesConversionType.Db)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedProductQuery"/> class.
        /// </summary>
        /// <param name="productService">
        /// The product service.
        /// </param>
        /// <param name="enableDataModifiers">
        /// The enable data modifiers.
        /// </param>
        /// <param name="conversionType">
        /// The detached value conversion type.
        /// </param>
        internal CachedProductQuery(IProductService productService, bool enableDataModifiers, DetachedValuesConversionType conversionType)
            : this(
            productService,
            ExamineManager.Instance.IndexProviderCollection["MerchelloProductIndexer"],
            ExamineManager.Instance.SearchProviderCollection["MerchelloProductSearcher"],
            enableDataModifiers,
            conversionType)
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
        /// The enable data modifiers.
        /// </param>
        /// <param name="conversionType">
        /// The is for back office editors.
        /// </param>
        internal CachedProductQuery(IPageCachedService<IProduct> service, BaseIndexProvider indexProvider, BaseSearchProvider searchProvider, bool enableDataModifiers, DetachedValuesConversionType conversionType)
            : base(service, indexProvider, searchProvider, enableDataModifiers)
        {
            _productService = (ProductService)service;
            this._conversionType = conversionType;
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
            //// modify data is handled in GetDisplayObject!
            return GetDisplayObject(key);
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

            return this.ModifyData(entity.ToProductDisplay(this._conversionType));
        }

        /// <summary>
        /// Gets a product by it's slug.
        /// </summary>
        /// <param name="slug">
        /// The slug.
        /// </param>
        /// <returns>
        /// The <see cref="ProductDisplay"/>.
        /// </returns>
        public ProductDisplay GetBySlug(string slug)
        {
            var criteria = SearchProvider.CreateSearchCriteria();
            criteria.Field("slugs", slug).And().Field("master", "True");

            var display = SearchProvider.Search(criteria).Select(PerformMapSearchResultToDisplayObject).FirstOrDefault();

            // Don't modifiy the data here as it would have been modified in the PerformMapSearchResultToDisplayObject
            if (display != null) return display;

            var key = _productService.GetKeyForSlug(slug);

            return Guid.Empty.Equals(key) ? null : this.GetByKey(key);
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
            result.SetConversionType(this._conversionType);

            if (result != null) return this.ModifyData(result);

            var variant = _productService.GetProductVariantByKey(key);

            if (variant != null) this.ReindexEntity(variant);

            return this.ModifyData(variant.ToProductVariantDisplay(this._conversionType));
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
            result.SetConversionType(this._conversionType);

            if (result != null) return this.ModifyData(result);

            var variant = _productService.GetProductVariantBySku(sku);

            if (variant != null) this.ReindexEntity(variant);

            return this.ModifyData(variant.ToProductVariantDisplay(this._conversionType));
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
        /// Gets products with that have an option with name and a collection of choice names
        /// </summary>
        /// <param name="optionName">
        /// The option name.
        /// </param>
        /// <param name="choiceNames">
        /// The choice names.
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
        public QueryResultDisplay GetProductsWithOption(
            string optionName,
            IEnumerable<string> choiceNames,
            long page,
            long itemsPerPage,
            string sortBy = "name",
            SortDirection sortDirection = SortDirection.Descending)
        {
            return
                this.GetQueryResultDisplay(
                    _productService.GetProductsKeysWithOption(
                        optionName,
                        choiceNames,
                        page,
                        itemsPerPage,
                        sortBy,
                        sortDirection));
        }

        /// <summary>
        /// Gets products with that have an option with name.
        /// </summary>
        /// <param name="optionName">
        /// The option name.
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
        public QueryResultDisplay GetProductsWithOption(
            string optionName,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending)
        {
            return
                this.GetQueryResultDisplay(
                    _productService.GetProductsKeysWithOption(optionName, page, itemsPerPage, sortBy, sortDirection));
        }

        /// <summary>
        /// Gets products with that have an options with name and choice name
        /// </summary>
        /// <param name="optionName">
        /// The option name.
        /// </param>
        /// <param name="choiceName">
        /// The choice name.
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
        public QueryResultDisplay GetProductsWithOption(
            string optionName,
            string choiceName,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending)
        {
            return
                this.GetQueryResultDisplay(
                    _productService.GetProductsKeysWithOption(
                        optionName,
                        choiceName,
                        page,
                        itemsPerPage,
                        sortBy,
                        sortDirection));
        }

        /// <summary>
        /// Gets products with that have an options with names
        /// </summary>
        /// <param name="optionNames">
        /// The option names.
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
        public QueryResultDisplay GetProductsWithOption(
            IEnumerable<string> optionNames,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending)
        {
            return
                this.GetQueryResultDisplay(
                    _productService.GetProductsKeysWithOption(optionNames, page, itemsPerPage, sortBy, sortDirection));
        }

        /// <summary>
        /// Get products that have prices within a price range
        /// </summary>
        /// <param name="min">
        /// The min.
        /// </param>
        /// <param name="max">
        /// The max.
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
        public QueryResultDisplay GetProductsInPriceRange(
            decimal min,
            decimal max,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending)
        {
            return
                this.GetQueryResultDisplay(
                    _productService.GetProductsKeysInPriceRange(min, max, page, itemsPerPage, sortBy, sortDirection));
        }

        /// <summary>
        /// Get products that have prices within a price range allowing for a tax modifier
        /// </summary>
        /// <param name="min">
        /// The min.
        /// </param>
        /// <param name="max">
        /// The max.
        /// </param>
        /// <param name="taxModifier">
        /// The tax modifier.
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
        public QueryResultDisplay GetProductsInPriceRange(
            decimal min,
            decimal max,
            decimal taxModifier,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending)
        {
            return
                this.GetQueryResultDisplay(
                    _productService.GetProductsKeysInPriceRange(min, max, taxModifier, page, itemsPerPage, sortBy, sortDirection));
        }

        /// <summary>
        /// The get products by barcode.
        /// </summary>
        /// <param name="barcode">
        /// The barcode.
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
        public QueryResultDisplay GetProductsByBarcode(
            string barcode,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending)
        {
            return
                this.GetQueryResultDisplay(
                    _productService.GetProductsByBarcode(barcode, page, itemsPerPage, sortBy, sortDirection));
        }

        /// <summary>
        /// The get products by barcode.
        /// </summary>
        /// <param name="barcodes">
        /// The barcodes.
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
        public QueryResultDisplay GetProductsByBarcode(
            IEnumerable<string> barcodes,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending)
        {
            return
                this.GetQueryResultDisplay(
                    _productService.GetProductsByBarcode(barcodes, page, itemsPerPage, sortBy, sortDirection));
        }

        /// <summary>
        /// Gets products by manufacturer.
        /// </summary>
        /// <param name="manufacturer">
        /// The manufacturer.
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
        public QueryResultDisplay GetProductsByManufacturer(
            string manufacturer,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending)
        {
            return
                this.GetQueryResultDisplay(
                    _productService.GetProductsKeysByManufacturer(
                        manufacturer,
                        page,
                        itemsPerPage,
                        sortBy,
                        sortDirection));
        }

        /// <summary>
        /// Get products for a list of manufacturers.
        /// </summary>
        /// <param name="manufacturer">
        /// The manufacturer.
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
        public QueryResultDisplay GetProductsByManufacturer(
            IEnumerable<string> manufacturer,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending)
        {
            return
                this.GetQueryResultDisplay(
                    _productService.GetProductsKeysByManufacturer(
                        manufacturer,
                        page,
                        itemsPerPage,
                        sortBy,
                        sortDirection));
        }

        /// <summary>
        /// Gets products that are in stock or do not track inventory
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
        /// <param name="includeAllowOutOfStockPurchase">
        /// The include allow out of stock purchase.
        /// </param>
        /// <returns>
        /// The <see cref="QueryResultDisplay"/>.
        /// </returns>
        public QueryResultDisplay GetProductsInStock(
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending,
            bool includeAllowOutOfStockPurchase = false)
        {
            return
                this.GetQueryResultDisplay(
                    _productService.GetProductsKeysInStock(
                        page,
                        itemsPerPage,
                        sortBy,
                        sortDirection));
        }

        /// <summary>
        /// Gets products that are marked on sale
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
        public QueryResultDisplay GetProductsOnSale(
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending)
        {
            return
                this.GetQueryResultDisplay(
                    _productService.GetProductsKeysOnSale(
                        page,
                        itemsPerPage,
                        sortBy,
                        sortDirection));
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
        /// Gets a display object from the Examine cache or falls back the the database if not found
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="ProductDisplay"/>.
        /// </returns>
        protected override ProductDisplay GetDisplayObject(Guid key)
        {
            var criteria = SearchProvider.CreateSearchCriteria();
            criteria.Field(KeyFieldInIndex, key.ToString()).And().Field("master", "True");

            var display = SearchProvider.Search(criteria).Select(PerformMapSearchResultToDisplayObject).FirstOrDefault();

            if (display != null)
            {
                display.SetConversionType(this._conversionType);
                return display;
            }

            var entity = Service.GetByKey(key);

            if (entity == null) return null;

            ReindexEntity(entity);

            return this.ModifyData(entity.ToProductDisplay(this._conversionType));
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
            return this.ModifyData(result.ToProductDisplay(GetVariantsByProduct, this._conversionType));
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