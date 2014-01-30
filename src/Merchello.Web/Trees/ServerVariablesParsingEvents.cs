using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Umbraco.Core;
using Umbraco.Web;
using Umbraco.Web.UI.JavaScript;
using Merchello.Web.Editors;
using System;

namespace Merchello.Web.Trees
{
    public class ServerVariablesParsingEvents : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            base.ApplicationStarted(umbracoApplication, applicationContext);

            ServerVariablesParser.Parsing += ServerVariablesParserParsing;
        }
        
        
        static void ServerVariablesParserParsing(object sender, Dictionary<string, object> items)
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
        }
    }
}
