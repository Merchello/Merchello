namespace Merchello.Web.Editors.Reports
{
    using System;
    using System.Collections.Generic;
    using System.Web.Http;

    using Merchello.Core;
    using Merchello.Core.Configuration;
    using Merchello.Core.Services;
    using Merchello.Web.Models.Querying;
    using Merchello.Web.Models.Reports;
    using Merchello.Web.Reporting;

    /// <summary>
    /// The controller responsible for rendering the abandoned basket report.
    /// </summary>
    public class AbandonedBasketReportApiController : ReportController
    {
        /// <summary>
        /// The item cache service.
        /// </summary>
        private readonly IItemCacheService _itemCacheService;

        /// <summary>
        /// The invoice service.
        /// </summary>
        private readonly IInvoiceService _invoiceService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbandonedBasketReportApiController"/> class.
        /// </summary>
        public AbandonedBasketReportApiController()
            : this(Core.MerchelloContext.Current)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AbandonedBasketReportApiController"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        public AbandonedBasketReportApiController(IMerchelloContext merchelloContext)
            : base(merchelloContext)
        {
            _itemCacheService = merchelloContext.Services.ItemCacheService;

            _invoiceService = merchelloContext.Services.InvoiceService;
        }

        /// <summary>
        /// Gets the base url definition for Server Variables Parsing.
        /// </summary>
        public override KeyValuePair<string, object> BaseUrl
        {
            get
            {
                return GetBaseUrl<AbandonedBasketReportApiController>("merchelloAbandonedBasketApiBaseUrl");
            }
        }

        /// <summary>
        /// Gets the default report data for initial page load.
        /// </summary>
        /// <returns>
        /// The <see cref="QueryResultDisplay"/>.
        /// </returns>
        [HttpGet]
        public override QueryResultDisplay GetDefaultReportData()
        {
            const ItemCacheType ItemCacheType = ItemCacheType.Basket;

            var maxDays = MerchelloConfiguration.Current.AnonymousCustomersMaxDays;
            var startDate = DateTime.Today.AddDays(-maxDays);
            var endDate = DateTime.Today;

            var anonymousBasketCount = _itemCacheService.Count(ItemCacheType, CustomerType.Anonymous, startDate, endDate);
            var anonymousCheckoutCount = _invoiceService.CountInvoices(startDate, endDate, CustomerType.Anonymous);
            var customerBasketCount = _itemCacheService.Count(ItemCacheType, CustomerType.Customer, startDate, endDate);
            var customerCheckoutCount = _invoiceService.CountInvoices(startDate, endDate, CustomerType.Customer);


            var result = new QueryResultDisplay()
                             {
                                 TotalItems = 1,
                                 TotalPages = 1,
                                 CurrentPage = 1,
                                 ItemsPerPage = 1,
                                 Items = new[]
                                    {
                                        new AbandonedBasketResult()
                                            {
                                                ConfiguredDays = maxDays,
                                                StartDate = startDate,
                                                EndDate = endDate,
                                                AnonymousBasketCount = anonymousBasketCount,
                                                AnonymousCheckoutCount = anonymousCheckoutCount,
                                                AnonymousCheckoutPercent = GetCheckoutPercent(anonymousBasketCount, anonymousCheckoutCount),
                                                CustomerBasketCount = customerBasketCount,
                                                CustomerCheckoutCount = customerCheckoutCount,
                                                CustomerCheckoutPercent = GetCheckoutPercent(customerBasketCount, customerCheckoutCount)
                                            }
                                    }
                             };

            return result;
        }

        /// <summary>
        /// Calculates the checkout percentage.
        /// </summary>
        /// <param name="basketCount">
        /// The basket count.
        /// </param>
        /// <param name="checkoutCount">
        /// The checkout count.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        private decimal GetCheckoutPercent(int basketCount, int checkoutCount)
        {
            if (basketCount + checkoutCount == 0) return 0;
            return (checkoutCount / (decimal)(basketCount + checkoutCount)) * 100;
        }
    }
}