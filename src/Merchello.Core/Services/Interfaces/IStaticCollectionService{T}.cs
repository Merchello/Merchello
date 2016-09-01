namespace Merchello.Core.Services
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Models.Interfaces;
    using Merchello.Core.Persistence.Querying;

    using Umbraco.Core.Persistence;

    /// <summary>
    /// Defines a service that has static collections.
    /// </summary>
    /// <typeparam name="T">
    /// The type of <see cref="IEntity"/>
    /// </typeparam>
    public interface IStaticCollectionService<T>
        where T : class, IEntity
    {
        /// <summary>
        /// Adds an entity to a collection.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <param name="collection">
        /// The collection.
        /// </param>
        void AddToCollection(T entity, IEntityCollection collection);

        /// <summary>
        /// Adds an entity to a collection.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        void AddToCollection(T entity, Guid collectionKey);

        /// <summary>
        /// The add invoice to collection.
        /// </summary>
        /// <param name="entityKey">
        /// The entity key.
        /// </param>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        void AddToCollection(Guid entityKey, Guid collectionKey);

        /// <summary>
        /// The remove invoice from collection.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <param name="collection">
        /// The collection.
        /// </param>
        void RemoveFromCollection(T entity, IEntityCollection collection);

        /// <summary>
        /// The remove invoice from collection.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        void RemoveFromCollection(T entity, Guid collectionKey);

        /// <summary>
        /// The remove invoice from collection.
        /// </summary>
        /// <param name="entityKey">
        /// The entity key.
        /// </param>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        void RemoveFromCollection(Guid entityKey, Guid collectionKey);

        /// <summary>
        /// Returns true if the invoice exists in the static collection.
        /// </summary>
        /// <param name="entityKey">
        /// The entity key.
        /// </param>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool ExistsInCollection(Guid entityKey, Guid collectionKey);

        /// <summary>
        /// Returns true if the entity exists in the at least one of the static collections.
        /// </summary>
        /// <param name="entityKey">
        /// The entity key.
        /// </param>
        /// <param name="collectionKeys">
        /// The collection keys.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool ExistsInCollection(Guid entityKey, IEnumerable<Guid> collectionKeys);

        /// <summary>
        /// Gets an entity from a collection.
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
        /// The <see cref="Page{T}"/>.
        /// </returns>
        Page<T> GetFromCollection(
            Guid collectionKey,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending);

        /// <summary>
        /// Gets distinct entities from a multiple collection.
        /// </summary>
        /// <param name="collectionKeys">
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
        /// The <see cref="Page{T}"/>.
        /// </returns>
        Page<T> GetProductsThatExistInAllCollections(
            IEnumerable<Guid> collectionKeys,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending);
        

        /// <summary>
        /// Gets an entity from a collection filtered by a search term
        /// </summary>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        /// <param name="searchTerm">
        /// The search term.
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
        /// The <see cref="Page{T}"/>.
        /// </returns>
        Page<T> GetFromCollection(
            Guid collectionKey,
            string searchTerm,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending);


        /// <summary>
        /// Gets an entity from multiple collections filtered by a search term
        /// </summary>
        /// <param name="collectionKeys">
        /// A collection of collection keys.
        /// </param>
        /// <param name="searchTerm">
        /// The search term.
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
        /// The <see cref="Page{T}"/>.
        /// </returns>
        Page<T> GetProductsThatExistInAllCollections(
            IEnumerable<Guid> collectionKeys,
            string searchTerm,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending);
    }
}