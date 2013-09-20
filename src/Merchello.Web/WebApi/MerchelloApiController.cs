using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Merchello.Core;
using Merchello.Core.Services;
using Newtonsoft.Json.Serialization;
using Umbraco.Web;
using Umbraco.Web.WebApi;

namespace Merchello.Web.WebApi
{
    public abstract class MerchelloApiController : UmbracoApiController //  : UmbracoAuthorizedJsonController we will eventually want to inherit this
    {
        protected MerchelloApiController()
            : this(MerchelloContext.Current)
        {

        }

        protected MerchelloApiController(MerchelloContext merchelloContext) : this(merchelloContext, UmbracoContext.Current)
        {
            if (merchelloContext == null) throw new ArgumentNullException("merchelloContext");
            MerchelloContext = merchelloContext;
            InstanceId = Guid.NewGuid();
        }

        protected MerchelloApiController(MerchelloContext merchelloContext, UmbracoContext umbracoContext) : base(umbracoContext)
        {
            if (merchelloContext == null) throw new ArgumentNullException("merchelloContext");
            MerchelloContext = merchelloContext;
            InstanceId = Guid.NewGuid();
        }

        /// <summary>
        /// Removes the xml formatter and configure the camel casing
        /// </summary>
        /// <param name="controllerContext"></param>
        protected override void Initialize(global::System.Web.Http.Controllers.HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);

            // remove the XmlFormatter 
            // TODO : this becomes redundant when we inherit from UmbracoAuthorizedJsonController
            controllerContext.Configuration.Formatters.Remove(controllerContext.Configuration.Formatters.XmlFormatter);
            
            // camel casing
            controllerContext.Configuration.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            
        }

        /// <summary>
        /// Returns the current MerchelloContext
        /// </summary>
        public MerchelloContext MerchelloContext { get; private set; }

        /// <summary>
        /// Useful for debugging
        /// </summary>
        internal Guid InstanceId { get; private set; }
    }
}
