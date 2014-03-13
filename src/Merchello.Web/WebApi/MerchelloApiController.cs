using System;
using Merchello.Core;
using Newtonsoft.Json.Serialization;
using Umbraco.Web;
using Umbraco.Web.Editors;
using Umbraco.Web.Mvc;


namespace Merchello.Web.WebApi
{
    [PluginController("Merchello")]
    [JsonCamelCaseFormatter]
    public abstract class MerchelloApiController : UmbracoAuthorizedJsonController
    {
        protected MerchelloApiController()
            : this(MerchelloContext.Current)
        {

        }

        protected MerchelloApiController(MerchelloContext merchelloContext) : this(merchelloContext, UmbracoContext.Current)
        {
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");
            
            MerchelloContext = merchelloContext;
            InstanceId = Guid.NewGuid();
        }

        protected MerchelloApiController(MerchelloContext merchelloContext, UmbracoContext umbracoContext) : base(umbracoContext)
        {
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");

            MerchelloContext = merchelloContext;
            InstanceId = Guid.NewGuid();
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
