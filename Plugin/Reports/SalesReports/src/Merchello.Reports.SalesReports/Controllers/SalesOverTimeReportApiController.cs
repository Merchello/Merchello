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
                             select
                                 new
                                 {
                                     date = g.Key,
                                     salestotal = g.Sum<InvoiceDisplay>((Func<InvoiceDisplay, decimal>)(item => item.Total)),
                                     salescount = g.Count<InvoiceDisplay>()


                                 };

            //    var list = new[]
            //{
            //    new {date = "12/1/2014", salestotal = 100, salescount = 3},
            //    new {date = "12/10/2014", salestotal = 33, salescount = 2},
            //    new {date = "12/15/2014", salestotal = 66, salescount = 1}
            //};

            result.Items = source;
            result.TotalItems = source.Count();
                result.ItemsPerPage = 10;
                result.CurrentPage = 0;
                result.TotalPages = result.TotalItems / result.ItemsPerPage;

                return result;

            }

        }

        /// <summary>
        /// Exports a search by date range.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The <see cref="QueryResultDisplay"/>.
        /// </returns>
        //[HttpPost]
        //public QueryResultDisplay ExportByDateRange(QueryDisplay query)
        //{
        //    var result = this.SearchByDateRange(query);

        //    var invoiceDateStart = query.Parameters.FirstOrDefault(x => x.FieldName == "invoiceDateStart");
        //    var invoiceDateEnd = query.Parameters.FirstOrDefault(x => x.FieldName == "invoiceDateEnd");

        //    if (!result.Items.Any()) return result;

        //    // check if the directory exists
        //    var exportDir = IOHelper.MapPath("~/App_Data/TEMP/Merchello/");
        //    if (!Directory.Exists(exportDir))
        //        Directory.CreateDirectory(exportDir);


        //    var dump = CsvSerializer.SerializeToCsv(result.Items);

        //    // write dump to export file
        //    var exportFileName = string.Concat("SalesOverTimeReport_", DateTime.Parse(invoiceDateStart.Value).ToString("_yyyyMMdd"), "_", DateTime.Parse(invoiceDateEnd.Value).ToString("_yyyyMMdd"), ".csv");
        //    var exportPath = string.Concat(exportDir, exportFileName);
        //    File.WriteAllText(exportPath, dump);

        //    return result;
        //}

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