namespace Merchello.Reports.SalesReports.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;

    using Merchello.Core;
    using Merchello.Web;
    using Merchello.Web.Models.Querying;
    using Merchello.Web.Reporting;
    using Merchello.Web.Trees;

    /// <summary>
    /// The sales by item report API controller.
    /// </summary>
    [BackOfficeTree("salesByItem", "reports", "Sales By Item", "icon-barcode", "Merchello.SalesReports\\SalesByItem\\report", 20)]
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

        ///// <summary>
        ///// The search by date range.
        ///// </summary>
        ///// <param name="query">
        ///// The query.
        ///// </param>
        ///// <returns>
        ///// The <see cref="QueryResultDisplay"/>.
        ///// </returns>
        //[HttpPost]
        //public QueryResultDisplay SearchByDateRange(QueryDisplay query)
        //{
        //    var invoiceDateStart = query.Parameters.FirstOrDefault(x => x.FieldName == "invoiceDateStart");
        //    var invoiceDateEnd = query.Parameters.FirstOrDefault(x => x.FieldName == "invoiceDateEnd");

        //    DateTime startDate;
        //    DateTime endDate;
        //    if (invoiceDateStart  == null) throw new NullReferenceException("invoiceDateStart is a required parameter"); );
        //    if(!DateTime.TryParse(invoiceDateStart.Value, out startDate)) throw new InvalidCastException("Failed to convert invoiceDateStart to a valid DateTime");

        //    endDate = invoiceDateEnd == null
        //        ? DateTime.MaxValue
        //        : DateTime.TryParse(invoiceDateEnd.Value, out endDate)
        //            ? endDate
        //            : DateTime.MaxValue;

        //    var invoices = _merchello.Query.Invoice.Search(
        //        startDate,
        //        endDate,
        //        1,
        //        long.MaxValue,
        //        query.SortBy,
        //        query.SortDirection);

           
        //}

        /// <summary>
        /// The get default report data.
        /// </summary>
        /// <returns>
        /// The <see cref="QueryResultDisplay"/>.
        /// </returns>
        public override QueryResultDisplay GetDefaultReportData()
        {
            throw new System.NotImplementedException();
        }
    }
}