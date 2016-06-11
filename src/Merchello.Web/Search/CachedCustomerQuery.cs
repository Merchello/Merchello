﻿namespace Merchello.Web.Search
{
    using System;
    using System.Collections.Generic;

    using Core;
    using Core.Models;
    using Core.Persistence.Querying;
    using Core.Services;
    using Examine;
    using global::Examine;
    using global::Examine.Providers;

    using Models.ContentEditing;
    using Models.Querying;

    /// <summary>
    /// Represents a CachedCustomerQuery.
    /// </summary>
    internal class CachedCustomerQuery : CachedQueryableCollectionQueryBase<ICustomer, CustomerDisplay>, ICachedCustomerQuery
    {
        /// <summary>
        /// The customer service.
        /// </summary>
        private readonly CustomerService _customerService;

        /// <summary>
        /// The method to retrieve invoices for a customer
        /// </summary>
        private readonly Func<Guid, IEnumerable<InvoiceDisplay>> _getInvoiceByCustomer;

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedCustomerQuery"/> class.
        /// </summary>
        public CachedCustomerQuery()
            : this (MerchelloContext.Current.Services.CustomerService, true)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedCustomerQuery"/> class.
        /// </summary>
        /// <param name="customerService">
        /// The customer service.
        /// </param>
        /// <param name="enableDataModifiers">
        /// A value indicating whether or not data modifiers are enabled.
        /// </param>
        public CachedCustomerQuery(ICustomerService customerService, bool enableDataModifiers)
            : this(
            customerService,
            ExamineManager.Instance.IndexProviderCollection["MerchelloCustomerIndexer"],
            ExamineManager.Instance.SearchProviderCollection["MerchelloCustomerSearcher"],
            enableDataModifiers)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedCustomerQuery"/> class.
        /// </summary>
        /// <param name="service">
        /// The service.
        /// </param>
        /// <param name="indexProvider">
        /// The index provider.
        /// </param>
        /// <param name="searchProvider">
        /// The search provider.
        /// </param>
        /// <param name="enableDataModifiers">
        /// A value indicating whether or not data modifiers are enabled.
        /// </param>
        internal CachedCustomerQuery(IPageCachedService<ICustomer> service, BaseIndexProvider indexProvider, BaseSearchProvider searchProvider, bool enableDataModifiers)
            : this(service, indexProvider, searchProvider, new CachedInvoiceQuery(MerchelloContext.Current.Services.InvoiceService, enableDataModifiers).GetByCustomerKey, enableDataModifiers)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedCustomerQuery"/> class.
        /// </summary>
        /// <param name="service">
        /// The service.
        /// </param>
        /// <param name="indexProvider">
        /// The index provider.
        /// </param>
        /// <param name="searchProvider">
        /// The search provider.
        /// </param>
        /// <param name="getInvoicesByCustomer">
        /// The get Invoices By Customer.
        /// </param>
        /// <param name="enableDataModifiers">
        /// A value indicating whether or not data modifiers are enabled.
        /// </param>
        internal CachedCustomerQuery(IPageCachedService<ICustomer> service, BaseIndexProvider indexProvider, BaseSearchProvider searchProvider, Func<Guid, IEnumerable<InvoiceDisplay>> getInvoicesByCustomer, bool enableDataModifiers)
            : base(service, indexProvider, searchProvider, enableDataModifiers)
        {
            _customerService = (CustomerService)service;
            _getInvoiceByCustomer = getInvoicesByCustomer;
        }


        /// <summary>
        /// Gets the key field in index.
        /// </summary>
        protected override string KeyFieldInIndex
        {
            get { return "customerKey"; }
        }

        /// <summary>
        /// Gets a <see cref="CustomerDisplay"/> by it's unique key
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="CustomerDisplay"/>.
        /// </returns>        
        public override CustomerDisplay GetByKey(Guid key)
        {
            return GetDisplayObject(key);
        }

        /// <summary>
        /// Searches all customers
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort by field
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="QueryResultDisplay"/>.
        /// </returns>
        public QueryResultDisplay Search(long page, long itemsPerPage, string sortBy = "loginName", SortDirection sortDirection = SortDirection.Ascending)
        {
            return GetQueryResultDisplay(_customerService.GetPagedKeys(page, itemsPerPage, sortBy, sortDirection));
        }

        /// <summary>
        /// Searches all customers by a term
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
        public QueryResultDisplay Search(string term, long page, long itemsPerPage, string sortBy = "loginName", SortDirection sortDirection = SortDirection.Ascending)
        {
            return GetQueryResultDisplay(_customerService.GetPagedKeys(term, page, itemsPerPage, sortBy, sortDirection));
        }

        /// <summary>
        /// Searches customers that have customer dates within a specified date range
        /// </summary>
        /// <param name="lastActivityDateStart">
        /// The customer date start.
        /// </param>
        /// <param name="lastActivityDateEnd">
        /// The customer date end.
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
        public QueryResultDisplay Search(DateTime lastActivityDateStart, DateTime lastActivityDateEnd, long page, long itemsPerPage, string sortBy = "loginName", SortDirection sortDirection = SortDirection.Ascending)
        {
            var query = Query<ICustomer>.Builder.Where(x => x.LastActivityDate >= lastActivityDateStart && x.LastActivityDate <= lastActivityDateEnd);

            return GetQueryResultDisplay(_customerService.GetPagedKeys(query, page, itemsPerPage, sortBy, sortDirection));
        }

        ///// <summary>
        ///// Gets customers from a collection.
        ///// </summary>
        ///// <param name="collectionKey">
        ///// The collection key.
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
        ///// The <see cref="QueryResultDisplay"/>.
        ///// </returns>
        //public QueryResultDisplay GetFromCollection(
        //    Guid collectionKey,
        //    long page,
        //    long itemsPerPage,
        //    string sortBy = "loginName",
        //    SortDirection sortDirection = SortDirection.Ascending)
        //{
        //    var provider = this.GetEntityCollectionProvider(collectionKey);

        //    return
        //        this.GetQueryResultDisplay(provider.GetPagedEntityKeys(page, itemsPerPage, sortBy, sortDirection));
        //}

        //public QueryResultDisplay GetFromCollection(
        //    Guid collectionKey,
        //    string searchTerm,
        //    long page,
        //    long itemsPerPage,
        //    string sortBy = "",
        //    SortDirection sortDirection = SortDirection.Ascending)
        //{
        //    throw new NotImplementedException();
        //}

        ///// <summary>
        ///// The get not in collection.
        ///// </summary>
        ///// <param name="collectionKey">
        ///// The collection key.
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
        ///// The <see cref="QueryResultDisplay"/>.
        ///// </returns>
        //public QueryResultDisplay GetNotInCollection(
        //    Guid collectionKey,
        //    long page,
        //    long itemsPerPage,
        //    string sortBy = "loginName",
        //    SortDirection sortDirection = SortDirection.Ascending)
        //{
        //    return
        //        this.GetQueryResultDisplay(
        //            _customerService.GetKeysNotInCollection(collectionKey, page, itemsPerPage, sortBy, sortDirection));
        //}

        //public QueryResultDisplay GetNotInCollection(
        //    Guid collectionKey,
        //    string searchTerm,
        //    long page,
        //    long itemsPerPage,
        //    string sortBy = "",
        //    SortDirection sortDirection = SortDirection.Ascending)
        //{
        //    throw new NotImplementedException();
        //}

        /// <summary>
        /// Re-indexes the <see cref="ICustomer"/>
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        internal override void ReindexEntity(ICustomer entity)
        {
            IndexProvider.ReIndexNode(entity.SerializeToXml().Root, IndexTypes.Customer);
        }

        /// <summary>
        /// Maps a <see cref="SearchResult"/> to a <see cref="CustomerDisplay"/>
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <returns>
        /// The <see cref="CustomerDisplay"/>.
        /// </returns>
        protected override CustomerDisplay PerformMapSearchResultToDisplayObject(SearchResult result)
        {
            return result.ToCustomerDisplay(_getInvoiceByCustomer);
        }
    }
}