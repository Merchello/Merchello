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
using Merchello.Core.Models;
using Umbraco.Core.Persistence;
using Merchello.Core.Logging;

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
        /// List of statuses
        /// </summary>
        private IEnumerable<InvStatus> _invStatuses;

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
        public SalesSearchReportApiController(IMerchelloContext merchelloContext)
            : this(merchelloContext, UmbracoContext.Current)
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
            var today = DateTime.Today;
            var endOfMonth = today.EndOfMonth();
            var startMonth = today.FirstOfMonth();
            //var startMonth = new DateTime(2016, 1, 1).FirstOfMonth();
            var invoiceStatuses = AllStatuses();

            return BuildSalesSearchSnapshot(startMonth, endOfMonth, invoiceStatuses.Select(x => x.Key), string.Empty);
        }

        /// <summary>
        /// Updates the report form
        /// </summary>
        /// <param name="salesSearchSnapshot"></param>
        /// <returns></returns>
        [HttpPost]
        public SalesSearchSnapshot UpdateData(SalesSearchSnapshot salesSearchSnapshot)
        {
            return BuildSalesSearchSnapshot(salesSearchSnapshot.StartDate, salesSearchSnapshot.EndDate, salesSearchSnapshot.InvoiceStatuses.Where(x => x.Checked).Select(x => x.Key), salesSearchSnapshot.Search);
        }

        private IEnumerable<InvStatus> AllStatuses()
        {
            if (_invStatuses == null)
            {
                _invStatuses = ApplicationContext.DatabaseContext.Database.Query<InvStatus>("SELECT pk, name FROM merchInvoiceStatus").ToList();
            }
            return _invStatuses;
        }

        /// <summary>
        /// Builds the sales snapshot 
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="invoiceStatuses"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        private SalesSearchSnapshot BuildSalesSearchSnapshot(DateTime startDate, DateTime endDate, IEnumerable<Guid> invoiceStatuses, string search)
        {
            // Get all the statuses
            var statuses = AllStatuses();

            // Loop and set selected statuses
            foreach (var status in statuses)
            {
                if (invoiceStatuses.Contains(status.Key))
                {
                    status.Checked = true;
                }
            }

            // Get the SQL
            var sql = ReportSqlHelper.SalesByItem.GetSaleSearchSql(startDate, endDate, invoiceStatuses, search);

            // Execure the SQL
            var results = ApplicationContext.DatabaseContext.Database.Query<SaleItem>(sql).ToList();

            // Group results by the product key
            var groupedResults = results.GroupBy(x => x.ExtendedData.GetProductKey());

            // List of ProductLineItem to add
            var ProductLineItemList = new List<ProductLineItem>();

            var currencySymbol = this.ActiveCurrencies.FirstOrDefault();

            // Loop each product
            foreach (var productGroup in groupedResults)
            {
                // We do a try as the product may be deleted and not exist anymore
                try
                {
                    // Get the base/master product (We need it for the name)
                    var product = _merchello.Query.Product.GetByKey(productGroup.Key);
                    if (product != null)
                    {
                        var productLineItem = new ProductLineItem
                        {
                            Name = product.Name,
                            Quantity = productGroup.Sum(x => x.Quantity),
                            Variants = new List<ProductLineItem>(),
                            CurrencySymbol = currencySymbol.Symbol
                        };

                        // Get the correct total
                        decimal productGroupTotal = 0;
                        foreach (var p in productGroup) {
                            productGroupTotal += p.Price * p.Quantity;
                        }
                        productLineItem.Total = productGroupTotal;

                        foreach (var variants in productGroup.GroupBy(x => x.Name))
                        {
                            var variantLineItem = new ProductLineItem
                            {
                                Name = variants.FirstOrDefault().Name,
                                Quantity = variants.Sum(x => x.Quantity),
                                CurrencySymbol = currencySymbol.Symbol
                            };

                            // Get the correct total
                            decimal productVariantTotal = 0;
                            foreach (var v in variants)
                            {
                                productVariantTotal += v.Price * v.Quantity;
                            }
                            variantLineItem.Total = productVariantTotal;

                            productLineItem.Variants.Add(variantLineItem);
                        }

                        ProductLineItemList.Add(productLineItem);
                    }
                }
                catch (Exception ex)
                {
                    MultiLogHelper.Error<SalesSearchReportApiController>("Error in BuildSalesSearchSnapshot", ex);
                }
            }

            // Make final model
            var salesSearchSnapshot = new SalesSearchSnapshot
            {
                EndDate = endDate,
                Search = search,
                StartDate = startDate,
                InvoiceStatuses = statuses,
                Products = ProductLineItemList.OrderByDescending(x => x.Total)
            };

            // return
            return salesSearchSnapshot;
        }

        #region Overrides

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
        public override KeyValuePair<string, object> BaseUrl
        {
            get
            {
                return GetBaseUrl<SalesSearchReportApiController>("merchelloSalesSearchBaseUrl");
            }
        }

        #endregion

        #region Helper Classes

        private class SaleItem
        {
            private ExtendedDataCollection extendedData;

            [Column("name")]
            public string Name { get; set; }

            [Column("quantity")]
            public int Quantity { get; set; }

            [Column("price")]
            public decimal Price { get; set; }

            [Column("extendedData")]
            public string ExDataString { get; set; }

            [Ignore]
            public ExtendedDataCollection ExtendedData
            {
                get
                {
                    if (extendedData == null)
                    {
                        extendedData = new ExtendedDataCollection(ExDataString);
                    }
                    return extendedData;
                }
            }
        }

        #endregion
    }
}
