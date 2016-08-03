namespace Merchello.Web.Models
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    /// <summary>
    /// The paged collection.
    /// </summary>
    public class PagedCollection
    {
        /// <summary>
        /// Gets or sets the total items.
        /// </summary>
        public long TotalItems { get; set; }

        /// <summary>
        /// Gets or sets the current page.
        /// </summary>
        public long CurrentPage { get; set; }

        /// <summary>
        /// Gets or sets the page size.
        /// </summary>
        public long PageSize { get; set; }

        /// <summary>
        /// Gets or sets the total pages.
        /// </summary>
        public long TotalPages { get; set; }

        /// <summary>
        /// Gets or sets the sort field.
        /// </summary>
        public string SortField { get; set; }

        /// <summary>
        /// Gets a value indicating whether is first page.
        /// </summary>
        public bool IsFirstPage
        {
            get
            {
                return this.CurrentPage <= 1;
            }
        }

        /// <summary>
        /// Gets a value indicating whether is last page.
        /// </summary>
        public bool IsLastPage
        {
            get
            {
                return this.CurrentPage >= this.TotalPages;
            }
        }
    }

    /// <summary>
    /// The paged collection.
    /// </summary>
    /// <typeparam name="TResultType">
    /// The type to be paged
    /// </typeparam>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public class PagedCollection<TResultType> : PagedCollection
    {
        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        public IEnumerable<TResultType> Items { get; set; }

        /// <summary>
        /// Returns an empty page for defaults.
        /// </summary>
        /// <returns>
        /// The <see cref="PagedCollection"/>.
        /// </returns>
        public static PagedCollection<TResultType> Empty()
        {
            return new PagedCollection<TResultType>
            {
                CurrentPage = 1,
                Items = Enumerable.Empty<TResultType>(),
                PageSize = 20,
                TotalItems = 0,
                TotalPages = 0
            };
        }
    }
}