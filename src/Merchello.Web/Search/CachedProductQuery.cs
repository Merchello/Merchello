namespace Merchello.Web.Search
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
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
    using Merchello.Web.Caching;
    using Merchello.Web.DataModifiers;
    using Merchello.Web.DataModifiers.Product;
    using Merchello.Web.Models;
    using Merchello.Web.Models.ContentEditing.Content;
    using Merchello.Web.Models.VirtualContent;

    using Models.ContentEditing;
    using Models.Querying;

    using Umbraco.Core.Cache;
    using Umbraco.Core.Events;

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
        /// The <see cref="ProductContentFactory"/>.
        /// </summary>
        private readonly Lazy<ProductContentFactory> _productContentFactory;

        /// <summary>
        /// The <see cref="VirtualProductContentCache"/>.
        /// </summary>
        private VirtualProductContentCache _cache;

        /// <summary>
        /// The data modifier.
        /// </summary>
        private Lazy<IDataModifierChain<IProductVariantDataModifierData>> _dataModifier;


        /// <summary>
        /// Initializes a new instance of the <see cref="CachedProductQuery"/> class.
        /// </summary>
        public CachedProductQuery()
            : this(MerchelloContext.Current, true)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedProductQuery"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The <see cref="IMerchelloContext"/>.
        /// </param>
        /// <param name="enableDataModifiers">
        /// A value indicating whether or not data modifiers are enabled.
        /// </param>
        public CachedProductQuery(IMerchelloContext merchelloContext, bool enableDataModifiers)
            : this(merchelloContext, enableDataModifiers, DetachedValuesConversionType.Db)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedProductQuery"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The <see cref="IMerchelloContext"/>.
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
        public CachedProductQuery(IMerchelloContext merchelloContext, BaseIndexProvider indexProvider, BaseSearchProvider searchProvider, bool enableDataModifiers) 
            : this(merchelloContext, indexProvider, searchProvider, enableDataModifiers, DetachedValuesConversionType.Db)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedProductQuery"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The <see cref="IMerchelloContext"/>.
        /// </param>
        /// <param name="enableDataModifiers">
        /// The enable data modifiers.
        /// </param>
        /// <param name="conversionType">
        /// The detached value conversion type.
        /// </param>
        internal CachedProductQuery(IMerchelloContext merchelloContext, bool enableDataModifiers, DetachedValuesConversionType conversionType)
            : this(
            merchelloContext,
            ExamineManager.Instance.IndexProviderCollection["MerchelloProductIndexer"],
            ExamineManager.Instance.SearchProviderCollection["MerchelloProductSearcher"],
            enableDataModifiers,
            conversionType)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedProductQuery"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The <see cref="IMerchelloContext"/>.
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
        internal CachedProductQuery(IMerchelloContext merchelloContext, BaseIndexProvider indexProvider, BaseSearchProvider searchProvider, bool enableDataModifiers, DetachedValuesConversionType conversionType)
            : base(merchelloContext.Cache, merchelloContext.Services.ProductService, indexProvider, searchProvider, enableDataModifiers)
        {
            _productService = (ProductService)merchelloContext.Services.ProductService;
            this._conversionType = conversionType;
            _productContentFactory = new Lazy<ProductContentFactory>(() => new ProductContentFactory());
            _cache = new VirtualProductContentCache(merchelloContext.Cache, this.GetProductContent, enableDataModifiers);
            this.Initialize();
        }

        internal event TypedEventHandler<CachedProductQuery, bool> DataModifierChanged; 
        
        /// <summary>
        /// Gets or sets a value indicating whether enable data modifiers.
        /// </summary>
        internal override bool EnableDataModifiers
        {
            get
            {
                return base.EnableDataModifiers;
            }

            set
            {
                base.EnableDataModifiers = value;
                if (DataModifierChanged != null)
                {
                    DataModifierChanged.Invoke(this, value);
                }
            }
        }

        /// <summary>
        /// Gets the key field in index.
        /// </summary>
        protected override string KeyFieldInIndex
        {
            get { return "productKey"; }
        }

        /// <summary>
        /// Gets <see cref="IProductContent"/> by it's key
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="IProductContent"/>.
        /// </returns>
        public IProductContent TypedProductContent(Guid key)
        {
            return _cache.GetByKey(key);
        }

        /// <summary>
        /// Gets the typed content by it's sku.
        /// </summary>
        /// <param name="sku">
        /// The sku.
        /// </param>
        /// <returns>
        /// The <see cref="IProductContent"/>.
        /// </returns>
        public IProductContent TypedProductContentBySku(string sku)
        {
            return _cache.GetBySku(sku, GetProductContentBySku);
        }

        /// <summary>
        /// Gets the typed content by it's slug.
        /// </summary>
        /// <param name="slug">
        /// The slug.
        /// </param>
        /// <returns>
        /// The <see cref="IProductContent"/>.
        /// </returns>
        public IProductContent TypedProductContentBySlug(string slug)
        {
            return _cache.GetBySlug(slug, GetProductContentBySlug);
        }

        /// <summary>
        /// Gets the typed <see cref="IProductContent"/> for a collection.
        /// </summary>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IProductContent}"/>.
        /// </returns>
        public IEnumerable<IProductContent> TypedProductContentFromCollection(Guid collectionKey)
        {
            return TypedProductContentFromCollection(collectionKey, 1, long.MaxValue);
        }

        /// <summary>
        /// The typed product content from collection.
        /// </summary>
        /// <param name="collectionKey">
        /// The collection key.
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
        /// The <see cref="IEnumerable{IProductContent}"/>.
        /// </returns>
        public IEnumerable<IProductContent> TypedProductContentFromCollection(
            Guid collectionKey,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Ascending)
        {
            return TypedProductContentPageFromCollection(collectionKey, page, itemsPerPage, sortBy, sortDirection).Items;
        }


        /// <summary>
        /// Gets a <see cref="PagedCollection{IProductContent}"/>.
        /// </summary>
        /// <param name="collectionKey">
        /// The collection key.
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
        /// The <see cref="PagedCollection"/>.
        /// </returns>
        public PagedCollection<IProductContent> TypedProductContentPageFromCollection(
            Guid collectionKey,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Ascending)
        {
            var pagedKeys = GetCollectionPagedKeys(collectionKey, page, itemsPerPage, sortBy, sortDirection);

            return _cache.GetPagedCollectionByCacheKey(pagedKeys, sortBy);
        }

        /// <summary>
        /// Search returning an <see cref="IProductContent"/> collection.
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
        /// The <see cref="IEnumerable{IProductContent}"/>.
        /// </returns>
        public IEnumerable<IProductContent> TypedProductContentSearch(long page, long itemsPerPage, string sortBy = "name", SortDirection sortDirection = SortDirection.Descending)
        {
            return TypedProductContentSearchPaged(page, itemsPerPage, sortBy, sortDirection).Items;
        }

        /// <summary>
        /// Search returning an <see cref="IProductContent"/> collection.
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
        /// The <see cref="IEnumerable{IProductContent}"/>.
        /// </returns>
        public IEnumerable<IProductContent> TypedProductContentSearch(
            string term,
            long page,
            long itemsPerPage,
            string sortBy = "name",
            SortDirection sortDirection = SortDirection.Ascending)
        {
            return TypedProductContentSearchPaged(term, page, itemsPerPage, sortBy, sortDirection).Items;
        }

        /// <summary>
        /// Search returning an <see cref="IProductContent"/> paged collection.
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
        /// The <see cref="PagedCollection"/>.
        /// </returns>
        public PagedCollection<IProductContent> TypedProductContentSearchPaged(
            long page,
            long itemsPerPage,
            string sortBy = "name",
            SortDirection sortDirection = SortDirection.Descending)
        {
            var cacheKey = PagedKeyCache.GetPagedQueryCacheKey<ICachedProductQuery>("Search", page, itemsPerPage, sortBy, sortDirection);
            var pagedKeys = PagedKeyCache.GetPageByCacheKey(cacheKey);

            if (pagedKeys != null) return _cache.MapPagedCollection(pagedKeys, sortBy);

            return
                _cache.GetPagedCollectionByCacheKey(
                    PagedKeyCache.CachePage(cacheKey, _productService.GetPagedKeys(page, itemsPerPage, sortBy, sortDirection)), 
                    sortBy);
        }

        /// <summary>
        /// Search returning an <see cref="IProductContent"/> paged collection.
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
        /// The <see cref="PagedCollection"/>.
        /// </returns>
        public PagedCollection<IProductContent> TypedProductContentSearchPaged(
            string term,
            long page,
            long itemsPerPage,
            string sortBy = "name",
            SortDirection sortDirection = SortDirection.Descending)
        {
            var cacheKey = PagedKeyCache.GetPagedQueryCacheKey<ICachedProductQuery>("Search", page, itemsPerPage, sortBy, sortDirection, new Dictionary<string, string> { { "term", term } });
            var pagedKeys = PagedKeyCache.GetPageByCacheKey(cacheKey);
            return
                _cache.GetPagedCollectionByCacheKey(
                    pagedKeys ?? _productService.GetPagedKeys(term, page, itemsPerPage, sortBy, sortDirection),
                    sortBy);
        }

        /// <inheritdoc />
        public PagedCollection<IProductContent> TypedProductContentByPriceRange(
            decimal min,
            decimal max,
            long page,
            long itemsPerPage,
            string sortBy = "price",
            SortDirection sortDirection = SortDirection.Descending)
        {
            var cacheKey = PagedKeyCache.GetPagedQueryCacheKey<ICachedProductQuery>(
                "GetProductsInPriceRange",
                page,
                itemsPerPage,
                sortBy,
                sortDirection,
                new Dictionary<string, string>
                    {
                                    { "min", min.ToString(CultureInfo.InvariantCulture) },
                                    { "max", max.ToString(CultureInfo.InvariantCulture) }
                    });

            var pagedKeys = PagedKeyCache.GetPageByCacheKey(cacheKey);

            return
                _cache.GetPagedCollectionByCacheKey(
                    pagedKeys ?? _productService.GetProductsKeysInPriceRange(min, max, page, itemsPerPage, sortBy, sortDirection),
                    sortBy);
        }

        ///// <inheritdoc />
        //public PagedCollection<IProductContent> TypedProductContentByPriceRange(
        //    string searchTerm,
        //    decimal min,
        //    decimal max,
        //    long page,
        //    long itemsPerPage,
        //    string sortBy = "price",
        //    SortDirection sortDirection = SortDirection.Descending)
        //{
        //    throw new NotImplementedException();
        //}

        /// <summary>
        /// Gets a <see cref="PagedCollection{IProductContent}"/> that exists in every collection referenced.
        /// </summary>
        /// <param name="collectionKeys">
        /// The collection of collection keys.
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
        /// The <see cref="PagedCollection"/>.
        /// </returns>
        public PagedCollection<IProductContent> TypedProductContentPageThatExistInAllCollections(
            IEnumerable<Guid> collectionKeys,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Ascending)
        {
            var keys = collectionKeys as Guid[] ?? collectionKeys.ToArray();

            if (!keys.Any()) return PagedCollection<IProductContent>.Empty();

            var pagedKeys = ((ProductService)Service).GetKeysThatExistInAllCollections(keys, page, itemsPerPage, sortBy, sortDirection);

            return _cache.MapPagedCollection(pagedKeys, sortBy);
        }

        /// <summary>
        /// Gets a <see cref="PagedCollection{IProductContent}"/> that exists in every collection referenced.
        /// </summary>
        /// <param name="collectionKeys">
        /// The collection of collection keys.
        /// </param>
        /// <param name="searchTerm">
        /// The search Term.
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
        /// The <see cref="PagedCollection"/>.
        /// </returns>
        public PagedCollection<IProductContent> TypedProductContentPageThatExistInAllCollections(
            IEnumerable<Guid> collectionKeys,
            string searchTerm,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Ascending)
        {
            var keys = collectionKeys as Guid[] ?? collectionKeys.ToArray();

            if (!keys.Any()) return PagedCollection<IProductContent>.Empty();

            var pagedKeys = ((ProductService)Service).GetKeysThatExistInAllCollections(keys, searchTerm, page, itemsPerPage, sortBy, sortDirection);

            return _cache.MapPagedCollection(pagedKeys, sortBy);
        }

        /// <inheritdoc/>
        public PagedCollection<IProductContent> TypedProductContentPageThatExistInAllCollections(
            IEnumerable<Guid> collectionKeys,
            decimal min,
            decimal max,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Ascending)
        {
            var keys = collectionKeys as Guid[] ?? collectionKeys.ToArray();

            if (!keys.Any()) return PagedCollection<IProductContent>.Empty();

            var pagedKeys = ((ProductService)Service).GetKeysThatExistInAllCollections(keys, min, max, page, itemsPerPage, sortBy, sortDirection);

            return _cache.MapPagedCollection(pagedKeys, sortBy);
        }

        /// <inheritdoc/>
        public PagedCollection<IProductContent> TypedProductContentPageThatExistInAllCollections(
            IEnumerable<Guid> collectionKeys,
            string searchTerm,
            decimal min,
            decimal max,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Ascending)
        {
            var keys = collectionKeys as Guid[] ?? collectionKeys.ToArray();

            if (!keys.Any()) return PagedCollection<IProductContent>.Empty();

            var pagedKeys = ((ProductService)Service).GetKeysThatExistInAllCollections(keys, searchTerm, min, max, page, itemsPerPage, sortBy, sortDirection);

            return _cache.MapPagedCollection(pagedKeys, sortBy);
        }

        /// <summary>
        /// Gets a <see cref="PagedCollection{IProductContent}"/> that does not exists in any of the collections referenced..
        /// </summary>
        /// <param name="collectionKeys">
        /// The collection keys.
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
        /// The <see cref="PagedCollection"/>.
        /// </returns>
        public PagedCollection<IProductContent> TypedProductContentPageThatNotInAnyCollections(
            IEnumerable<Guid> collectionKeys,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Ascending)
        {
            var keys = collectionKeys as Guid[] ?? collectionKeys.ToArray();

            if (!keys.Any()) return PagedCollection<IProductContent>.Empty();

            var pagedKeys = ((ProductService)Service).GetKeysNotInAnyCollections(keys, page, itemsPerPage, sortBy, sortDirection);

            return _cache.MapPagedCollection(pagedKeys, sortBy);
        }

        /// <summary>
        /// Gets a <see cref="PagedCollection{IProductContent}"/> that does not exists in any of the collections referenced..
        /// </summary>
        /// <param name="collectionKeys">
        /// The collection keys.
        /// </param>
        /// <param name="searchTerm">
        /// The search Term.
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
        /// The <see cref="PagedCollection"/>.
        /// </returns>
        public PagedCollection<IProductContent> TypedProductContentPageThatNotInAnyCollections(
            IEnumerable<Guid> collectionKeys,
            string searchTerm,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Ascending)
        {
            var keys = collectionKeys as Guid[] ?? collectionKeys.ToArray();

            if (!keys.Any()) return PagedCollection<IProductContent>.Empty();

            var pagedKeys = ((ProductService)Service).GetKeysNotInAnyCollections(keys, searchTerm, page, itemsPerPage, sortBy, sortDirection);

            return _cache.MapPagedCollection(pagedKeys, sortBy);
        }

        /// <inheritdoc/>
        public PagedCollection<IProductContent> TypedProductContentPageThatNotInAnyCollections(
            IEnumerable<Guid> collectionKeys,
            decimal min,
            decimal max,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Ascending)
        {
            var keys = collectionKeys as Guid[] ?? collectionKeys.ToArray();

            if (!keys.Any()) return PagedCollection<IProductContent>.Empty();

            var pagedKeys = ((ProductService)Service).GetKeysNotInAnyCollections(keys, min, max, page, itemsPerPage, sortBy, sortDirection);

            return _cache.MapPagedCollection(pagedKeys, sortBy);
        }

        /// <inheritdoc/>
        public PagedCollection<IProductContent> TypedProductContentPageThatNotInAnyCollections(
            IEnumerable<Guid> collectionKeys,
            string searchTerm,
            decimal min,
            decimal max,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Ascending)
        {
            var keys = collectionKeys as Guid[] ?? collectionKeys.ToArray();

            if (!keys.Any()) return PagedCollection<IProductContent>.Empty();

            var pagedKeys = ((ProductService)Service).GetKeysNotInAnyCollections(keys, searchTerm, min, max, page, itemsPerPage, sortBy, sortDirection);

            return _cache.MapPagedCollection(pagedKeys, sortBy);
        }

        /// <summary>
        /// Gets a <see cref="PagedCollection{IProductContent}"/> that exists in any of the collections passed.
        /// </summary>
        /// <param name="collectionKeys">
        /// The collection keys.
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
        /// The <see cref="PagedCollection"/>.
        /// </returns>
        public PagedCollection<IProductContent> TypedProductContentPageThatExistsInAnyCollections(
            IEnumerable<Guid> collectionKeys,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Ascending)
        {
            var keys = collectionKeys as Guid[] ?? collectionKeys.ToArray();

            if (!keys.Any()) return PagedCollection<IProductContent>.Empty();

            var pagedKeys = ((ProductService)Service).GetKeysThatExistInAnyCollections(keys, page, itemsPerPage, sortBy, sortDirection);

            return _cache.MapPagedCollection(pagedKeys, sortBy);
        }

        /// <summary>
        /// Gets a <see cref="PagedCollection{IProductContent}"/> that exists in any of the collections passed.
        /// </summary>
        /// <param name="collectionKeys">
        /// The collection keys.
        /// </param>
        /// <param name="searchTerm">
        /// The search Term.
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
        /// The <see cref="PagedCollection"/>.
        /// </returns>
        public PagedCollection<IProductContent> TypedProductContentPageThatExistsInAnyCollections(
            IEnumerable<Guid> collectionKeys,
            string searchTerm,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Ascending)
        {
            var keys = collectionKeys as Guid[] ?? collectionKeys.ToArray();

            if (!keys.Any()) return PagedCollection<IProductContent>.Empty();

            var pagedKeys = ((ProductService)Service).GetKeysThatExistInAnyCollections(keys, searchTerm, page, itemsPerPage, sortBy, sortDirection);

            return _cache.MapPagedCollection(pagedKeys, sortBy);
        }

        /// <inheritdoc/>
        public PagedCollection<IProductContent> TypedProductContentPageThatExistsInAnyCollections(
            IEnumerable<Guid> collectionKeys,
            decimal min,
            decimal max,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Ascending)
        {
            var keys = collectionKeys as Guid[] ?? collectionKeys.ToArray();

            if (!keys.Any()) return PagedCollection<IProductContent>.Empty();

            var pagedKeys = ((ProductService)Service).GetKeysThatExistInAnyCollections(keys, min, max, page, itemsPerPage, sortBy, sortDirection);

            return _cache.MapPagedCollection(pagedKeys, sortBy);
        }

        /// <inheritdoc/>
        public PagedCollection<IProductContent> TypedProductContentPageThatExistsInAnyCollections(
            IEnumerable<Guid> collectionKeys,
            string searchTerm,
            decimal min,
            decimal max,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Ascending)
        {
            var keys = collectionKeys as Guid[] ?? collectionKeys.ToArray();

            if (!keys.Any()) return PagedCollection<IProductContent>.Empty();

            var pagedKeys = ((ProductService)Service).GetKeysThatExistInAnyCollections(keys, searchTerm, min, max, page, itemsPerPage, sortBy, sortDirection);

            return _cache.MapPagedCollection(pagedKeys, sortBy);
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


            var displays = SearchProvider.Search(criteria).Select(PerformMapSearchResultToDisplayObject);
            var display = displays.FirstOrDefault();

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

            var displays = SearchProvider.Search(criteria).Select(PerformMapSearchResultToDisplayObject).ToArray();

            var display = displays.FirstOrDefault(x => x.DetachedContents.Any(y => y.Slug.Equals(slug, StringComparison.InvariantCultureIgnoreCase)));

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


            if (result != null)
            {
                result.EnsureValueConversion(this._conversionType);
                return this.ModifyData(result);
            }

            var variant = _productService.GetProductVariantByKey(key);

            if (variant != null)
            {
                this.ReindexEntity(variant);

                return this.ModifyData(variant.ToProductVariantDisplay(this._conversionType));
            }
            return null;
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

            if (result != null)
            {
                result.EnsureValueConversion(this._conversionType);
                return this.ModifyData(result);
            }

            var variant = _productService.GetProductVariantBySku(sku);

            if (variant != null)
            {
                this.ReindexEntity(variant);

                return this.ModifyData(variant.ToProductVariantDisplay(this._conversionType));
            }

            return null;
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
            var cacheKey = PagedKeyCache.GetPagedQueryCacheKey<ICachedProductQuery>("Search", page, itemsPerPage, sortBy, sortDirection);
            var pagedKeys = PagedKeyCache.GetPageByCacheKey(cacheKey);
            return GetQueryResultDisplay(
                pagedKeys ??
                PagedKeyCache.CachePage(cacheKey, _productService.GetPagedKeys(page, itemsPerPage, sortBy, sortDirection)));
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
            var cacheKey = PagedKeyCache.GetPagedQueryCacheKey<ICachedProductQuery>("Search", page, itemsPerPage, sortBy, sortDirection, new Dictionary<string, string> { { "term", term } });
            var pagedKeys = PagedKeyCache.GetPageByCacheKey(cacheKey);
            return GetQueryResultDisplay(
                pagedKeys ??
                PagedKeyCache.CachePage(cacheKey, _productService.GetPagedKeys(term, page, itemsPerPage, sortBy, sortDirection)));
        }

        /// <summary>
        /// Gets products with that have an option with name.
        /// </summary>
        /// <param name="optionKey">
        /// The option key.
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
            Guid optionKey,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending)
        {

            var cacheKey = PagedKeyCache.GetPagedQueryCacheKey<ICachedProductQuery>(
                "GetProductsWithOption",
                page,
                itemsPerPage,
                sortBy,
                sortDirection,
                new Dictionary<string, string> { { "optionKey", optionKey.ToString() } });

            var pagedKeys = PagedKeyCache.GetPageByCacheKey(cacheKey);

            return
            this.GetQueryResultDisplay(
                pagedKeys ??
                PagedKeyCache.CachePage(
                    cacheKey,
                    _productService.GetProductsKeysWithOption(optionKey, page, itemsPerPage, sortBy, sortDirection)));
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
            var choices = choiceNames as string[] ?? choiceNames.ToArray();
            var args = new Dictionary<string, string>
                {
                    { "optionName", optionName },
                    { "choiceNames", string.Join(string.Empty, choices.OrderBy(x => x)) }
                };

            var cacheKey = PagedKeyCache.GetPagedQueryCacheKey<ICachedProductQuery>(
                "GetProductsWithOption",
                page,
                itemsPerPage,
                sortBy,
                sortDirection,
                args);

            var pagedKeys = PagedKeyCache.GetPageByCacheKey(cacheKey);

            return
                this.GetQueryResultDisplay(
                    pagedKeys ?? 
                    PagedKeyCache.CachePage(
                        cacheKey,
                        _productService.GetProductsKeysWithOption(optionName, choices, page, itemsPerPage, sortBy, sortDirection)));
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
            var cacheKey = PagedKeyCache.GetPagedQueryCacheKey<ICachedProductQuery>(
              "GetProductsWithOption",
              page,
              itemsPerPage,
              sortBy,
              sortDirection,
              new Dictionary<string, string> { { "optionName", optionName } });

            var pagedKeys = PagedKeyCache.GetPageByCacheKey(cacheKey);

            return
                this.GetQueryResultDisplay(
                    pagedKeys
                    ?? PagedKeyCache.CachePage(
                        cacheKey,
                        _productService.GetProductsKeysWithOption(optionName, page, itemsPerPage, sortBy, sortDirection)));
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
            var cacheKey = PagedKeyCache.GetPagedQueryCacheKey<ICachedProductQuery>(
              "GetProductsWithOption",
              page,
              itemsPerPage,
              sortBy,
              sortDirection,
              new Dictionary<string, string>
                  {
                        { "optionName", optionName },
                        { "choiceName", choiceName }
                  });

            var pagedKeys = PagedKeyCache.GetPageByCacheKey(cacheKey);

            return
                this.GetQueryResultDisplay(
                    pagedKeys ??
                    PagedKeyCache.CachePage(
                        cacheKey,
                        _productService.GetProductsKeysWithOption(optionName, choiceName, page, itemsPerPage, sortBy, sortDirection)));
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
            var names = optionNames as string[] ?? optionNames.ToArray();
            var cacheKey = PagedKeyCache.GetPagedQueryCacheKey<ICachedProductQuery>(
              "GetProductsWithOption",
              page,
              itemsPerPage,
              sortBy,
              sortDirection,
              new Dictionary<string, string>
                  {
                        { "optionNames", string.Join(string.Empty, names.OrderBy(x => x)) }
                  });

            var pagedKeys = PagedKeyCache.GetPageByCacheKey(cacheKey);

            return
                this.GetQueryResultDisplay(
                    pagedKeys ??
                    PagedKeyCache.CachePage(
                        cacheKey,
                        _productService.GetProductsKeysWithOption(names, page, itemsPerPage, sortBy, sortDirection)));
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
            var cacheKey = PagedKeyCache.GetPagedQueryCacheKey<ICachedProductQuery>(
                "GetProductsInPriceRange",
                page,
                itemsPerPage,
                sortBy,
                sortDirection,
                new Dictionary<string, string>
                    {
                        { "min", min.ToString(CultureInfo.InvariantCulture) },
                        { "max", max.ToString(CultureInfo.InvariantCulture) }
                    });

            var pagedKeys = PagedKeyCache.GetPageByCacheKey(cacheKey);

            return
                this.GetQueryResultDisplay(
                    pagedKeys ??
                    PagedKeyCache.CachePage(
                        cacheKey,
                        _productService.GetProductsKeysInPriceRange(min, max, page, itemsPerPage, sortBy, sortDirection)));
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
            var cacheKey = PagedKeyCache.GetPagedQueryCacheKey<ICachedProductQuery>(
                "GetProductsInPriceRange",
                page,
                itemsPerPage,
                sortBy,
                sortDirection,
                new Dictionary<string, string>
                    {
                        { "min", min.ToString(CultureInfo.InvariantCulture) },
                        { "max", max.ToString(CultureInfo.InvariantCulture) },
                        { "taxModifier", taxModifier.ToString(CultureInfo.InvariantCulture) }
                    });

            var pagedKeys = PagedKeyCache.GetPageByCacheKey(cacheKey);

            return
                this.GetQueryResultDisplay(
                    pagedKeys ??
                    PagedKeyCache.CachePage(
                        cacheKey,
                        _productService.GetProductsKeysInPriceRange(min, max, taxModifier, page, itemsPerPage, sortBy, sortDirection)));
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
            var cacheKey = PagedKeyCache.GetPagedQueryCacheKey<ICachedProductQuery>(
                "GetProductsByBarcode",
                page,
                itemsPerPage,
                sortBy,
                sortDirection,
                new Dictionary<string, string> { { "barcode", barcode } });

            var pagedKeys = PagedKeyCache.GetPageByCacheKey(cacheKey);

            return
                this.GetQueryResultDisplay(
                    pagedKeys ??
                    PagedKeyCache.CachePage(
                        cacheKey,
                        _productService.GetProductsByBarcode(barcode, page, itemsPerPage, sortBy, sortDirection)));
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
            var barcodesArray = barcodes as string[] ?? barcodes.ToArray();
            var cacheKey = PagedKeyCache.GetPagedQueryCacheKey<ICachedProductQuery>(
                "GetProductsByBarcode",
                page,
                itemsPerPage,
                sortBy,
                sortDirection,
                new Dictionary<string, string>
                    {
                        { "barcodes", string.Join(string.Empty, barcodesArray.OrderBy(x => x)) }
                    });

            var pagedKeys = PagedKeyCache.GetPageByCacheKey(cacheKey);

            return
                this.GetQueryResultDisplay(
                    pagedKeys ??
                    PagedKeyCache.CachePage(
                        cacheKey,
                        _productService.GetProductsByBarcode(barcodesArray, page, itemsPerPage, sortBy, sortDirection)));
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
            var cacheKey = PagedKeyCache.GetPagedQueryCacheKey<ICachedProductQuery>(
                "GetProductsByManufacturer",
                page,
                itemsPerPage,
                sortBy,
                sortDirection,
                new Dictionary<string, string> { { "manufacturer", manufacturer} });

            var pagedKeys = PagedKeyCache.GetPageByCacheKey(cacheKey);

            return
                this.GetQueryResultDisplay(
                    pagedKeys ??
                    PagedKeyCache.CachePage(
                        cacheKey,
                        _productService.GetProductsKeysByManufacturer(manufacturer, page, itemsPerPage, sortBy, sortDirection)));
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
            var manufacturerArray = manufacturer as string[] ?? manufacturer.ToArray();
            var cacheKey = PagedKeyCache.GetPagedQueryCacheKey<ICachedProductQuery>(
            "GetProductsByManufacturer",
            page,
            itemsPerPage,
            sortBy,
            sortDirection,
            new Dictionary<string, string> { { "manufacturer", string.Join(string.Empty, manufacturerArray.OrderBy(x => x)) } });

            var pagedKeys = PagedKeyCache.GetPageByCacheKey(cacheKey);

            return
                this.GetQueryResultDisplay(
                    pagedKeys ??
                    PagedKeyCache.CachePage(
                        cacheKey,
                        _productService.GetProductsKeysByManufacturer(manufacturerArray, page, itemsPerPage, sortBy, sortDirection)));
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
            var cacheKey = PagedKeyCache.GetPagedQueryCacheKey<ICachedProductQuery>(
                "GetProductsInStock",
                page,
                itemsPerPage,
                sortBy,
                sortDirection,
                new Dictionary<string, string>
                    {
                        {
                            "includeAllowOutOfStockPurchase",
                            includeAllowOutOfStockPurchase.ToString()
                        }
                    });

            var pagedKeys = PagedKeyCache.GetPageByCacheKey(cacheKey);

            return
                this.GetQueryResultDisplay(
                    pagedKeys ??
                    PagedKeyCache.CachePage(
                        cacheKey,
                        _productService.GetProductsKeysInStock(page, itemsPerPage, sortBy, sortDirection)));
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
            var cacheKey = PagedKeyCache.GetPagedQueryCacheKey<ICachedProductQuery>(
                "GetProductsOnSale",
                page,
                itemsPerPage,
                sortBy,
                sortDirection);

            var pagedKeys = PagedKeyCache.GetPageByCacheKey(cacheKey);

            return
                this.GetQueryResultDisplay(
                    pagedKeys ??
                    PagedKeyCache.CachePage(
                        cacheKey,
                        _productService.GetProductsKeysOnSale(page, itemsPerPage, sortBy, sortDirection)));
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
                display.EnsureValueConversion(this._conversionType);
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
        /// Gets the virtual content.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="IProductContent"/>.
        /// </returns>
        private IProductContent GetProductContent(Guid key)
        {
            var display = GetByKey(key);
            return display == null ? null :
                display.AsProductContent(_productContentFactory.Value);
        }

        /// <summary>
        /// Gest the <see cref="IProductContent"/> by sku.
        /// </summary>
        /// <param name="sku">
        /// The sku.
        /// </param>
        /// <returns>
        /// The <see cref="IProductContent"/>.
        /// </returns>
        private IProductContent GetProductContentBySku(string sku)
        {
            var display = GetBySku(sku);
            return display == null ? null :
                display.AsProductContent(_productContentFactory.Value);
        }

        /// <summary>
        /// Gets the <see cref="IProductContent"/> by slug.
        /// </summary>
        /// <param name="slug">
        /// The slug.
        /// </param>
        /// <returns>
        /// The <see cref="IProductContent"/>.
        /// </returns>
        private IProductContent GetProductContentBySlug(string slug)
        {
            var display = GetBySlug(slug);
            return display == null ? null :
                display.AsProductContent(_productContentFactory.Value);
        }

        /// <summary>
        /// Initializes the lazy
        /// </summary>
        private void Initialize()
        {
            if (MerchelloContext.HasCurrent)
            _dataModifier = new Lazy<IDataModifierChain<IProductVariantDataModifierData>>(() => new ProductVariantDataModifierChain(MerchelloContext.Current));
            DataModifierChanged += OnDataModifierChanged;
        }

        private void OnDataModifierChanged(CachedProductQuery sender, bool e)
        {
            _cache.ModifiedVersion = e;
        }
    }
}