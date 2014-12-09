namespace Merchello.Reports.SalesReports.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Web.Http;

    using ICSharpCode.SharpZipLib.Zip;

    using Merchello.Core;
    using Merchello.Reports.SalesReports.Visitors;
    using Merchello.Web;
    using Merchello.Web.Models.ContentEditing;
    using Merchello.Web.Models.Querying;
    using Merchello.Web.Reporting;
    using Merchello.Web.Trees;

    using ServiceStack.Text;

    using Umbraco.Core.IO;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// The sales over time report controller.
    /// </summary>
    [BackOfficeTree("salesOverTime", "reports", "Sales Over Time", "icon-loading", "Merchello.SalesReports\\SalesOverTime\\report", 10)]
    [PluginController("MerchelloSalesReports")]
    public class SalesOverTimeReportApiController : ReportController
    {
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
        [HttpGet]
        public QueryResultDisplay SearchByDateRange(QueryDisplay query)
        {
            var invoiceDateStart = query.Parameters.FirstOrDefault(x => x.FieldName == "invoiceDateStart");
            var invoiceDateEnd = query.Parameters.FirstOrDefault(x => x.FieldName == "invoiceDateEnd");

            DateTime startDate;
            DateTime endDate;
            if (invoiceDateStart == null) throw new NullReferenceException("SalesOverTimeReportApiController::SearchByDateRange: invoiceDateStart is a required parameter");
            if (!DateTime.TryParse(invoiceDateStart.Value, out startDate)) throw new InvalidCastException("SalesOverTimeReportApiController::SearchByDateRange: Failed to convert invoiceDateStart to a valid DateTime");

            endDate = invoiceDateEnd == null
                ? DateTime.MaxValue
                : DateTime.TryParse(invoiceDateEnd.Value, out endDate)
                    ? endDate
                    : DateTime.MaxValue;

            var invoices = _merchello.Query.Invoice.Search(
                startDate,
                endDate,
                1,
                long.MaxValue,
                query.SortBy,
                query.SortDirection);


            var result = new QueryResultDisplay();

            if (!invoices.Items.Any())
            {
                return result;
            }
            else
            {
                //build list of items grouped by date. each item has "date", "salestotal", "salescount"
                var source = from invoiceItem in invoices.Items.ToList().Cast<InvoiceDisplay>()
                             where invoiceItem.InvoiceStatus.Name == "Paid"
                             group invoiceItem by invoiceItem.InvoiceDate.Date
                                 into g
                                 orderby g.Key descending
                                 select
                                     new
                                     {
                                         date = g.Key.ToString("MMMM dd, yyyy"),
                                         salestotal = g.Sum<InvoiceDisplay>((Func<InvoiceDisplay, decimal>)(item => item.Total)),
                                         salescount = g.Count<InvoiceDisplay>()


                                     };

                result.Items = source;
                result.TotalItems = source.Count();
                result.ItemsPerPage = 10;
                result.CurrentPage = 0;
                result.TotalPages = result.TotalItems / result.ItemsPerPage;

                return result;

            }

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
            var query = new QueryDisplay()
                        {
                            CurrentPage = 0,
                            ItemsPerPage = int.MaxValue,
                            Parameters = new List<QueryDisplayParameter>()
                            {
                                new QueryDisplayParameter()
                                    {
                                    FieldName = "invoiceDateStart",
                                    Value = DateTime.Now.AddMonths(-10).ToShortDateString()
                                    },
                                new QueryDisplayParameter()
                                    {
                                        FieldName = "invoiceDateEnd",
                                        Value = DateTime.Now.AddMonths(10).ToShortDateString()
                                    }
                            },
                            SortBy = "invoiceDate"
                        };

            return SearchByDateRange(query);
        }
    }


}