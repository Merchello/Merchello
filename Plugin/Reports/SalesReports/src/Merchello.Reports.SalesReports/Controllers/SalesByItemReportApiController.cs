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

    using Umbraco.Core.IO;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// The sales by item report API controller.
    /// </summary>
    [BackOfficeTree("salesByItem", "reports", "Sales By Item", "icon-barcode", "Merchello.SalesReports\\SalesByItem\\report", 20)]
    [PluginController("MerchelloSalesReports")]
    public class SalesByItemReportApiController : ReportController
    {
        /// <summary>
        /// The <see cref="MerchelloHelper"/>.
        /// </summary>
        private readonly MerchelloHelper _merchello;

        /// <summary>
        /// Initializes a new instance of the <see cref="SalesByItemReportApiController"/> class.
        /// </summary>
        public SalesByItemReportApiController()
            : this(Core.MerchelloContext.Current)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SalesByItemReportApiController"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        public SalesByItemReportApiController(IMerchelloContext merchelloContext)
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
                return GetBaseUrl<SalesByItemReportApiController>("merchelloReportSalesByItem");
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
            if (invoiceDateStart  == null) throw new NullReferenceException("invoiceDateStart is a required parameter");
            if (!DateTime.TryParse(invoiceDateStart.Value, out startDate)) throw new InvalidCastException("Failed to convert invoiceDateStart to a valid DateTime");

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

            if (!invoices.Items.Any()) return result;

            // Use a visitor to build the collection of report data
            var vistor = new SalesByItemVisitor(_merchello);
           
            foreach (var invoice in invoices.Items)
            {
                ((InvoiceDisplay)invoice).Accept(vistor);
            }

            result.TotalItems = vistor.Results.Count();
            result.ItemsPerPage = vistor.Results.Count();
            result.CurrentPage = 0;
            result.TotalPages = 1;
            result.Items = vistor.Results;

            return result;
        }

        ///// <summary>
        ///// Exports a search by date range.
        ///// </summary>
        ///// <param name="query">
        ///// The query.
        ///// </param>
        ///// <returns>
        ///// The <see cref="QueryResultDisplay"/>.
        ///// </returns>
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
        //    var exportFileName = string.Concat("SalesByItemReport_", DateTime.Parse(invoiceDateStart.Value).ToString("_yyyyMMdd"), "_", DateTime.Parse(invoiceDateEnd.Value).ToString("_yyyyMMdd"), ".csv");
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
                                    Value = DateTime.Now.AddMonths(-1).ToShortDateString()
                                    },
                                new QueryDisplayParameter()
                                    {
                                        FieldName = "invoiceDateEnd",
                                        Value = DateTime.Now.ToShortDateString()
                                    }
                            },
                            SortBy = "invoiceDate"
                        };

            return SearchByDateRange(query);
        }
    }
}