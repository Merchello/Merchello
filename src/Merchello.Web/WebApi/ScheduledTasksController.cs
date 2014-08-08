namespace Merchello.Web.WebApi
{
    using System;
    using System.Linq;
    using System.Web.Http;
    using Core;
    using Core.Configuration;
    using Core.Services;
    using Umbraco.Core.Logging;
    using Umbraco.Web.Mvc;
    using Umbraco.Web.WebApi;

    /// <summary>
    /// Schedule Tasks
    /// </summary>
    [PluginController("Merchello")]
    public class ScheduledTasksApiController : UmbracoApiController
    {
        /// <summary>
        /// The <see cref="IAnonymousCustomerService"/>.
        /// </summary>
        private readonly IAnonymousCustomerService _anonymousCustomerService;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduledTasksApiController"/> class. 
        /// </summary>
        public ScheduledTasksApiController()
            : this(MerchelloContext.Current)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduledTasksApiController"/> class. 
        /// </summary>
        /// <param name="merchelloContext">
        /// The <see cref="IMerchelloContext"/>
        /// </param>
        public ScheduledTasksApiController(IMerchelloContext merchelloContext)
        {
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");
            _anonymousCustomerService = ((ServiceContext)merchelloContext.Services).AnonymousCustomerService;
        }

        /// <summary>
        /// Delete all customers older than the date in the setting
        /// 
        /// GET /umbraco/Merchello/ScheduledTasksApi/RemoveAnonymousCustomers
        ///     /Umbraco/Api/ScheduledTasksApiController/RemoveAnonymousCustomers
        /// </summary>
        /// <returns>
        /// The count of anonymous customers deleted
        /// </returns>
        [AcceptVerbs("GET", "POST")]
        public int RemoveAnonymousCustomers()
        {
            int maxDays = MerchelloConfiguration.Current.AnonymousCustomersMaxDays;

            var anonymousCustomers = _anonymousCustomerService.GetAnonymousCustomersCreatedBefore(DateTime.Now.AddDays(-maxDays)).ToArray();

            _anonymousCustomerService.Delete(anonymousCustomers);

            LogHelper.Info<string>(string.Format("RemoveAnonymousCustomers - Removed Count {0}", anonymousCustomers.Count()));

            return anonymousCustomers.Count();
        }
    }
}
