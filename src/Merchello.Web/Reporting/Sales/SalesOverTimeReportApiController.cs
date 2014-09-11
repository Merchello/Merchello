namespace Merchello.Web.Reporting.Sales
{
    using System.Collections.Generic;

    using Merchello.Core;
    using Merchello.Web.Models.Querying;
    using Merchello.Web.Trees;

    /// <summary>
    /// The sales over time report controller.
    /// </summary>
    [BackOfficeTree("salesOverTime", "reports", "Sales Over Time", "icon-loading", "merchello/merchello/SalesOverTime/manage/", 10)]
    public class SalesOverTimeReportApiController : ReportController
    {
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