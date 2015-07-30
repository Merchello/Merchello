namespace Merchello.Core.EntityCollections
{
    using System;
    using System.Collections.Generic;
    using System.Web.UI;

    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Models.TypeFields;
    using Merchello.Core.Persistence.Querying;

    using Umbraco.Core.Persistence;

    /// <summary>
    /// The EntityCollectionProvider interface.
    /// </summary>
    /// <typeparam name="T">
    /// The type of entity
    /// </typeparam>
    public interface IEntityCollectionProvider<T>
        where T : IEntity
    {
        /// <summary>
        /// Gets the provider key.
        /// </summary>
        Guid ProviderKey { get; }

        /// <summary>
        /// Gets the entity type.
        /// </summary>
        ITypeField EntityType { get; }

        /// <summary>
        /// Gets a value indicating whether this provider self registers.
        /// </summary>
        /// <remarks>
        /// If true, the boot manager will automatically add the collection to the merchEntityCollection table if it does not exist.
        /// Likewise, if the provider is removed, it will remove itself from the merchEntityCollection table
        /// </remarks>
        bool ManagesUniqueCollection { get; }

        /// <summary>
        /// The get entities.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        IEnumerable<T> GetEntities();

        /// <summary>
        /// The get entities.
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
        /// The <see cref="Page"/>.
        /// </returns>
        Page<T> GetPagedEntities(long page, long itemsPerPage, string sortBy = "name", SortDirection sortDirection = SortDirection.Ascending);

        /// <summary>
        /// The get paged entity keys.
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
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        Page<Guid> GetPagedEntityKeys(long page, long itemsPerPage, string sortBy = "name", SortDirection sortDirection = SortDirection.Ascending);
    }
}