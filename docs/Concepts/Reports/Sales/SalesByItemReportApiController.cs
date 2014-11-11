namespace Merchello.Web.Reporting.Sales
{
    using System.Collections.Generic;

    using Merchello.Core;
    using Merchello.Web.Models.Querying;
    using Merchello.Web.Trees;

    /// <summary>
    /// The sales by item report api controller.
    /// </summary>
    [BackOfficeTree("salesByItem", "reports", "Sales By Item", "icon-barcode", "merchello/merchello/SalesByItem/manage/", 20)]
    public class SalesByItemReportApiController : ReportController
    {
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