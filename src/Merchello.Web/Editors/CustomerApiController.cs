using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Mvc;
using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Web.WebApi;
using Umbraco.Web;

namespace Merchello.Web.Editors
{
    [PluginController("Merchello")]
    public class CustomerApiController : MerchelloApiController
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public CustomerApiController()
            : this(MerchelloContext.Current)
        {            
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="merchelloContext"></param>
        public CustomerApiController(MerchelloContext merchelloContext)
            : base(merchelloContext)
        {
        }

        /// <summary>
        /// This is a helper contructor for unit testing
        /// </summary>
        internal CustomerApiController(MerchelloContext merchelloContext, UmbracoContext umbracoContext)
            : base(merchelloContext, umbracoContext)
        {
        }





        /// <summary>
        /// Returns all Customers
        /// </summary>
        /// <param name="contentId"></param>
        public ICustomer GetCustomer(Guid key)
        {
            return MerchelloContext.Services.CustomerService.GetByKey(key);
        }
    }
}
