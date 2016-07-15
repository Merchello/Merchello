namespace Merchello.Web.Models.Querying
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Web.UI;

    using Merchello.Core.Models.EntityBase;

    using Umbraco.Core.Persistence;

    /// <summary>
    /// A wrapper to return query results
    /// </summary>
    public class QueryResultDisplay
    {
        /// <summary>
        /// Gets or sets the current page index
        /// </summary>
        public long CurrentPage { get; set; }

        /// <summary>
        /// Gets or sets the page size.
        /// </summary>
        public long ItemsPerPage { get; set; }

        /// <summary>
        /// Gets or sets the total number of pages.
        /// </summary>
        public long TotalPages { get; set; }

        /// <summary>
        /// Gets or sets the number of total results in returned by the query.
        /// </summary>
        public long TotalItems { get; set; }

        /// <summary>
        /// Gets or sets the results to be serialized
        /// </summary>
        public IEnumerable<object> Items { get; set; }
    }

    /// <summary>
    /// Extension methods for <see cref="QueryResultDisplay"/>.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    internal static class QueryResultDisplayExtensions
    {
        /// <summary>
        /// The to query result display.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="mapper">
        /// The mapping function to map <see cref="IEntity"/> to the display object
        /// </param>
        /// <typeparam name="TEntity">
        /// The type of the <see cref="IEntity"/>
        /// </typeparam>
        /// <typeparam name="TDispaly">
        /// The type of display object
        /// </typeparam>
        /// <returns>
        /// The <see cref="QueryResultDisplay"/>.
        /// </returns>
        public static QueryResultDisplay ToQueryResultDisplay<TEntity, TDispaly>(this Page<TEntity> page, Func<TEntity, TDispaly> mapper)
            where TEntity : IEntity
            where TDispaly : class
        {
            return new QueryResultDisplay
                       {
                           CurrentPage = page.CurrentPage,
                           ItemsPerPage = page.ItemsPerPage,
                           TotalItems = page.TotalItems,
                           TotalPages = page.TotalPages,
                           Items = page.Items.Select(mapper.Invoke)
                       };
        }

    }
}