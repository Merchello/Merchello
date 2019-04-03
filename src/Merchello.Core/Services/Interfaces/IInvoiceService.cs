namespace Merchello.Core.Services
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Models;

    /// <summary>
    /// Defines the InvoiceService
    /// </summary>
    public interface IInvoiceService : IStaticCollectionService<IInvoice>, IPageCachedService<IInvoice>
    {
        /// <summary>
        /// Creates a <see cref="IInvoice"/> without saving it to the database
        /// </summary>
        /// <param name="invoiceStatusKey">The <see cref="IInvoiceStatus"/> key</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        /// <returns><see cref="IInvoice"/></returns>
        IInvoice CreateInvoice(Guid invoiceStatusKey, bool raiseEvents = true);

        /// <summary>
        /// Creates a <see cref="IInvoice"/> with an assigned invoice number without saving it to the database
        /// </summary>
        /// <param name="invoiceStatusKey">
        /// The <see cref="IInvoiceStatus"/> key
        /// </param>
        /// <param name="invoiceNumber">
        /// The invoice Number
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        /// <returns>
        /// <see cref="IInvoice"/>
        /// </returns>
        /// <remarks>
        /// Invoice number must be a positive integer value or zero
        /// </remarks>
        IInvoice CreateInvoice(Guid invoiceStatusKey, int invoiceNumber, bool raiseEvents = true);

        /// <summary>
        /// Saves a single <see cref="IInvoice"/>
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Save(IInvoice invoice, bool raiseEvents = true);

        /// <summary>
        /// Saves a collection of <see cref="IInvoice"/>
        /// </summary>
        /// <param name="invoices">The collection of <see cref="IInvoice"/></param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Save(IEnumerable<IInvoice> invoices, bool raiseEvents = true);
        
        /// <summary>
        /// Deletes a single <see cref="IInvoice"/>
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/> to be deleted</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Delete(IInvoice invoice, bool raiseEvents = true);

        /// <summary>
        /// Deletes a collection <see cref="IInvoice"/>
        /// </summary>
        /// <param name="invoices">The collection of <see cref="IInvoice"/> to be deleted</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Delete(IEnumerable<IInvoice> invoices, bool raiseEvents = true);

        /// <summary>
        /// Gets a <see cref="IInvoice"/> given it's unique 'key' (GUID)
        /// </summary>
        /// <param name="key">The <see cref="IInvoice"/>'s unique 'key' (GUID)</param>
        /// <returns><see cref="IInvoice"/></returns>
        IInvoice GetByKey(Guid key);

        /// <summary>
        /// Gets a <see cref="IInvoice"/> given it's unique 'InvoiceNumber'
        /// </summary>
        /// <param name="invoiceNumber">The invoice number of the <see cref="IInvoice"/> to be retrieved</param>
        /// <returns><see cref="IInvoice"/></returns>
        IInvoice GetByInvoiceNumber(int invoiceNumber);

        /// <summary>
        /// Gets list of <see cref="IInvoice"/> objects given a list of Keys
        /// </summary>
        /// <param name="keys">List of GUID 'key' for the invoices to retrieve</param>
        /// <returns>List of <see cref="IInvoice"/></returns>
        IEnumerable<IInvoice> GetByKeys(IEnumerable<Guid> keys);

        /// <summary>
        /// Gets a collection of <see cref="IInvoice"/> objects that are associated with a <see cref="IPayment"/> by the payments 'key'
        /// </summary>
        /// <param name="paymentKey">The <see cref="IPayment"/> key (GUID)</param>
        /// <returns>A collection of <see cref="IInvoice"/></returns>
        IEnumerable<IInvoice> GetInvoicesByPaymentKey(Guid paymentKey);

        /// <summary>
        /// Get invoices by a customer key.
        /// </summary>
        /// <param name="customeryKey">
        /// The customer key.
        /// </param>
        /// <returns>
        /// The collection of <see cref="IInvoice"/>.
        /// </returns>
        IEnumerable<IInvoice> GetInvoicesByCustomerKey(Guid customeryKey);

        /// <summary>
        /// Get a collection invoices by date range.
        /// </summary>
        /// <param name="startDate">
        /// The start date.
        /// </param>
        /// <param name="endDate">
        /// The end date.
        /// </param>
        /// <returns>
        /// The collection of <see cref="IInvoice"/>.
        /// </returns>
        IEnumerable<IInvoice> GetInvoicesByDateRange(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Gets the total count of all invoices
        /// </summary>
        /// <returns>
        /// The <see cref="int"/> representing the count of invoices.
        /// </returns>
        int CountInvoices();

        /// <summary>
        /// Gets the total count of all invoices within a date range.
        /// </summary>
        /// <param name="startDate">
        /// The start date.
        /// </param>
        /// <param name="endDate">
        /// The end date.
        /// </param>
        /// <returns>
        /// The <see cref="int"/> representing the count of invoices.
        /// </returns>
        int CountInvoices(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Gets the total count of all invoices within a date range.
        /// </summary>
        /// <param name="startDate">
        /// The start date.
        /// </param>
        /// <param name="endDate">
        /// The end date.
        /// </param>
        /// <param name="invoiceStatuses">
        /// The invoice statuses.
        /// </param>
        /// <returns>
        /// The <see cref="int"/> representing the count of invoices.
        /// </returns>
        int CountInvoices(DateTime startDate, DateTime endDate, IEnumerable<IInvoiceStatus> invoiceStatuses);

        /// <summary>
        /// The count invoices by customer type
        /// </summary>
        /// <param name="startDate">
        /// The start date.
        /// </param>
        /// <param name="endDate">
        /// The end date.
        /// </param>
        /// <param name="customerType">
        /// The customer type.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        int CountInvoices(DateTime startDate, DateTime endDate, CustomerType customerType);

        /// <summary>
        /// Gets the totals of invoices in a date range for a specific currency code.
        /// </summary>
        /// <param name="startDate">
        /// The start date.
        /// </param>
        /// <param name="endDate">
        /// The end date.
        /// </param>
        /// <param name="currencyCode">
        /// The currency code.
        /// </param>
        /// <returns>
        /// The sum of the invoice totals.
        /// </returns>
        decimal SumInvoiceTotals(DateTime startDate, DateTime endDate, string currencyCode);

        /// <summary>
        /// Gets the totals of invoices in a date range for a specific currency code.
        /// </summary>
        /// <param name="startDate">
        /// The start date.
        /// </param>
        /// <param name="endDate">
        /// The end date.
        /// </param>
        /// <param name="currencyCode">
        /// The currency code.
        /// </param>
        /// <param name="invoiceStatuses">
        /// The invoice statuses.
        /// </param>
        /// <returns>
        /// The sum of the invoice totals.
        /// </returns>
        decimal SumInvoiceTotals(DateTime startDate, DateTime endDate, string currencyCode, IEnumerable<IInvoiceStatus> invoiceStatuses);

        /// <summary>
        /// Gets the total of line items for a give SKU invoiced in a specific currency across the date range.
        /// </summary>
        /// <param name="startDate">
        /// The start date.
        /// </param>
        /// <param name="endDate">
        /// The end date.
        /// </param>
        /// <param name="currencyCode">
        /// The currency code.
        /// </param>
        /// <param name="sku">
        /// The SKU.
        /// </param>
        /// <returns>
        /// The total of line items for a give SKU invoiced in a specific currency across the date range.
        /// </returns>
        decimal SumLineItemTotalsBySku(DateTime startDate, DateTime endDate, string currencyCode, string sku);

        /// <summary>
        /// Gets distinct currency codes used in invoices.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{String}"/>.
        /// </returns>
        IEnumerable<string> GetDistinctCurrencyCodes();

        /// <summary>
        /// Gets a InvoiceOrderShipment structure
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        InvoiceOrderShipment GetInvoiceOrderShipment(Guid invoiceId);

        ///// <summary>
        ///// Gets a <see cref="Page{IInvoice}"/>
        ///// </summary>
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
        ///// The <see cref="Page{IInvoice}"/>.
        ///// </returns>
        //Page<IInvoice> GetPage(long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Descending);        

        #region InvoiceStatus

        /// <summary>
        /// Gets an <see cref="IInvoiceStatus"/> by it's key
        /// </summary>
        /// <param name="key">The <see cref="IInvoiceStatus"/> key</param>
        /// <returns><see cref="IInvoiceStatus"/></returns>
        IInvoiceStatus GetInvoiceStatusByKey(Guid key);

        /// <summary>
        /// Returns a collection of all <see cref="IInvoiceStatus"/>
        /// </summary>
        /// <returns>
        /// The collection of <see cref="IInvoiceStatus"/>.
        /// </returns>
        IEnumerable<IInvoiceStatus> GetAllInvoiceStatuses();


        #endregion
    }
}