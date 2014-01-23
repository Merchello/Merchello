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
            if (items.ContainsKey("umbracoUrls"))
            {
                Dictionary<string, object> umbracoUrls = (Dictionary<string, object>)items["umbracoUrls"];

                UrlHelper Url = new UrlHelper(new RequestContext(new HttpContextWrapper(HttpContext.Current), new RouteData()));

                umbracoUrls.Add("merchelloProductApiBaseUrl", Url.GetUmbracoApiServiceBaseUrl<ProductApiController>(
                                                       controller => controller.GetAllProducts()));
                umbracoUrls.Add("merchelloProductVariantsApiBaseUrl", Url.GetUmbracoApiServiceBaseUrl<ProductVariantApiController>(
                                                       controller => controller.GetProductVariant(Guid.NewGuid())));
                umbracoUrls.Add("merchellSettingsApiBaseUrl", Url.GetUmbracoApiServiceBaseUrl<SettingsApiController>(
                                                       controller => controller.GetAllCountries()));
                umbracoUrls.Add("merchellWarehouseApiBaseUrl", Url.GetUmbracoApiServiceBaseUrl<WarehouseApiController>(
                                                       controller => controller.GetDefaultWarehouse()));
            }
        }
    }
}
