namespace Merchello.Core.Persistence.Repositories
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Persistence.Querying;

    using Umbraco.Core.Persistence;

    /// <summary>
    /// Represents a price and collection queries for products.
    /// </summary>
    /// <remarks>
    /// This is simply a marker interface to mark V2 additions that need to be refactored and ported forward to V3
    /// </remarks>
    public interface IProductCollectionPriceQueries
    {
        ///// <summary>
        ///// The get keys not in collection.
        ///// </summary>
        ///// <param name="collectionKey">
        ///// The collection key.
        ///// </param>
        ///// <param name="min">
        ///// The min.
        ///// </param>
        ///// <param name="max">
        ///// The max.
        ///// </param>
        ///// <param name="page">
        ///// The page.
        ///// </param>
        ///// <param name="itemsPerPage">
        ///// The items per page.
        ///// </param>
        ///// <param name="orderExpression">
        ///// The order expression.
        ///// </param>
        ///// <param name="sortDirection">
        ///// The sort direction.
        ///// </param>
        ///// <returns>
        ///// The <see cref="Page{T}"/>.
        ///// </returns>
        //Page<Guid> GetKeysNotInCollection(
        //    Guid collectionKey,
        //    decimal min,
        //    decimal max,
        //    long page,
        //    long itemsPerPage,
        //    string orderExpression,
        //    SortDirection sortDirection = SortDirection.Descending);

        ///// <summary>
        ///// The get keys not in collection.
        ///// </summary>
        ///// <param name="collectionKey">
        ///// The collection key.
        ///// </param>
        ///// <param name="term">
        ///// The filter term
        ///// </param>
        ///// <param name="min">
        ///// The min.
        ///// </param>
        ///// <param name="max">
        ///// The max.
        ///// </param>
        ///// <param name="page">
        ///// The page.
        ///// </param>
        ///// <param name="itemsPerPage">
        ///// The items per page.
        ///// </param>
        ///// <param name="orderExpression">
        ///// The order expression.
        ///// </param>
        ///// <param name="sortDirection">
        ///// The sort direction.
        ///// </param>
        ///// <returns>
        ///// The <see cref="Page{Guid}"/>.
        ///// </returns>
        //Page<Guid> GetKeysNotInCollection(
        //    Guid collectionKey,
        //    string term,
        //    decimal min,
        //    decimal max,
        //    long page,
        //    long itemsPerPage,
        //    string orderExpression,
        //    SortDirection sortDirection = SortDirection.Descending);

        /// <summary>
        /// Gets the entity keys for distinct entities in multiple collections
        /// </summary>
        /// <param name="collectionKeys">
        /// The collection key.
        /// </param>
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
        /// <param name="orderExpression">
        /// The order expression.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{T}"/>.
        /// </returns>
        Page<Guid> GetKeysThatExistInAllCollections(
            Guid[] collectionKeys,
            decimal min,
            decimal max,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending);

        /// <summary>
        /// Gets a paged list of distinct keys for entities in multiple collections.
        /// </summary>
        /// <param name="collectionKeys">
        /// The collection keys.
        /// </param>
        /// <param name="term">
        /// A filter term
        /// </param>
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
        /// <param name="orderExpression">
        /// The order expression.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{T}"/>.
        /// </returns>
        Page<Guid> GetKeysThatExistInAllCollections(
            Guid[] collectionKeys,
            string term,
            decimal min,
            decimal max,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending);

        /// <summary>
        /// The get keys not in multiple collection.
        /// </summary>
        /// <param name="collectionKeys">
        /// The collection key.
        /// </param>
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
        /// <param name="orderExpression">
        /// The order expression.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        Page<Guid> GetKeysNotInAnyCollections(
            Guid[] collectionKeys,
            decimal min,
            decimal max,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending);

        /// <summary>
        /// The get keys not in multiple collections.
        /// </summary>
        /// <param name="collectionKeys">
        /// The collection key.
        /// </param>
        /// <param name="term">
        /// The filter term
        /// </param>
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
        /// <param name="orderExpression">
        /// The order expression.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        Page<Guid> GetKeysNotInAnyCollections(
            Guid[] collectionKeys,
            string term,
            decimal min,
            decimal max,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending);

        /// <summary>
        /// Gets a collection of keys that exist in any one of the collections passed.
        /// </summary>
        /// <param name="collectionKeys">
        /// The collection keys.
        /// </param>
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
        /// <param name="orderExpression">
        /// The order expression.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        Page<Guid> GetKeysThatExistInAnyCollections(
            Guid[] collectionKeys,
            decimal min,
            decimal max,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending);

        /// <summary>
        /// Gets a collection of keys that exist in any one of the collections passed.
        /// </summary>
        /// <param name="collectionKeys">
        /// The collection keys.
        /// </param>
        /// <param name="term">
        /// The search term.
        /// </param>
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
        /// <param name="orderExpression">
        /// The order expression.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        Page<Guid> GetKeysThatExistInAnyCollections(
            Guid[] collectionKeys,
            string term,
            decimal min,
            decimal max,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending);

        /// <summary>
        /// Gets a list of all manufacturers.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{String}"/> list of distinct manufacturers.
        /// </returns>
        IEnumerable<string> GetAllManufacturers();
    }
}