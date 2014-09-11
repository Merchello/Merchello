namespace Merchello.Web.Trees
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Editors;
    using Models.ContentEditing;
    using Models.Querying;
    using Reporting;
    using Umbraco.Core;
    using Umbraco.Core.Logging;
    using Umbraco.Web;
    using Umbraco.Web.UI.JavaScript;

    /// <summary>
    /// Merchello Angular Services Routing
    /// </summary>
    /// <remarks>
    /// TODO Move this to the Merchello WebBootManager
    /// </remarks>
    public class ServerVariablesParsingEvents : ApplicationEventHandler
    {
        /// <summary>
        /// The application started.
        /// </summary>
        /// <param name="umbracoApplication">
        /// The umbraco application.
        /// </param>
        /// <param name="applicationContext">
        /// The application context.
        /// </param>
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            base.ApplicationStarted(umbracoApplication, applicationContext);

            LogHelper.Info<ServerVariablesParsingEvents>("Initializing Merchello ServerVariablesParsingEvents");

            ServerVariablesParser.Parsing += ServerVariablesParserParsing;
        }

        /// <summary>
        /// The server variables parser parsing.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="items">
        /// The items.
        /// </param>
        private static void ServerVariablesParserParsing(object sender, Dictionary<string, object> items)
        {
            if (!items.ContainsKey("umbracoUrls")) return;

            var umbracoUrls = (Dictionary<string, object>)items["umbracoUrls"];

            var url = new UrlHelper(new RequestContext(new HttpContextWrapper(HttpContext.Current), new RouteData()));            

            umbracoUrls.Add(
                "merchelloAuditLogApiBaseUrl",
                url.GetUmbracoApiServiceBaseUrl<AuditLogApiController>(
                    controller => controller.GetSalesHistoryByInvoiceKey(Guid.Empty)));

            umbracoUrls.Add(
                "merchelloProductApiBaseUrl", 
                url.GetUmbracoApiServiceBaseUrl<ProductApiController>(
                controller => controller.SearchProducts(new QueryDisplay()
                    {
                        CurrentPage = 0,
                        ItemsPerPage = 100,
                        Parameters = new QueryDisplayParameter[] { }
                    })));

            //umbracoUrls.Add(
            //    "merchelloProductVariantsApiBaseUrl", 
            //    url.GetUmbracoApiServiceBaseUrl<ProductVariantApiController>(
            //    controller => controller.GetProductVariant(Guid.NewGuid())));

            umbracoUrls.Add(
                "merchelloCustomerApiBaseUrl",
                url.GetUmbracoApiServiceBaseUrl<CustomerApiController>(
                controller => controller.SearchCustomers(new QueryDisplay()
                    {
                        CurrentPage = 0,
                        ItemsPerPage = 100,
                        Parameters = new QueryDisplayParameter[] { }
                    })));

            umbracoUrls.Add(
                "merchelloSettingsApiBaseUrl", 
                url.GetUmbracoApiServiceBaseUrl<SettingsApiController>(
                controller => controller.GetAllCountries()));

            umbracoUrls.Add(
                "merchelloWarehouseApiBaseUrl", 
                url.GetUmbracoApiServiceBaseUrl<WarehouseApiController>(
                controller => controller.GetDefaultWarehouse()));

            umbracoUrls.Add(
                "merchelloCatalogShippingApiBaseUrl", 
                url.GetUmbracoApiServiceBaseUrl<ShippingGatewayApiController>(
                controller => controller.GetShipCountry(Guid.NewGuid())));
            
            umbracoUrls.Add(
                "merchelloNotificationApiBaseUrl", 
                url.GetUmbracoApiServiceBaseUrl<NotificationGatewayApiController>(
                controller => controller.GetAllGatewayProviders()));

            umbracoUrls.Add(
                "merchelloPaymentGatewayApiBaseUrl", 
                url.GetUmbracoApiServiceBaseUrl<PaymentGatewayApiController>(
                controller => controller.GetAllGatewayProviders()));

            umbracoUrls.Add(
                "merchelloTaxationGatewayApiBaseUrl", 
                url.GetUmbracoApiServiceBaseUrl<TaxationGatewayApiController>(
                controller => controller.GetAllGatewayProviders()));

            umbracoUrls.Add(
                "merchelloInvoiceApiBaseUrl", 
                url.GetUmbracoApiServiceBaseUrl<InvoiceApiController>(
                controller => controller.SearchInvoices(new QueryDisplay()
                {
                    CurrentPage = 0,
                    ItemsPerPage = 100,
                    Parameters = new QueryDisplayParameter[] { }
                })));

            umbracoUrls.Add(
                "merchelloOrderApiBaseUrl", 
                url.GetUmbracoApiServiceBaseUrl<OrderApiController>(
                controller => controller.GetOrder(Guid.NewGuid())));

            umbracoUrls.Add(
                "merchelloShipmentApiBaseUrl",
                url.GetUmbracoApiServiceBaseUrl<ShipmentApiController>(
                controller => controller.GetShipment(Guid.NewGuid())));

            umbracoUrls.Add(
                "merchelloRateTableApiBaseUrl",
                url.GetUmbracoApiServiceBaseUrl<FixedRateShippingApiController>(
                controller => controller.GetShipFixedRateTable(new ShipMethodDisplay())));

            umbracoUrls.Add(
                "merchelloPaymentApiBaseUrl", 
                url.GetUmbracoApiServiceBaseUrl<PaymentApiController>(
                controller => controller.GetPayment(Guid.NewGuid())));
            
            umbracoUrls.Add(
                "merchelloGatewayProviderApiBaseUrl", 
                url.GetUmbracoApiServiceBaseUrl<GatewayProviderApiController>(
                controller => controller.GetGatewayProvider(Guid.NewGuid())));

            if (!ReportApiControllerResolver.HasCurrent) return;

            foreach (var keyValue in ReportApiControllerResolver.Current.GetAll().Select(reportController => reportController.BaseUrl))
            {
                umbracoUrls.Add(keyValue.Key, keyValue.Value);
            }
        }
    }
}
