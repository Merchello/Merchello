namespace Merchello.Core.Services
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Models;
    using Merchello.Core.Persistence.Querying;

    using Umbraco.Core.Persistence;
    using Umbraco.Core.Services;

    /// <summary>
    /// Defines the product option service.
    /// </summary>
    public interface IProductOptionService : IService
    {
        /// <summary>
        /// Creates a <see cref="IProductOption"/> without saving it to the database.
        /// </summary>
        /// <param name="name">
        /// The option name.
        /// </param>
        /// <param name="shared">
        /// A value indicating whether or not this is a shared option (usable by multiple products).
        /// </param>
        /// <param name="required">
        /// The required.
        /// </param>
        /// <param name="raiseEvents">
        ///  Optional boolean indicating whether or not to raise events.
        /// </param>
        /// <returns>
        /// The <see cref="IProductOption"/>.
        /// </returns>
        IProductOption CreateProductOption(string name, bool shared = false, bool required = true, bool raiseEvents = true);

        /// <summary>
        /// Creates a <see cref="IProductOption"/> and saves it to the database.
        /// </summary>
        /// <param name="name">
        /// The option name.
        /// </param>
        /// <param name="shared">
        /// A value indicating whether or not this is a shared option (usable by multiple products).
        /// </param>
        /// <param name="required">
        /// The required.
        /// </param>
        /// <param name="raiseEvents">
        ///  Optional boolean indicating whether or not to raise events.
        /// </param>
        /// <returns>
        /// The <see cref="IProductOption"/>.
        /// </returns>
        IProductOption CreateProductOptionWithKey(string name, bool shared = false, bool required = true, bool raiseEvents = true);

        /// <summary>
        /// Saves a single product option.
        /// </summary>
        /// <param name="option">
        /// The option to be saved
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events.
        /// </param>
        void Save(IProductOption option, bool raiseEvents = true);


        /// <summary>
        /// Saves a collection of product options
        /// </summary>
        /// <param name="options">
        /// The collection of product options to be saved
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events.
        /// </param>
        void Save(IEnumerable<IProductOption> options, bool raiseEvents = true);


        /// <summary>
        /// Deletes a product option
        /// </summary>
        /// <param name="option">
        /// The option to be deleted
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events.
        /// </param>
        /// <remarks>
        /// This performs a check to ensure the option is valid to be deleted
        /// </remarks>
        void Delete(IProductOption option, bool raiseEvents = true);

        /// <summary>
        /// Deletes a collection of product options
        /// </summary>
        /// <param name="options">
        /// The collection of product options to be deleted
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events.
        /// </param>
        /// <remarks>
        /// This performs a check to ensure the option is valid to be deleted
        /// </remarks>
        void Delete(IEnumerable<IProductOption> options, bool raiseEvents = true);

        /// <summary>
        /// Gets a <see cref="IProductOption"/> by it's key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="IProductOption"/>.
        /// </returns>
        IProductOption GetByKey(Guid key);

        /// <summary>
        /// Gets a collection of <see cref="IProductOption"/> by a list of keys.
        /// </summary>
        /// <param name="keys">
        /// The keys.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IProductOption}"/>.
        /// </returns>
        IEnumerable<IProductOption> GetByKeys(IEnumerable<Guid> keys);


        /// <summary>
        /// Gets a page of <see cref="IProductOption"/>.
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
        /// <param name="sharedOnly">
        /// The shared Only.
        /// </param>
        /// <returns>
        /// The <see cref="Page{IProductOption}"/>.
        /// </returns>
        Page<IProductOption> GetPage(long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Descending, bool sharedOnly = true);


        /// <summary>
        /// Gets a page of <see cref="IProductOption"/>.
        /// </summary>
        /// <param name="term">
        /// A search term to filter by
        /// </param>
        /// <param name="page">
        /// The page requested.
        /// </param>
        /// <param name="itemsPerPage">
        /// The number of items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort by field.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <param name="sharedOnly">
        /// Indicates whether or not to only include shared option.
        /// </param>
        /// <returns>
        /// The <see cref="Page{IProductOption}"/>.
        /// </returns>
        Page<IProductOption> GetPage(string term, long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Descending, bool sharedOnly = true);
    }
}