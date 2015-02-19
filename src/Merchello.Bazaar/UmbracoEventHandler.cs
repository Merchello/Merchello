namespace Merchello.Bazaar
{
    using System;
    using System.Collections.Generic;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    using Merchello.Bazaar.Controllers;

    using Umbraco.Core;
    using Umbraco.Web;
    using Umbraco.Web.UI.JavaScript;

    /// <summary>
    /// The umbraco event handler.
    /// </summary>
    public class UmbracoEventHandler : ApplicationEventHandler
    {
        /// <summary>
        /// Handles Umbraco Events.
        /// </summary>
        /// <param name="umbracoApplication">
        /// The <see cref="UmbracoApplicationBase"/>.
        /// </param>
        /// <param name="applicationContext">
        /// Umbraco <see cref="ApplicationContext"/>.
        /// </param>
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            ServerVariablesParser.Parsing += this.ServerVariablesParserOnParsing;
        }

        /// <summary>
        /// The server variables parser on parsing.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The dictionary.
        /// </param>
        private void ServerVariablesParserOnParsing(object sender, Dictionary<string, object> e)
        {
            if (HttpContext.Current == null) throw new InvalidOperationException("HttpContext is null");

            var urlHelper = new UrlHelper(new RequestContext(new HttpContextWrapper(HttpContext.Current), new RouteData()));

            e.Add(
                "merchello.bazaar", 
                new Dictionary<string, object>()
                    {
                        { "merchelloBazaarPropertyEditorsApiBaseUrl", urlHelper.GetUmbracoApiServiceBaseUrl<PropertyEditorsController>(controller => controller.GetThemes()) }
                    });
        }
    }
}