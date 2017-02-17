namespace Merchello.Core
{
    using System;
    using System.Linq;

    using Merchello.Core.Models;

    using Umbraco.Core.Persistence;

    /// <summary>
    /// Extension methods for PagedCollection.
    /// </summary>
    public static partial class Extensions
    {
        /// <summary>
        /// Maps a <see cref="Page{TDto}"/> to <see cref="PagedCollection{TItem}"/>.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="mapper">
        /// The mapper.
        /// </param>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <typeparam name="TDto">
        /// The originating DTO
        /// </typeparam>
        /// <typeparam name="TItem">
        /// The result item
        /// </typeparam>
        /// <returns>
        /// The <see cref="PagedCollection"/>.
        /// </returns>
        internal static PagedCollection<TItem> AsPagedCollection<TDto, TItem>(this Page<TDto> page, Func<TDto, TItem> mapper, string sortBy = "")
        {
            return new PagedCollection<TItem>
                       {
                           CurrentPage = page.CurrentPage,
                           TotalItems = page.TotalItems,
                           TotalPages = page.TotalPages,
                           PageSize = page.ItemsPerPage,
                           Items = page.Items.Select(mapper.Invoke),
                           SortField = sortBy
                       };
        }
    }
}
