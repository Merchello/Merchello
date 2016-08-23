namespace Merchello.Core.Persistence.Repositories
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using Merchello.Core.Models.Interfaces;
    using Merchello.Core.Persistence.Querying;

    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.Querying;
    using Umbraco.Core.Persistence.Repositories;

    /// <summary>
    /// Defines an EntityCollectionRepository.
    /// </summary>
    internal interface IEntityCollectionRepository : IRepositoryQueryable<Guid, IEntityCollection>
    {
        /// <summary>
        /// The get entity collections by product key.
        /// </summary>
        /// <param name="productKey">
        /// The product key.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        /// <remarks>
        /// Used by the StaticProductCollectionProvider
        /// </remarks>
        IEnumerable<IEntityCollection> GetEntityCollectionsByProductKey(Guid productKey);

        /// <summary>
        /// The get entity collections by invoice key.
        /// </summary>
        /// <param name="invoiceKey">
        /// The invoice key.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        IEnumerable<IEntityCollection> GetEntityCollectionsByInvoiceKey(Guid invoiceKey);

        /// <summary>
        /// The get entity collections by customer key.
        /// </summary>
        /// <param name="customerKey">
        /// The customer key.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        IEnumerable<IEntityCollection> GetEntityCollectionsByCustomerKey(Guid customerKey);
            
        /// <summary>
        /// Gets a page of <see cref="IEntityCollection"/>
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <param name="orderExpression">
        /// The order expression.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{TEntity}"/>.
        /// </returns>
        Page<IEntityCollection> GetPage(long page, long itemsPerPage, IQuery<IEntityCollection> query, string orderExpression, SortDirection sortDirection = SortDirection.Descending);

        /// <summary>
        /// Gets a collection of <see cref="IEntitySpecifiedFilterCollection"/> by a collection of keys.
        /// </summary>
        /// <param name="keys">
        /// The keys.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        /// <remarks>
        /// TODO this is pretty brittle since it assumes the collection will be intended to be used as a specification collection.
        /// However, it merely builds a spec collection using whatever collection and it's children - so Service should definitely
        /// have this as an internal method until we can refactor
        /// </remarks>
        IEnumerable<IEntitySpecifiedFilterCollection> GetEntitySpecificationCollectionsByProviderKeys(Guid[] keys);

        /// <summary>
        ///  Gets <see cref="IEntitySpecifiedFilterCollection"/> by it's key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="IEntitySpecifiedFilterCollection"/>.
        /// </returns>
        /// <remarks>
        /// TODO this is pretty brittle since it assumes the collection will be intended to be used as a specification collection.
        /// However, it merely builds a spec collection using whatever collection and it's children - so Service should definitely
        /// have this as an internal method until we can refactor
        /// </remarks>
        IEntitySpecifiedFilterCollection GetEntitySpecificationCollection(Guid key);
    }
}