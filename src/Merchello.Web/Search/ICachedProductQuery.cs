﻿namespace Merchello.Web.Search
{
    using System;
    using System.Collections.Generic;

    using Core.Persistence.Querying;

    using Merchello.Core;
    using Merchello.Web.Models;
    using Merchello.Web.Models.VirtualContent;

    using Models.ContentEditing;
    using Models.Querying;

    /// <summary>
    /// Defines a CachedProductQuery.
    /// </summary>
    public interface ICachedProductQuery : ICachedCollectionQuery
    {
        /// <summary>
        /// Gets a <see cref="IProductContent"/> by it's key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="IProductContent"/>.
        /// </returns>
        IProductContent TypedProductContent(Guid key);

        /// <summary>
        /// Gets the <see cref="IProductContent"/> by it's sku.
        /// </summary>
        /// <param name="sku">
        /// The sku.
        /// </param>
        /// <returns>
        /// The <see cref="IProductContent"/>.
        /// </returns>
        IProductContent TypedProductContentBySku(string sku);

        /// <summary>
        /// Gets the <see cref="IProductContent"/> by it's slug.
        /// </summary>
        /// <param name="slug">
        /// The slug.
        /// </param>
        /// <returns>
        /// The <see cref="IProductContent"/>.
        /// </returns>
        IProductContent TypedProductContentBySlug(string slug);

        /// <summary>
        /// Gets the typed <see cref="IProductContent"/> for a collection.
        /// </summary>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IProductContent}"/>.
        /// </returns>
        IEnumerable<IProductContent> TypedProductContentFromCollection(Guid collectionKey);

        /// <summary>
        /// Gets the typed <see cref="IProductContent"/> for a collection.
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
        /// The <see cref="IEnumerable{IProductContent}"/>.
        /// </returns>
        IEnumerable<IProductContent> TypedProductContentFromCollection(Guid collectionKey, long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Ascending, bool includeUnavailable = false);

        /// <summary>
        /// Gets a <see cref="PagedCollection{IProductContent}"/>.
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
        /// The <see cref="PagedCollection"/>.
        /// </returns>
        PagedCollection<IProductContent> TypedProductContentPageFromCollection(Guid collectionKey, long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Ascending, bool includeUnavailable = false);

        /// <summary>
        /// Search returning an <see cref="IProductContent"/> collection.
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
        /// The <see cref="IEnumerable{IProductContent}"/>.
        /// </returns>
        IEnumerable<IProductContent> TypedProductContentSearch(long page, long itemsPerPage, string sortBy = "name", SortDirection sortDirection = SortDirection.Descending, bool includeUnavailable = false);

        /// <summary>
        /// Search returning an <see cref="IProductContent"/> collection.
        /// </summary>
        /// <param name="term">
        /// The term.
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
        /// The <see cref="IEnumerable{IProductContent}"/>.
        /// </returns>
        IEnumerable<IProductContent> TypedProductContentSearch(string term, long page, long itemsPerPage, string sortBy = "name", SortDirection sortDirection = SortDirection.Ascending, bool includeUnavailable = false);

        /// <summary>
        /// Search returning an <see cref="IProductContent"/> paged collection.
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
        /// The <see cref="PagedCollection"/>.
        /// </returns>
        PagedCollection<IProductContent> TypedProductContentSearchPaged(long page, long itemsPerPage, string sortBy = "name", SortDirection sortDirection = SortDirection.Descending, bool includeUnavailable = false);

        /// <summary>
        /// Search returning an <see cref="IProductContent"/> paged collection.
        /// </summary>
        /// <param name="term">
        /// The term.
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
        /// The <see cref="PagedCollection"/>.
        /// </returns>
        PagedCollection<IProductContent> TypedProductContentSearchPaged(string term, long page, long itemsPerPage, string sortBy = "name", SortDirection sortDirection = SortDirection.Descending, bool includeUnavailable = false);

        /// <summary>
        /// Gets a collection of typed product content by price range.
        /// </summary>
        /// <param name="min">
        /// The min price.
        /// </param>
        /// <param name="max">
        /// The max price.
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
        /// The <see cref="PagedCollection"/>.
        /// </returns>
        PagedCollection<IProductContent> TypedProductContentByPriceRange(decimal min, decimal max, long page, long itemsPerPage, string sortBy = "price", SortDirection sortDirection = SortDirection.Descending, bool includeUnavailable = false);

        ///// <summary>
        ///// Gets a collection of typed product content by price range including a search term.
        ///// </summary>
        ///// <param name="searchTerm">
        ///// The search term.
        ///// </param>
        ///// <param name="min">
        ///// The min price.
        ///// </param>
        ///// <param name="max">
        ///// The max price.
        ///// </param>
        ///// <param name="page">
        ///// The page.
        ///// </param>
        ///// <param name="itemsPerPage">
        ///// The items per page.
        ///// </param>
        ///// <param name="sortBy">
        ///// The sort by.
        ///// </param>
        ///// <param name="sortDirection">
        ///// The sort direction.
        ///// </param>
        ///// <returns>
        ///// The <see cref="PagedCollection"/>.
        ///// </returns>
        //PagedCollection<IProductContent> TypedProductContentByPriceRange(string searchTerm, decimal min, decimal max, long page, long itemsPerPage, string sortBy = "price", SortDirection sortDirection = SortDirection.Descending);

        /// <summary>
        /// Gets a <see cref="PagedCollection{IProductContent}"/> that exists in every collection referenced.
        /// </summary>
        /// <param name="collectionKeys">
        /// The collection of collection keys.
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
        /// The <see cref="PagedCollection"/>.
        /// </returns>
        PagedCollection<IProductContent> TypedProductContentPageThatExistInAllCollections(IEnumerable<Guid> collectionKeys, long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Ascending, bool includeUnavailable = false);

        /// <summary>
        /// Gets a <see cref="PagedCollection{IProductContent}"/> that exists in every collection referenced.
        /// </summary>
        /// <param name="collectionKeys">
        /// The collection of collection keys.
        /// </param>
        /// <param name="searchTerm">
        /// The search Term.
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
        /// The <see cref="PagedCollection"/>.
        /// </returns>
        PagedCollection<IProductContent> TypedProductContentPageThatExistInAllCollections(IEnumerable<Guid> collectionKeys, string searchTerm, long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Ascending, bool includeUnavailable = false);

        /// <summary>
        /// Gets a <see cref="PagedCollection{IProductContent}"/> that exists in every collection referenced.
        /// </summary>
        /// <param name="collectionKeys">
        /// The collection of collection keys.
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
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="PagedCollection"/>.
        /// </returns>
        PagedCollection<IProductContent> TypedProductContentPageThatExistInAllCollections(IEnumerable<Guid> collectionKeys, decimal min, decimal max, long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Ascending, bool includeUnavailable = false);

        /// <summary>
        /// Gets a <see cref="PagedCollection{IProductContent}"/> that exists in every collection referenced.
        /// </summary>
        /// <param name="collectionKeys">
        /// The collection of collection keys.
        /// </param>
        /// <param name="searchTerm">
        /// The search Term.
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
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="PagedCollection"/>.
        /// </returns>
        PagedCollection<IProductContent> TypedProductContentPageThatExistInAllCollections(IEnumerable<Guid> collectionKeys, string searchTerm, decimal min, decimal max, long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Ascending, bool includeUnavailable = false);




        /// <summary>
        /// Gets a <see cref="PagedCollection{IProductContent}"/> that does not exists in any of the collections referenced..
        /// </summary>
        /// <param name="collectionKeys">
        /// The collection keys.
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
        /// The <see cref="PagedCollection"/>.
        /// </returns>
        PagedCollection<IProductContent> TypedProductContentPageThatNotInAnyCollections(IEnumerable<Guid> collectionKeys, long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Ascending, bool includeUnavailable = false);

        /// <summary>
        /// Gets a <see cref="PagedCollection{IProductContent}"/> that does not exists in any of the collections referenced..
        /// </summary>
        /// <param name="collectionKeys">
        /// The collection keys.
        /// </param>
        /// <param name="searchTerm">
        /// The search Term.
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
        /// The <see cref="PagedCollection"/>.
        /// </returns>
        PagedCollection<IProductContent> TypedProductContentPageThatNotInAnyCollections(IEnumerable<Guid> collectionKeys, string searchTerm, long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Ascending, bool includeUnavailable = false);



        /// <summary>
        /// Gets a <see cref="PagedCollection{IProductContent}"/> that does not exists in any of the collections referenced..
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
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="PagedCollection"/>.
        /// </returns>
        PagedCollection<IProductContent> TypedProductContentPageThatNotInAnyCollections(IEnumerable<Guid> collectionKeys, decimal min, decimal max, long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Ascending, bool includeUnavailable = false);

        /// <summary>
        /// Gets a <see cref="PagedCollection{IProductContent}"/> that does not exists in any of the collections referenced..
        /// </summary>
        /// <param name="collectionKeys">
        /// The collection keys.
        /// </param>
        /// <param name="searchTerm">
        /// The search Term.
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
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="PagedCollection"/>.
        /// </returns>
        PagedCollection<IProductContent> TypedProductContentPageThatNotInAnyCollections(IEnumerable<Guid> collectionKeys, string searchTerm, decimal min, decimal max, long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Ascending, bool includeUnavailable = false);


        /// <summary>
        /// Gets a <see cref="PagedCollection{IProductContent}"/> that exists in any of the collections passed.
        /// </summary>
        /// <param name="collectionKeys">
        /// The collection keys.
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
        /// The <see cref="PagedCollection"/>.
        /// </returns>
        PagedCollection<IProductContent> TypedProductContentPageThatExistsInAnyCollections(IEnumerable<Guid> collectionKeys, long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Ascending, bool includeUnavailable = false);


        /// <summary>
        /// Gets a <see cref="PagedCollection{IProductContent}"/> that exists in any of the collections passed.
        /// </summary>
        /// <param name="collectionKeys">
        /// The collection keys.
        /// </param>
        /// <param name="searchTerm">
        /// The search Term.
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
        /// The <see cref="PagedCollection"/>.
        /// </returns>
        PagedCollection<IProductContent> TypedProductContentPageThatExistsInAnyCollections(IEnumerable<Guid> collectionKeys, string searchTerm, long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Ascending, bool includeUnavailable = false);

        /// <summary>
        /// Gets a <see cref="PagedCollection{IProductContent}"/> that exists in any of the collections passed.
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
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="PagedCollection"/>.
        /// </returns>
        PagedCollection<IProductContent> TypedProductContentPageThatExistsInAnyCollections(IEnumerable<Guid> collectionKeys, decimal min, decimal max, long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Ascending, bool includeUnavailable = false);

        /// <summary>
        /// Gets a <see cref="PagedCollection{IProductContent}"/> that exists in any of the collections passed.
        /// </summary>
        /// <param name="collectionKeys">
        /// The collection keys.
        /// </param>
        /// <param name="searchTerm">
        /// The search Term.
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
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="PagedCollection"/>.
        /// </returns>
        PagedCollection<IProductContent> TypedProductContentPageThatExistsInAnyCollections(IEnumerable<Guid> collectionKeys, string searchTerm, decimal min, decimal max, long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Ascending, bool includeUnavailable = false);



        /// <summary>
        /// Gets a <see cref="ProductDisplay"/> by it's key
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="ProductDisplay"/>.
        /// </returns>
        ProductDisplay GetByKey(Guid key);

        /// <summary>
        /// Gets a <see cref="ProductDisplay"/> by it's SKU
        /// </summary>
        /// <param name="sku">
        /// The SKU.
        /// </param>
        /// <returns>
        /// The <see cref="ProductDisplay"/>.
        /// </returns>
        ProductDisplay GetBySku(string sku);

        /// <summary>
        /// The get by slug.
        /// </summary>
        /// <param name="slug">
        /// The slug.
        /// </param>
        /// <returns>
        /// The <see cref="ProductDisplay"/>.
        /// </returns>
        ProductDisplay GetBySlug(string slug);

        /// <summary>
        /// Gets a <see cref="ProductVariantDisplay"/> by it's key
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="ProductVariantDisplay"/>.
        /// </returns>
        ProductVariantDisplay GetProductVariantByKey(Guid key);

        /// <summary>
        /// Gets a <see cref="ProductVariantDisplay"/> by it's SKU
        /// </summary>
        /// <param name="sku">
        /// The SKU.
        /// </param>
        /// <returns>
        /// The <see cref="ProductVariantDisplay"/>.
        /// </returns>
        ProductVariantDisplay GetProductVariantBySku(string sku);

        /// <summary>
        /// Searches all products
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
        /// The <see cref="QueryResultDisplay"/>.
        /// </returns>
        QueryResultDisplay Search(long page, long itemsPerPage, string sortBy = "name", SortDirection sortDirection = SortDirection.Descending);


        /// <summary>
        /// Searches all products for a given term
        /// </summary>
        /// <param name="term">
        /// The term.
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
        /// The <see cref="QueryResultDisplay"/>.
        /// </returns>
        QueryResultDisplay Search(string term, long page, long itemsPerPage, string sortBy = "name", SortDirection sortDirection = SortDirection.Ascending);

        /// <summary>
        /// Gets products with that have an option with name.
        /// </summary>
        /// <param name="optionKey">
        /// The option key.
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
        /// The <see cref="QueryResultDisplay"/>.
        /// </returns>
        QueryResultDisplay GetProductsWithOption(Guid optionKey, long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Descending);


        /// <summary>
        /// Gets products with that have an option with name and a collection of choice names
        /// </summary>
        /// <param name="optionName">
        /// The option name.
        /// </param>
        /// <param name="choiceNames">
        /// The choice names.
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
        /// The <see cref="QueryResultDisplay"/>.
        /// </returns>
        QueryResultDisplay GetProductsWithOption(string optionName, IEnumerable<string> choiceNames, long page, long itemsPerPage, string sortBy = "name", SortDirection sortDirection = SortDirection.Descending);

        /// <summary>
        /// Gets products with that have an option with name.
        /// </summary>
        /// <param name="optionName">
        /// The option name.
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
        /// The <see cref="QueryResultDisplay"/>.
        /// </returns>
        QueryResultDisplay GetProductsWithOption(string optionName, long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Descending);

        /// <summary>
        /// Gets products with that have an options with name and choice name
        /// </summary>
        /// <param name="optionName">
        /// The option name.
        /// </param>
        /// <param name="choiceName">
        /// The choice name.
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
        /// The <see cref="QueryResultDisplay"/>.
        /// </returns>
        QueryResultDisplay GetProductsWithOption(string optionName, string choiceName, long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Descending);

        /// <summary>
        /// Gets products with that have an options with names
        /// </summary>
        /// <param name="optionNames">
        /// The option names.
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
        /// The <see cref="QueryResultDisplay"/>.
        /// </returns>
        QueryResultDisplay GetProductsWithOption(IEnumerable<string> optionNames, long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Descending);

        /// <summary>
        /// Get products that have prices within a price range
        /// </summary>
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
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="QueryResultDisplay"/>.
        /// </returns>
        QueryResultDisplay GetProductsInPriceRange(decimal min, decimal max, long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Descending);

        /// <summary>
        /// Get products that have prices within a price range allowing for a tax modifier
        /// </summary>
        /// <param name="min">
        /// The min.
        /// </param>
        /// <param name="max">
        /// The max.
        /// </param>
        /// <param name="taxModifier">
        /// The tax modifier.
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
        /// The <see cref="QueryResultDisplay"/>.
        /// </returns>
        QueryResultDisplay GetProductsInPriceRange(decimal min, decimal max, decimal taxModifier, long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Descending);

        /// <summary>
        /// The get products by barcode.
        /// </summary>
        /// <param name="barcode">
        /// The barcode.
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
        /// The <see cref="QueryResultDisplay"/>.
        /// </returns>
        QueryResultDisplay GetProductsByBarcode(string barcode, long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Descending);

        /// <summary>
        /// The get products by barcode.
        /// </summary>
        /// <param name="barcode">
        /// The barcode.
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
        /// The <see cref="QueryResultDisplay"/>.
        /// </returns>
        QueryResultDisplay GetProductsByBarcode(IEnumerable<string> barcode, long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Descending);

        /// <summary>
        /// Gets products by manufacturer.
        /// </summary>
        /// <param name="manufacturer">
        /// The manufacturer.
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
        /// The <see cref="QueryResultDisplay"/>.
        /// </returns>
        QueryResultDisplay GetProductsByManufacturer(string manufacturer, long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Descending);

        /// <summary>
        /// Get products for a list of manufacturers.
        /// </summary>
        /// <param name="manufacturer">
        /// The manufacturer.
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
        /// The <see cref="QueryResultDisplay"/>.
        /// </returns>
        QueryResultDisplay GetProductsByManufacturer(IEnumerable<string> manufacturer, long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Descending);

        /// <summary>
        /// Gets products that are in stock or do not track inventory
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
        /// <param name="includeAllowOutOfStockPurchase">
        /// The include allow out of stock purchase.
        /// </param>
        /// <returns>
        /// The <see cref="QueryResultDisplay"/>.
        /// </returns>
        QueryResultDisplay GetProductsInStock(long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Descending, bool includeAllowOutOfStockPurchase = false);

        /// <summary>
        /// Gets products that are marked on sale
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
        /// The <see cref="QueryResultDisplay"/>.
        /// </returns>
        QueryResultDisplay GetProductsOnSale(long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Descending);
    }
}