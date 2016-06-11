﻿namespace Merchello.FastTrack.Ui
{
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Configuration;
    using Merchello.Web.Models.VirtualContent;

    using Umbraco.Core;
    using Umbraco.Core.Services;

    /// <summary>
    /// Registers Umbraco event handlers.
    /// </summary>
    public class UmbracoEventHandler : IApplicationEventHandler
    {
        /// <summary>
        /// Handles Umbraco Initialized Event.
        /// </summary>
        /// <param name="umbracoApplication">
        /// The <see cref="UmbracoApplicationBase"/>.
        /// </param>
        /// <param name="applicationContext">
        /// Umbraco <see cref="ApplicationContext"/>.
        /// </param>
        public void OnApplicationInitialized(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
        }

        /// <summary>
        /// Handles Umbraco Starting.
        /// </summary>
        /// <param name="umbracoApplication">
        /// The <see cref="UmbracoApplicationBase"/>.
        /// </param>
        /// <param name="applicationContext">
        /// Umbraco <see cref="ApplicationContext"/>.
        /// </param>
        public void OnApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            // We handle the Initializing event so that we can set the parent node of the virtual content to the store
            // so that published content queries in views will work correctly
            ProductContentFactory.Initializing += ProductContentFactoryOnInitializing;

            MemberService.Saved += MemberServiceSaved;
        }

        /// <summary>
        /// Handles Umbraco Started.
        /// </summary>
        /// <param name="umbracoApplication">
        /// The <see cref="UmbracoApplicationBase"/>.
        /// </param>
        /// <param name="applicationContext">
        /// Umbraco <see cref="ApplicationContext"/>.
        /// </param>
        public void OnApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
        }

        //// Event handler methods

        /// <summary>
        /// Handles the <see cref="ProductContentFactory"/> on initializing event.
        /// </summary>
        /// <param name="sender">
        /// The <see cref="ProductContentFactory"/>.
        /// </param>
        /// <param name="e">
        /// The <see cref="VirtualContentEventArgs"/>.
        /// </param>
        /// <remarks>
        /// This is required to set the parent id of the virtual content
        /// </remarks>
        private static void ProductContentFactoryOnInitializing(ProductContentFactory sender, VirtualContentEventArgs e)
        {
            var store = ExampleUiHelper.Content.GetStoreRoot();
            e.Parent = store;
        }

        /// <summary>
        /// Copies first name, last name and email address from saved member to Merchello customer.
        /// </summary>
        /// <param name="sender">
        /// The <see cref="IMemberService"/>.
        /// </param>
        /// <param name="e">
        /// The saved <see cref="IMember"/>s.
        /// </param>
        private static void MemberServiceSaved(IMemberService sender, Umbraco.Core.Events.SaveEventArgs<Umbraco.Core.Models.IMember> e)
        {
            var members = e.SavedEntities.ToArray();

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
    }
}