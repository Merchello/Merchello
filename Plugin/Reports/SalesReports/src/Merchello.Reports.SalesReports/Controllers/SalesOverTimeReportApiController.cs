namespace Merchello.Reports.SalesReports.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Web.Http;

    using Merchello.Core;
    using Merchello.Core.Services;
    using Merchello.Reports.SalesReports.Models;
    using Merchello.Web;
    using Merchello.Web.Models.ContentEditing;
    using Merchello.Web.Models.Querying;
    using Merchello.Web.Reporting;
    using Merchello.Web.Trees;

    using Umbraco.Core.Logging;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// The sales over time report controller.
    /// </summary>
    [BackOfficeTree("salesOverTime", "reports", "Sales Over Time", "icon-loading", "Merchello.SalesReports\\SalesOverTime\\report", 10)]
    [PluginController("MerchelloSalesReports")]
    public class SalesOverTimeReportApiController : ReportController
    {
        /// <summary>
        /// The store setting service.
        /// </summary>
        private readonly StoreSettingService _storeSettingService;

        /// <summary>
        /// The <see cref="MerchelloHelper"/>.
        /// </summary>
        private readonly MerchelloHelper _merchello;

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
            : base(merchelloContext)
        {
            _storeSettingService = MerchelloContext.Services.StoreSettingService as StoreSettingService;
            _merchello = new MerchelloHelper(merchelloContext.Services);
        }

        /// <summary>
        /// Gets the base url.
        /// </summary>
        public override KeyValuePair<string, object> BaseUrl
        {
            get
            {
                return GetBaseUrl<SalesOverTimeReportApiController>("merchelloReportSalesOverTime");
            }
        }

        /// <summary>
        /// The search by date range.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The <see cref="QueryResultDisplay"/>.
        /// </returns>
        [HttpPost]
        public QueryResultDisplay SearchByDateRange(QueryDisplay query)
        {
            var invoiceDateStart = query.Parameters.FirstOrDefault(x => x.FieldName == "invoiceDateStart");
            var invoiceDateEnd = query.Parameters.FirstOrDefault(x => x.FieldName == "invoiceDateEnd");

            DateTime startDate;
            DateTime endDate;
            if (invoiceDateStart == null) throw new NullReferenceException("SalesOverTimeReportApiController::SearchByDateRange: invoiceDateStart is a required parameter");

            var settings = _storeSettingService.GetAll().ToList();
            var dateFormat = settings.FirstOrDefault(s => s.Name == "dateFormat");
            if (dateFormat == null)
            {
                if (!DateTime.TryParse(invoiceDateStart.Value, out startDate)) throw new InvalidCastException("SalesOverTimeReportApiController::SearchByDateRange: Failed to convert invoiceDateStart to a valid DateTime");
            }
            else if (!DateTime.TryParseExact(invoiceDateStart.Value, dateFormat.Value, CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate))
            {
                throw new InvalidCastException("SalesOverTimeReportApiController::SearchByDateRange: Failed to convert invoiceDateStart to a valid DateTime");
            }

            endDate = invoiceDateEnd == null || dateFormat == null
                ? DateTime.MaxValue
                : DateTime.TryParseExact(invoiceDateEnd.Value, dateFormat.Value, CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate)
                    ? endDate
                    : DateTime.MaxValue;

            var invoices = _merchello.Query.Invoice.Search(
                startDate,
                endDate.AddDays(1), // through end of day
                1,
                long.MaxValue,
                query.SortBy,
                query.SortDirection);


            var result = new QueryResultDisplay();

            if (!invoices.Items.Any())
            {
                return result;
            }

            ////build list of items grouped by date. each item has "date", "salestotal", "salescount"
            var source = (from invoiceItem in invoices.Items.ToList().Cast<InvoiceDisplay>()
                         where invoiceItem.InvoiceStatus.Alias == "paid"
                         group invoiceItem by invoiceItem.InvoiceDate.Date
                         into g
                         orderby g.Key descending
                         select
                             new SalesOverTimeResult
                                 {
                                     Date = g.Key.ToString("MMMM dd, yyyy"),
                                     SalesTotal = g.Sum(item => item.Total),
                                     SalesCount = g.Count()
                                 }).ToArray();

            result.Items = source;
            result.TotalItems = source.Count();
            result.ItemsPerPage = 10;
            result.CurrentPage = 0;
            result.TotalPages = result.TotalItems / result.ItemsPerPage;

            return result;
        }

        /// <summary>
        /// The get default report data.
        /// </summary>
        /// <returns>
        /// The <see cref="QueryResultDisplay"/>.
        /// </returns>
        [HttpGet]
        public override QueryResultDisplay GetDefaultReportData()
        {
            var settings = _storeSettingService.GetAll().ToList();
            var dateFormat = settings.FirstOrDefault(s => s.Name == "dateFormat");

            var query = new QueryDisplay()
            {
                CurrentPage = 0,
                ItemsPerPage = int.MaxValue,
                Parameters = new List<QueryDisplayParameter>()
                {
                    new QueryDisplayParameter()
                        {
                        FieldName = "invoiceDateStart",
                        Value = dateFormat != null ? DateTime.Now.AddMonths(-1).ToString(dateFormat.Value) : DateTime.Now.AddMonths(-1).ToShortDateString()
                        },
                    new QueryDisplayParameter()
                        {
                            FieldName = "invoiceDateEnd",
                            Value = dateFormat != null ? DateTime.Now.ToString(dateFormat.Value) : DateTime.Now.ToShortDateString()
                        }
                },
                SortBy = "invoiceDate"
            };

            try
            {
                var results = SearchByDateRange(query);
                return results;
            }
            catch (Exception ex)
            {
                LogHelper.Error<SalesByItemReportApiController>("The system was unable to determine the default report data for the SalesOverTime report", ex);
            }

            return new QueryResultDisplay();
        }
    }
}