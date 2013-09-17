using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Umbraco.Web.Mvc;
using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Web.WebApi;
using Umbraco.Web;

namespace Merchello.Web.Editors
{
    [PluginController("Merchello")]
    public class ProductApiController : MerchelloApiController
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ProductApiController()
            : this(MerchelloContext.Current)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="merchelloContext"></param>
        public ProductApiController(MerchelloContext merchelloContext)
            : base(merchelloContext)
        {
        }

        /// <summary>
        /// This is a helper contructor for unit testing
        /// </summary>
        internal ProductApiController(MerchelloContext merchelloContext, UmbracoContext umbracoContext)
            : base(merchelloContext, umbracoContext)
        {
        }

        /// <summary>
        /// Returns all Product by Key
        /// </summary>
        /// <param name="key"></param>
        public Product GetProduct(Guid key)
        {
            //var customer = MerchelloContext.Services. as Product;
            //if (customer == null)
            //{
            //    throw new HttpResponseException(HttpStatusCode.NotFound);
            //}

            //return customer;
            return null;
        }
    }
}
