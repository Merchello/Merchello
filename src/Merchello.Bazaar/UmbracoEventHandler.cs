namespace Merchello.Bazaar
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    using Merchello.Bazaar.Controllers.Api;
    using Merchello.Core;
    using Merchello.Core.Configuration;

    using Umbraco.Core;
    using Umbraco.Core.Events;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Models;
    using Umbraco.Core.Services;
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

            LogHelper.Info<UmbracoEventHandler>("Binding Merchello Customer synchronization");
            MemberService.Saved += MemberServiceOnSaved;
        }

        /// <summary>
        /// Updates the Merchello Customer's First Name and Last Name on MemberService save.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="saveEventArgs">
        /// The save event args.
        /// </param>
        private void MemberServiceOnSaved(IMemberService sender, SaveEventArgs<IMember> saveEventArgs)
        {
            var members = saveEventArgs.SavedEntities.ToArray();

            // Allowed member types for Merchello customers
            var customerMemberTypes = MerchelloConfiguration.Current.CustomerMemberTypes.ToArray();

            // Get a reference to Merchello's customer service
            var customerService = MerchelloContext.Current.Services.CustomerService;

            foreach (var member in members)
            {
                // verify the member is a customer type
                if (!customerMemberTypes.Contains(member.ContentTypeAlias)) continue;

                var customer = customerService.GetByLoginName(member.Username);
                if (customer == null) continue;

                customer.FirstName = member.GetValue<string>("firstName") ?? string.Empty;
                customer.LastName = member.GetValue<string>("lastName") ?? string.Empty;
                customer.Email = member.Username;

                customerService.Save(customer);
            }
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
            if (!e.ContainsKey("merchelloBazaarUrls"))
            e.Add(
                "merchelloBazaarUrls", 
                new Dictionary<string, object>()
                    {
                        { "merchelloBazaarPropertyEditorsApiBaseUrl", urlHelper.GetUmbracoApiServiceBaseUrl<PropertyEditorsController>(controller => controller.GetThemes()) }
                    });
        }
    }
}