namespace Merchello.Web.Search
{
    using System;
    using System.Linq;

    using global::Examine;
    using Merchello.Web.Models.Querying;

    using Umbraco.Core.Persistence;

    /// <summary>
    /// The query result factory.
    /// </summary>\
    /// <typeparam name="TDisplay">
    /// The type of display object
    /// </typeparam>
    internal class QueryResultFactory<TDisplay>
        where TDisplay : class, new()
    {
        /// <summary>
        /// Maps a <see cref="SearchResult"/> to <see cref="TDisplay"/>
        /// </summary>
        private readonly Func<SearchResult, TDisplay> _map;

        /// <summary>
        /// Gets a <see cref="TDisplay"/>
        /// </summary>
        private readonly Func<Guid, TDisplay> _get; 

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryResultFactory{TDisplay}"/> class.
        /// </summary>
        /// <param name="map">
        /// The map.
        /// </param>
        /// <param name="get">
        /// The get method
        /// </param>
        public QueryResultFactory(Func<SearchResult, TDisplay> map, Func<Guid, TDisplay> get)
        {
            _map = map;
            _get = get;
        }

        /// <summary>
        /// Builds a <see cref="QueryResultDisplay"/>
        /// </summary>
        /// <param name="results">
        /// The results.
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
        public QueryResultDisplay BuildQueryResult(ISearchResults results, long currentPage, long itemsPerPage)
        {
            return new QueryResultDisplay()
            {
                Items = results.Select(_map).Skip((int)((currentPage - 1) * itemsPerPage)).Take((int)itemsPerPage),
                CurrentPage = currentPage,
                ItemsPerPage = itemsPerPage,
                TotalPages = ((results.Count() - 1) / itemsPerPage) + 1,
                TotalItems = results.Count()
            };
        }

        /// <summary>
        /// Builds a <see cref="QueryResultDisplay"/> from a <see cref="Page{Guid}"/>.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <returns>
        /// The <see cref="QueryResultDisplay"/>.
        /// </returns>
        public QueryResultDisplay BuildQueryResult(Page<Guid> page)
        {
            return new QueryResultDisplay()
            {
                Items = page.Items.Select(x => _get(x)),
                CurrentPage = page.CurrentPage - 1,
                ItemsPerPage = page.ItemsPerPage,
                TotalPages = page.TotalPages,
                TotalItems = page.TotalItems
            };
        }
    }
}