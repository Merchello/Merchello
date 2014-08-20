namespace Merchello.Web
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Core;
    using log4net;

    using Merchello.Core.Configuration;
    using Merchello.Core.Models;

    using Umbraco.Core;
    using Umbraco.Core.Events;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Models;
    using Umbraco.Core.Services;

    /// <summary>
    /// Handles the Umbraco Application "Starting" and "Started" event and initiates the Merchello startup
    /// </summary>
    public class UmbracoApplicationEventHandler : ApplicationEventHandler
    {
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// The Umbraco Application Starting event.
        /// </summary>
        /// <param name="umbracoApplication">
        /// The umbraco application.
        /// </param>
        /// <param name="applicationContext">
        /// The application context.
        /// </param>
        protected override void ApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            base.ApplicationStarting(umbracoApplication, applicationContext);

            // Initialize Merchello
            Log.Info("Attempting to initialize Merchello");
            try
            {
                MerchelloBootstrapper.Init(new WebBootManager());
                Log.Info("Initialization of Merchello complete");                
            }
            catch (Exception ex)
            {
                Log.Error("Initialization of Merchello failed", ex);
            }
        }

        /// <summary>
        /// The Umbraco Application Starting event.
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

            LogHelper.Info<UmbracoApplicationEventHandler>("Initializing Customer related events");

            MemberService.Saving += this.MemberServiceOnSaving;
        }

        /// <summary>
        /// Handles the member saving event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="saveEventArgs">
        /// The save event args.
        /// </param>
        /// <remarks>
        /// Merchello customers are associated with Umbraco members by their username.  If the 
        /// member changes their username we have to update the association.
        /// </remarks>
        private void MemberServiceOnSaving(IMemberService sender, SaveEventArgs<IMember> saveEventArgs)
        {
            foreach (var member in saveEventArgs.SavedEntities)
            {
                if (MerchelloConfiguration.Current.CustomerMemberTypes.Any(x => x == member.ContentTypeAlias))
                {
                    var original = ApplicationContext.Current.Services.MemberService.GetByKey(member.Key);

                    var customerService = MerchelloContext.Current.Services.CustomerService;

                    ICustomer customer;
                    if (original == null)
                    {
                        // assert there is not already a customer with the login name
                        customer = customerService.GetByLoginName(member.Username);

                        if (customer != null)
                        {
                            LogHelper.Info<UmbracoApplicationEventHandler>("A customer already exists with the loginName of: " + member.Username + " -- ABORTING customer creation");
                            return;
                        }

                        customerService.CreateCustomerWithKey(member.Username, string.Empty, string.Empty, member.Email);

                        return;
                    }

                    if (original.Username == member.Username && original.Email == member.Email) return;

                    customer = customerService.GetByLoginName(original.Username);

                    if (customer == null) return;

                    ((Customer)customer).LoginName = member.Username;
                    customer.Email = member.Email;

                    customerService.Save(customer);
                }
            }
        }
    }
}
