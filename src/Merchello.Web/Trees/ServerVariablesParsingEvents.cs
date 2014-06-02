using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Web;
using Umbraco.Web.UI.JavaScript;
using Merchello.Web.Editors;
using System;

namespace Merchello.Web.Trees
{
    /// <summary>
    /// Merchello Angular Services Routing
    /// </summary>
    // TODO Move this to the Merchello WebBootManager
    public class ServerVariablesParsingEvents : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            base.ApplicationStarted(umbracoApplication, applicationContext);

            LogHelper.Info<ServerVariablesParsingEvents>("Initializing Merchello ServerVariablesParsingEvents");

            ServerVariablesParser.Parsing += ServerVariablesParserParsing;
        }
                
        private static void ServerVariablesParserParsing(object sender, Dictionary<string, object> items)
        {
            if (!items.ContainsKey("umbracoUrls")) return;

            var umbracoUrls = (Dictionary<string, object>)items["umbracoUrls"];

            var url = new UrlHelper(new RequestContext(new HttpContextWrapper(HttpContext.Current), new RouteData()));

            umbracoUrls.Add("merchelloProductApiBaseUrl", url.GetUmbracoApiServiceBaseUrl<ProductApiController>(
                controller => controller.GetAllProducts()));
            umbracoUrls.Add("merchelloProductVariantsApiBaseUrl", url.GetUmbracoApiServiceBaseUrl<ProductVariantApiController>(
                controller => controller.GetProductVariant(Guid.NewGuid())));
            umbracoUrls.Add("merchelloSettingsApiBaseUrl", url.GetUmbracoApiServiceBaseUrl<SettingsApiController>(
                controller => controller.GetAllCountries()));
            umbracoUrls.Add("merchelloWarehouseApiBaseUrl", url.GetUmbracoApiServiceBaseUrl<WarehouseApiController>(
                controller => controller.GetDefaultWarehouse()));
            umbracoUrls.Add("merchelloCatalogShippingApiBaseUrl", url.GetUmbracoApiServiceBaseUrl<ShippingGatewayApiController>(
                controller => controller.GetShipCountry(Guid.NewGuid())));
            umbracoUrls.Add("merchelloCatalogFixedRateShippingApiBaseUrl", url.GetUmbracoApiServiceBaseUrl<CatalogFixedRateShippingApiController>(
                controller => controller.GetAllShipCountryFixedRateProviders(Guid.NewGuid())));
            umbracoUrls.Add("merchelloNotificationApiBaseUrl", url.GetUmbracoApiServiceBaseUrl<NotificationGatewayApiController>(
                controller => controller.GetAllGatewayProviders()));
            umbracoUrls.Add("merchelloPaymentGatewayApiBaseUrl", url.GetUmbracoApiServiceBaseUrl<PaymentGatewayApiController>(
                controller => controller.GetAllGatewayProviders()));
            umbracoUrls.Add("merchelloTaxationGatewayApiBaseUrl", url.GetUmbracoApiServiceBaseUrl<TaxationGatewayApiController>(
                controller => controller.GetAllGatewayProviders()));
            umbracoUrls.Add("merchelloInvoiceApiBaseUrl", url.GetUmbracoApiServiceBaseUrl<InvoiceApiController>(
                controller => controller.GetAllInvoices()));
            umbracoUrls.Add("merchelloOrderApiBaseUrl", url.GetUmbracoApiServiceBaseUrl<OrderApiController>(
                controller => controller.GetOrder(Guid.NewGuid())));
            umbracoUrls.Add("merchelloShipmentApiBaseUrl", url.GetUmbracoApiServiceBaseUrl<ShipmentApiController>(
                controller => controller.GetShipment(Guid.NewGuid())));
            umbracoUrls.Add("merchelloPaymentApiBaseUrl", url.GetUmbracoApiServiceBaseUrl<PaymentApiController>(
                controller => controller.GetPayment(Guid.NewGuid())));            
            umbracoUrls.Add("merchelloGatewayProviderApiBaseUrl", url.GetUmbracoApiServiceBaseUrl<GatewayProviderApiController>(
                controller => controller.GetGatewayProvider(Guid.NewGuid())));
        }
    }
}
