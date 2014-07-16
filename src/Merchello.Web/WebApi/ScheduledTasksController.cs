using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Linq;
using Umbraco.Web.Mvc;
using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Core.Configuration;
using Merchello.Web.WebApi;
using Umbraco.Web;
using Merchello.Web.Models.ContentEditing;
using Examine;
using Umbraco.Web.WebApi;
using Umbraco.Core.Logging;


namespace Merchello.Web.WebApi
{
    /// <summary>
    /// Schedule Tasks
    /// </summary>
    [PluginController("Merchello")]
    public class ScheduledTasksApiController : UmbracoApiController
    {
        private readonly IAnonymousCustomerService _anonymousCustomerService;
        
        /// <summary>
        /// Constructor
        /// </summary>
        public ScheduledTasksApiController()
            : this(MerchelloContext.Current)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="merchelloContext"></param>
        public ScheduledTasksApiController(MerchelloContext merchelloContext)
            : base()
        {
            _anonymousCustomerService = ((ServiceContext)merchelloContext.Services).AnonymousCustomerService;
        }

        /// <summary>
        /// Delete all customers older than the date in the setting
        /// 
        /// GET /umbraco/Merchello/ScheduledTasksApi/RemoveAnonymousCustomers
        ///     /Umbraco/Api/ScheduledTasksApiController/RemoveAnonymousCustomers
        /// </summary>
        [AcceptVerbs("GET", "POST")]
        public int RemoveAnonymousCustomers()
        {
            int maxDays = MerchelloConfiguration.Current.AnonymousCustomersMaxDays;

            var anonymousCustomers = _anonymousCustomerService.GetAnonymousCustomersCreatedBefore(DateTime.Now.AddDays(-maxDays));

            _anonymousCustomerService.Delete(anonymousCustomers);

            LogHelper.Info<string>(string.Format("RemoveAnonymousCustomers - Removed Count {0}", anonymousCustomers.Count()));

            return anonymousCustomers.Count();
        }
    }
}
