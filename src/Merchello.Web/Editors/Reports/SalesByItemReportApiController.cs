namespace Merchello.Web.Editors.Reports
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    using Merchello.Core;
    using Merchello.Core.Services;
    using Merchello.Web.Models.Querying;
    using Merchello.Web.Reporting;

    using Umbraco.Web.Mvc;

    /// <summary>
    /// API controller responsible for rendering the sales by item report.
    /// </summary>
    [PluginController("Merchello")]
    public class SalesByItemReportApiController : ReportController
    {
        /// <summary>
        /// The <see cref="CultureInfo"/>.
        /// </summary>
        private readonly CultureInfo _culture;

        /// <summary>
        /// The store setting service.
        /// </summary>
        private readonly StoreSettingService _storeSettingService;

        /// <summary>
        /// The <see cref="MerchelloHelper"/>.
        /// </summary>
        private readonly MerchelloHelper _merchello;

        /// <summary>
        /// Initializes a new instance of the <see cref="SalesByItemReportApiController"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        public SalesByItemReportApiController(IMerchelloContext merchelloContext)
            : base(merchelloContext)
        {
            _storeSettingService = MerchelloContext.Services.StoreSettingService as StoreSettingService;
            _merchello = new MerchelloHelper(merchelloContext.Services);
        }

        /// <summary>
        /// Gets the base url definition for Server Variables Parsing.
        /// </summary>
        public override KeyValuePair<string, object> BaseUrl
        {
            get
            {
                return GetBaseUrl<SalesByItemReportApiController>("merchelloSalesByItemApiBaseUrl");
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