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

namespace Merchello.Web.Editors
{
    [PluginController("MerchelloApi")]
    public class CustomerController : MerchelloApiController
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public CustomerController()
            : this(MerchelloContext.Current)
        {            
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="merchelloContext"></param>
        public CustomerController(MerchelloContext merchelloContext)
            : base(merchelloContext)
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
