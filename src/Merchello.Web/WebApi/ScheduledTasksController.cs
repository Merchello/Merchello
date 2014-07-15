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
    //[PluginController("Merchello")]
    public class ScheduledTasksApiController : UmbracoApiController
    {
        /// <summary>
        /// Delete all customers older than the date in the setting
        /// 
        /// GET /umbraco/Merchello/ScheduledTasksApi/RemoveAnonymousCustomers
        /// </summary>
        [AcceptVerbs("GET", "POST")]
        public int RemoveAnonymousCustomers()
        {
            int maxDays = MerchelloConfiguration.Current.AnonymousCustomersMaxDays;

            var repo = new AnonymousCustomerService();

            var anonymousCustomers = repo.GetAnonymousCustomersCreatedBefore(DateTime.Now); //.AddDays(-maxDays));

            repo.Delete(anonymousCustomers);

            LogHelper.Info<string>(string.Format("RemoveAnonymousCustomers - Removed Count {0}", anonymousCustomers.Count()));

            return anonymousCustomers.Count();
        }
    }
}
