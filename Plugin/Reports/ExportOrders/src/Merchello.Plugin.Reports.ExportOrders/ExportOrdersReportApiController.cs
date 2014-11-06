namespace Merchello.Plugin.Reports.ExportOrders
{
    using System.Collections.Generic;

    using Merchello.Core;
    using Merchello.Web.Models.Querying;
    using Merchello.Web.Trees;
    using Merchello.Web.Reporting;

    /// <summary>
    /// The sales over time report controller.
    /// </summary>
    [BackOfficeTree("exportOrders", "reports", "Export Orders", "icon-download", "/merchello/merchello/ViewReport/Merchello.ExportOrders|ExportOrders", 10)]
    public class ExportOrdersReportApiController : ReportController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExportOrdersReportApiController"/> class.
        /// </summary>
        public ExportOrdersReportApiController()
            : this(Core.MerchelloContext.Current)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportOrdersReportApiController"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        public ExportOrdersReportApiController(IMerchelloContext merchelloContext)
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
                return GetBaseUrl<ExportOrdersReportApiController>("merchelloReportExportOrders");
            }
        }

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