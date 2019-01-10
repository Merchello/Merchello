using Merchello.Core;
using Merchello.Web.Models.Querying;
using Merchello.Web.Models.Reports;
using Merchello.Web.Trees;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Umbraco.Web.Mvc;
using System.Globalization;
using Merchello.Core.Services;
using Merchello.Web.Reporting;
using Umbraco.Core.Services;
using Umbraco.Web;
using Merchello.Core.Models.TypeFields;

namespace Merchello.Web.Editors.Reports
{
    /// <summary>
    ///     API Controller responsible for the Sales Search Report
    /// </summary>
    [BackOfficeTree("salesSearch", "reports", "Sales Search", "icon-loading",
    "/app_plugins/merchello/backoffice/merchello/salessearch.html", 1)]
    [PluginController("Merchello")]
    public class SalesSearchReportApiController : ReportController
    {
        #region Privates
        /// <summary>
        /// The <see cref="CultureInfo"/>.
        /// </summary>
        private readonly CultureInfo _culture;

        /// <summary>
        /// The <see cref="IInvoiceService"/>.
        /// </summary>
        private readonly IInvoiceService _invoiceService;

        /// <summary>
        /// The <see cref="MerchelloHelper"/>.
        /// </summary>
        private readonly MerchelloHelper _merchello;

        /// <summary>
        /// The text service.
        /// </summary>
        private readonly ILocalizedTextService _textService;

        /// <summary>
        /// The product line item type field key.
        /// </summary>
        private readonly Guid _productLineItemTfKey = EnumTypeFieldConverter.LineItemType.Product.TypeKey;

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SalesByItemReportApiController"/> class.
        /// </summary>
        public SalesSearchReportApiController()
            : this(Core.MerchelloContext.Current)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SalesSearchReportApiController" /> class
        /// </summary>
        /// <param name="merchelloContext"></param>
        public SalesSearchReportApiController(IMerchelloContext merchelloContext) : base(merchelloContext)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SalesSearchReportApiController"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="umbracoContext">
        /// The umbraco Context.
        /// </param>
        public SalesSearchReportApiController(IMerchelloContext merchelloContext, UmbracoContext umbracoContext)
            : base(merchelloContext)
        {
            _culture = LocalizationHelper.GetCultureFromUser(umbracoContext.Security.CurrentUser);

            _invoiceService = merchelloContext.Services.InvoiceService;

            _merchello = new MerchelloHelper();

            _textService = umbracoContext.Application.Services.TextService;
        } 
        #endregion

        /// <summary>
        ///  Gets the initial data for the report
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public SalesSearchSnapshot GetInitialData()
        {
            return new SalesSearchSnapshot
            {
              ProductKey = Guid.NewGuid(),
              ProductName = "Poo Face"
            };
        }

        /// <summary>
        ///     Gets the default report data.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public override QueryResultDisplay GetDefaultReportData()
        {
            // Bit hacky. Don't need this but have to have it
            return new QueryResultDisplay
            {
                Items = Enumerable.Empty<SalesSearchSnapshot>(),
                CurrentPage = 1,
                ItemsPerPage = 100,
                TotalItems = 0,
                TotalPages = 1
            };
        }


        /// <summary>
        ///     Registers the controller in Merchello's Angular routing
        /// </summary>
        public override KeyValuePair<string, object> BaseUrl => GetBaseUrl<SalesSearchReportApiController>("merchelloSalesSearchBaseUrl");

    }
}
