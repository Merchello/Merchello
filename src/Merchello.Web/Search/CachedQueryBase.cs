namespace Merchello.Web.Search
{
    using System;
    using System.Linq;
    using System.Web.UI;

    using Core;
    using Core.Models.EntityBase;
    using Core.Services;
    using global::Examine;
    using global::Examine.Providers;
    using global::Examine.SearchCriteria;

    using Merchello.Web.Models.Querying;

    using umbraco;

    using Umbraco.Core.Persistence;

    /// <summary>
    /// The cached query base.
    /// </summary>
    /// <typeparam name="TEntity">
    /// The type of <see cref="IEntity"/>
    /// </typeparam>
    /// <typeparam name="TDisplay">
    /// The type of display object
    /// </typeparam>
    internal abstract class CachedQueryBase<TEntity, TDisplay>
        where TEntity : class, IEntity
        where TDisplay : class, new()
    {
        /// <summary>
        /// The <see cref="QueryResultFactory{TDisplay}"/>
        /// </summary>
        private readonly Lazy<QueryResultFactory<TDisplay>> _resultFactory; 

        /// <summary>
        /// The Merchello core service service.
        /// </summary>
        private readonly IPageCachedService<TEntity> _service;

        /// <summary>
        /// The Examine Index provider.
        /// </summary>
        private readonly BaseIndexProvider _indexProvider;

        /// <summary>
        /// The Examine Search provider.
        /// </summary>
        private readonly BaseSearchProvider _searchProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedQueryBase{TEntity,TDisplay}"/> class.
        /// </summary>
        /// <param name="service">
        /// The service.
        /// </param>
        /// <param name="indexProvider">
        /// The Examine Index provider.
        /// </param>
        /// <param name="searchProvider">
        /// The Examine Search provider.
        /// </param>
        protected CachedQueryBase(
            IPageCachedService<TEntity> service, 
            BaseIndexProvider indexProvider, 
            BaseSearchProvider searchProvider)
        {
            Mandate.ParameterNotNull(service, "service");
            Mandate.ParameterNotNull(indexProvider, "indexProvider");
            Mandate.ParameterNotNull(searchProvider, "searchProvider");

            _service = service;
            _indexProvider = indexProvider;
            _searchProvider = searchProvider;

            _resultFactory = new Lazy<QueryResultFactory<TDisplay>>(() => new QueryResultFactory<TDisplay>(PerformMapSearchResultToDisplayObject, GetDisplayObject));
        }

        /// <summary>
        /// Gets the index provider.
        /// </summary>
        protected BaseIndexProvider IndexProvider
        {
            get { return _indexProvider; }
        }

        /// <summary>
        /// Gets the search provider.
        /// </summary>
        protected BaseSearchProvider SearchProvider
        {
            get { return _searchProvider; }
        }

        /// <summary>
        /// Gets an display class by it's unique by key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="TDisplay"/>.
        /// </returns>
        public abstract TDisplay GetByKey(Guid key);


        /// <summary>
        /// The search cache.
        /// </summary>
        /// <param name="criteria">
        /// The criteria.
        /// </param>
        /// <param name="currentPage">
        /// The current page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <returns>
        /// The <see cref="QueryResultDisplay"/>.
        /// </returns>
        protected QueryResultDisplay CacheSearch(ISearchCriteria criteria, long currentPage, long itemsPerPage)
        {
            return _resultFactory.Value.BuildQueryResult(_searchProvider.Search(criteria), currentPage, itemsPerPage);
        }

        /// <summary>
        /// Gets a <see cref="QueryResultDisplay"/> from a <see cref="Page{Guid}"/>.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="keyField">
        /// The key field.
        /// </param>
        /// <returns>
        /// The <see cref="QueryResultDisplay"/>.
        /// </returns>
        protected QueryResultDisplay GetQueryResultDisplay(Page<Guid> page, string keyField)
        {
            return _resultFactory.Value.BuildQueryResult(page, keyField);
        }

        /// <summary>
        /// Gets a display object from the Examine cache or falls back the the database if not found
        /// </summary>
        /// <param name="keyField">
        /// The key field.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="TDisplay"/>.
        /// </returns>
        protected TDisplay GetDisplayObject(string keyField, Guid key)
        {
            var criteria = _searchProvider.CreateSearchCriteria();
            criteria.Field(keyField, key.ToString());

            var display = _searchProvider.Search(criteria).Select(PerformMapSearchResultToDisplayObject).FirstOrDefault();

            if (display != null) return display;

            var entity = _service.GetByKey(key);

            if (entity == null) return null;

            ReindexEntity(entity);

            return AutoMapper.Mapper.Map<TDisplay>(entity);
        }

        /// <summary>
        /// Maps a search result to display object.
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <returns>
        /// The <see cref="TDisplay"/>.
        /// </returns>
        protected abstract TDisplay PerformMapSearchResultToDisplayObject(SearchResult result);

        /// <summary>
        /// The re-index retrieved.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected abstract void ReindexEntity(TEntity entity);
    }
}