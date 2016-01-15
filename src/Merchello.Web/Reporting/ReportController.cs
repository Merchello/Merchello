namespace Merchello.Web.Reporting
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    using Merchello.Core;
    using Merchello.Core.Models;
    using Merchello.Core.Services;
    using Merchello.Web.Models.ContentEditing;
    using Merchello.Web.Models.Querying;

    using Umbraco.Core.Cache;
    using Umbraco.Web;

    using WebApi;

    /// <summary>
    /// Represents a report controller.
    /// </summary>
    public abstract class ReportController : MerchelloApiController
    {
        /// <summary>
        /// The Gets a list of active currency codes.
        /// </summary>
        private Lazy<IEnumerable<ICurrency>> _activeCurrencies;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportController"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        protected ReportController(IMerchelloContext merchelloContext)
            : base(merchelloContext)
        {
            Initialize();
        }

        /// <summary>
        /// Gets the list of currency codes used in invoices.
        /// </summary>
        public virtual IEnumerable<ICurrency> ActiveCurrencies
        {
            get
            {
                return _activeCurrencies.Value;
            }
        }

        /// <summary>
        /// Gets the runtime cache.
        /// </summary>
        public virtual IRuntimeCacheProvider RuntimeCache
        {
            get
            {
                return MerchelloContext.Cache.RuntimeCache;
            }
        }

        /// <summary>
        /// Gets the base url.
        /// </summary>
        public abstract KeyValuePair<string, object> BaseUrl { get; }

        /// <summary>
        /// The get default report.
        /// </summary>
        /// <returns>
        /// The <see cref="QueryResultDisplay"/>.
        /// </returns>
        public abstract QueryResultDisplay GetDefaultReportData();

        /// <summary>
        /// Utility method to create the base url.
        /// </summary>
        /// <param name="routeName">
        /// The route name.
        /// </param>
        /// <typeparam name="T">
        /// The controller type
        /// </typeparam>
        /// <returns>
        /// The key value pair
        /// </returns>
        protected static KeyValuePair<string, object> GetBaseUrl<T>(string routeName) where T : ReportController
        {
            var url = new UrlHelper(new RequestContext(new HttpContextWrapper(HttpContext.Current), new RouteData()));
            return new KeyValuePair<string, object>(
                routeName,
                url.GetUmbracoApiServiceBaseUrl<T>(controller => controller.GetDefaultReportData()));
        }



        /// <summary>
        /// The initializes the controller.
        /// </summary>
        private void Initialize()
        {
            
            _activeCurrencies =
                new Lazy<IEnumerable<ICurrency>>(
                    () =>
                        {
                            var currencyCodes = MerchelloContext.Services.InvoiceService.GetDistinctCurrencyCodes().ToList();
                            if (!currencyCodes.Any())
                            {
                                var code =
                                    ((InvoiceService)MerchelloContext.Services.InvoiceService).GetDefaultCurrencyCode();
                                currencyCodes.Add(code);
                            }
                            return currencyCodes.Select(c => this.MerchelloContext.Services.StoreSettingService.GetCurrencyByCode(c)).ToList();
                        });
        }
    }
}