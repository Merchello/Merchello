namespace Merchello.Web.Editors.Reports
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;

    using Merchello.Core;
    using Merchello.Core.Models.TypeFields;
    using Merchello.Core.Services;
    using Merchello.Web.Models.ContentEditing;
    using Merchello.Web.Models.Querying;
    using Merchello.Web.Models.Reports;

    using Umbraco.Core.Persistence;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// API controller responsible for rendering the sales by item report.
    /// </summary>
    [PluginController("Merchello")]
    public class SalesByItemReportApiController : MonthlyReportApiControllerBase<IEnumerable<SalesByItemResult>>
    {
        /// <summary>
        /// The <see cref="IInvoiceService"/>.
        /// </summary>
        private readonly IInvoiceService _invoiceService;

        /// <summary>
        /// The <see cref="MerchelloHelper"/>.
        /// </summary>
        private readonly MerchelloHelper _merchello;

        /// <summary>
        /// The product line item type field key.
        /// </summary>
        private readonly Guid _productLineItemTfKey = EnumTypeFieldConverter.LineItemType.Product.TypeKey;

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

            _invoiceService = merchelloContext.Services.InvoiceService;
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
        /// Gets the default report data for initial page load.
        /// </summary>
        /// <returns>
        /// The <see cref="QueryResultDisplay"/>.
        /// </returns>
        [HttpGet]
        public override QueryResultDisplay GetDefaultReportData()
        {
            var today = DateTime.Today;
            var endOfMonth = GetEndOfMonth(today);
            var startMonth = endOfMonth.AddMonths(-11);
            var startOfYear = GetFirstOfMonth(startMonth);

            return BuildResult(startOfYear, endOfMonth);
        }

        /// <summary>
        /// Builds the result set for the report.
        /// </summary>
        /// <param name="startDate">
        /// The start date.
        /// </param>
        /// <param name="endDate">
        /// The end date.
        /// </param>
        /// <returns>
        /// The <see cref="QueryResultDisplay"/>.
        /// </returns>
        protected override QueryResultDisplay BuildResult(DateTime startDate, DateTime endDate)
        {
            var currentDate = startDate;
            var results = this.GetResults(startDate, currentDate).ToArray();

            return new QueryResultDisplay()
            {
                Items = results,
                CurrentPage = 1,
                ItemsPerPage = results.Count(),
                TotalItems = results.Count(),
                TotalPages = 1
            };
        }

        /// <summary>
        /// Performs the actual work of querying for the results.
        /// </summary>
        /// <param name="startDate">
        /// The start date.
        /// </param>
        /// <param name="endDate">
        /// The end date.
        /// </param>
        /// <returns>
        /// The typed result.
        /// </returns>
        protected override IEnumerable<SalesByItemResult> GetResults(DateTime startDate, DateTime endDate)
        {
            // determine the top 5 items within the date range
            var database = ApplicationContext.DatabaseContext.Database;

            // We're using an internal helper here so we can keep all of our report SQL queries in a single location
            // in case we want to refactor to some sort of service at a later date.
            var sql = ReportSqlHelper.SalesByItem.GetSkuSaleCountSql(startDate, endDate, _productLineItemTfKey);

            var dtos = database.Fetch<SkuSaleCountDto>(sql);

            // there have not been any item sales in this period
            if (!dtos.Any()) return Enumerable.Empty<SalesByItemResult>();

            var results = new List<SalesByItemResult>();

            foreach (var dto in dtos)
            {
                var variant = GetProductVariant(dto.Sku);

                if (variant != null)
                {
                    var result = new SalesByItemResult()
                        {
                            ProductVariant = variant,
                            Quantity = dto.SaleCount,
                            Totals = this.ActiveCurrencies.Select(c => new ResultCurrencyValue()
                            {
                                Currency = c.ToCurrencyDisplay(),
                                Value = this._invoiceService.SumLineItemTotalsBySku(startDate, endDate, c.CurrencyCode, dto.Sku)
                            }).ToList()
                        };

                    results.Add(result);
                }
            }

            return results;
        }

        /// <summary>
        /// Safely queries for the product.
        /// </summary>
        /// <param name="sku">
        /// The SKU.
        /// </param>
        /// <returns>
        /// The <see cref="ProductVariantDisplay"/>.
        /// </returns>
        private ProductVariantDisplay GetProductVariant(string sku)
        {
            var variant = _merchello.Query.Product.GetProductVariantBySku(sku);

            if (variant != null) return variant;

            var product = this._merchello.Query.Product.GetBySku(sku);

            return product != null ? product.AsMasterVariantDisplay() : null;
        }

        /// <summary>
        /// Small POCO for the internal query.
        /// </summary>
        private class SkuSaleCountDto
        {
            /// <summary>
            /// Gets or sets the SKU.
            /// </summary>
            [Column("sku")]
            public string Sku { get; set; }

            /// <summary>
            /// Gets or sets the sale count.
            /// </summary>
            [Column("saleCount")]
            public long SaleCount { get; set; }
        }
    }
}