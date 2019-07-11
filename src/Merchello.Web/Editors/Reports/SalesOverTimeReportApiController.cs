namespace Merchello.Web.Editors.Reports
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Web.Http;

    using Merchello.Core;
    using Merchello.Core.Services;
    using Merchello.Web.Models.ContentEditing;
    using Merchello.Web.Models.Querying;
    using Merchello.Web.Models.Reports;

    using Umbraco.Core.Services;
    using Umbraco.Web;
    using Umbraco.Web.Mvc;
    using Merchello.Core.Models;
    using Newtonsoft.Json;

    /// <summary>
    /// A controller for rendering the sales over time report.
    /// </summary>
    [PluginController("Merchello")]
    public class SalesOverTimeReportApiController : MonthlyReportApiControllerBase<SalesOverTimeResult>
    {
        /// <summary>
        /// The <see cref="CultureInfo"/>.
        /// </summary>
        private readonly CultureInfo _culture;

        /// <summary>
        /// The <see cref="IInvoiceService"/>.
        /// </summary>
        private readonly IInvoiceService _invoiceService;

        /// <summary>
        /// The text service.
        /// </summary>
        private readonly ILocalizedTextService _textService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SalesOverTimeReportApiController"/> class.
        /// </summary>
        public SalesOverTimeReportApiController()
            : this(Core.MerchelloContext.Current)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SalesOverTimeReportApiController"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        public SalesOverTimeReportApiController(IMerchelloContext merchelloContext)
            : this(merchelloContext, UmbracoContext.Current)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SalesOverTimeReportApiController"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The <see cref="IMerchelloContext"/>.
        /// </param>
        /// <param name="umbracoContext">
        /// The umbraco Context.
        /// </param>
        public SalesOverTimeReportApiController(IMerchelloContext merchelloContext, UmbracoContext umbracoContext)
            : base(merchelloContext)
        {
            _culture = LocalizationHelper.GetCultureFromUser(umbracoContext.Security.CurrentUser);

            _invoiceService = merchelloContext.Services.InvoiceService;

            _textService = umbracoContext.Application.Services.TextService;
        }

        /// <summary>
        /// Gets the base url definition for Server Variables Parsing.
        /// </summary>
        public override KeyValuePair<string, object> BaseUrl
        {
            get
            {
                return GetBaseUrl<SalesOverTimeReportApiController>("merchelloSalesOverTimeApiBaseUrl");
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
            var today = DateTime.Today;
            var endOfMonth = GetEndOfMonth(today);
            var startMonth = endOfMonth.AddMonths(-11);
            var startOfYear = GetFirstOfMonth(startMonth);
            var invoiceStatuses = AllInvoiceStatuses();

            return BuildResult(startOfYear, endOfMonth, invoiceStatuses);
        }

        /// <summary>
        /// Builds the report data for a custom date range
        /// </summary>
        /// <param name="query">A <see cref="QueryResultDisplay"/> containing the date range</param>
        /// <returns>The <see cref="QueryResultDisplay"/></returns>
        [HttpPost]
        public override QueryResultDisplay GetCustomDateRange(QueryDisplay query)
        {
            var invoiceDateStart = query.Parameters.FirstOrDefault(x => x.FieldName == "invoiceDateStart");
            var invoiceDateEnd = query.Parameters.FirstOrDefault(x => x.FieldName == "invoiceDateEnd");
            var invoiceStatuses = GetInvoiceStatusesFromParameters(query);

            var isDateSearch = invoiceDateStart != null && !string.IsNullOrEmpty(invoiceDateStart.Value) &&
               invoiceDateEnd != null && !string.IsNullOrEmpty(invoiceDateEnd.Value);

            if (!isDateSearch) return GetDefaultReportData();

            DateTime startDate;

            //// Assert the start date
            if (DateTime.TryParse(invoiceDateStart.Value, out startDate))
            {
                DateTime endDate;
                //// Assert the end date
                if (DateTime.TryParse(invoiceDateEnd.Value, out endDate))
                {
                    //// Return the default report if startDate >= endDate
                    if (startDate >= endDate) return GetDefaultReportData();

                    var endOfMonth = GetEndOfMonth(endDate);
                    var startOfYear = GetFirstOfMonth(startDate);

                    return BuildResult(startOfYear, endOfMonth, invoiceStatuses);
                }

                return GetDefaultReportData();
            }

            return GetDefaultReportData();
        }

        /// <summary>
        /// Gets the weekly results.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The <see cref="QueryResultDisplay"/>.
        /// </returns>
        [HttpPost]
        public QueryResultDisplay GetWeeklyResult(QueryDisplay query)
        {
            var invoiceDateEnd = query.Parameters.FirstOrDefault(x => x.FieldName == "invoiceDateEnd");
            var isDateSearch = invoiceDateEnd != null && !string.IsNullOrEmpty(invoiceDateEnd.Value);
            var invoiceStatuses = GetInvoiceStatusesFromParameters(query);

            DateTime weekEnding;
            if (!isDateSearch)
            {
                weekEnding = DateTime.Today;
            }
            else
            {
                if (!DateTime.TryParse(invoiceDateEnd.Value, out weekEnding)) weekEnding = DateTime.Today;
            }

            var weekStarting = weekEnding.StartOfWeek();
            weekEnding = weekStarting.AddDays(6);


            var count = 0;
            var results = new List<SalesOverTimeResult>();
            var currentDate = weekStarting;
            while (currentDate <= weekEnding)
            {
                var endDate = currentDate.AddDays(1).AddMilliseconds(-1);

                count++;
                results.Add(GetResults(currentDate, endDate, invoiceStatuses));
                currentDate = currentDate.AddDays(1);
            }

            return new QueryResultDisplay()
            {
                Items = results,
                CurrentPage = 1,
                ItemsPerPage = count,
                TotalItems = count,
                TotalPages = 1,
                InvoiceStatuses = invoiceStatuses
            };
        }

        /// <summary>
        /// Builds the result set for the report.
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
        /// The <see cref="QueryResultDisplay"/>.
        /// </returns>
        protected override QueryResultDisplay BuildResult(DateTime startDate, DateTime endDate, IEnumerable<InvStatus> invoiceStatuses)
        {
            // Get all the statuses
            var statuses = AllInvoiceStatuses();
            var selectedStatuses = invoiceStatuses.Where(x => x.Checked).Select(x => x.Key).ToList();

            // Loop and set selected statuses
            foreach (var status in statuses)
            {
                status.Checked = selectedStatuses.Contains(status.Key);
            }

            var count = 0;

            var currentDate = startDate;
            var results = new List<SalesOverTimeResult>();

            while (currentDate <= endDate)
            {
                var monthEnd = currentDate.AddMonths(1).AddMilliseconds(-1);
                count++;
                results.Add(this.GetResults(currentDate, monthEnd, statuses));
                currentDate = currentDate.AddMonths(1);
            }

            return new QueryResultDisplay()
            {
                Items = results,
                CurrentPage = 1,
                ItemsPerPage = count,
                TotalItems = count,
                TotalPages = 1,
                InvoiceStatuses = statuses
            };
        }

        /// <summary>
        /// Performs the actual work of querying for the results.
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
        /// The <see cref="SalesOverTimeResult"/>.
        /// </returns>
        protected override SalesOverTimeResult GetResults(DateTime startDate, DateTime endDate, IEnumerable<InvStatus> invoiceStatuses)
        {
            var monthName = _textService.GetLocalizedMonthName(_culture, startDate.Month);
            var checkedStatuses = invoiceStatuses.Where(x => x.Checked).Select(x => new InvoiceStatus { Key = x.Key }).ToList();

            var count = 0;
            if (checkedStatuses != null && checkedStatuses.Any())
            {
                count = _invoiceService.CountInvoices(startDate, endDate, checkedStatuses.Select(x => new InvoiceStatus { Key = x.Key }).ToList());
            }
            else
            {
                count = _invoiceService.CountInvoices(startDate, endDate);
            }

            var totals = this.ActiveCurrencies.Select(c => new ResultCurrencyValue()
            {
                Currency = c.ToCurrencyDisplay(),
                Value = startDate > DateTime.Today ? 0M : checkedStatuses != null && checkedStatuses.Any() ? _invoiceService.SumInvoiceTotals(startDate, endDate, c.CurrencyCode, checkedStatuses) : _invoiceService.SumInvoiceTotals(startDate, endDate, c.CurrencyCode)
            }).ToList();

            return new SalesOverTimeResult()
            {
                StartDate = startDate,
                EndDate = endDate,
                Month = monthName,
                Year = startDate.Year.ToString(),
                SalesCount = count,
                Totals = totals
            };
        }
    }
}