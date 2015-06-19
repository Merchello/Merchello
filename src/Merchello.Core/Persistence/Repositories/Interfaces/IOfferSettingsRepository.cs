namespace Merchello.Core.Persistence.Repositories
{
    using System.Web.UI;

    using Merchello.Core.Models.Interfaces;
    using Merchello.Core.Models.Rdbms;
    using Merchello.Core.Persistence.Querying;

    using Umbraco.Core.Persistence;

    /// <summary>
    /// Marker interface for the OfferSettingsRepository.
    /// </summary>
    public interface IOfferSettingsRepository : IPagedRepository<IOfferSettings, OfferSettingsDto>
    {
        /// <summary>
        /// Performs a paged search by search term.
        /// </summary>
        /// <param name="term">
        /// The term to filter by.
        /// </param>
        /// <param name="page">
        /// The current page.
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
        /// The <see cref="Page"/>.
        /// </returns>
        Page<IOfferSettings> Search(
            string term,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending);
    }
}