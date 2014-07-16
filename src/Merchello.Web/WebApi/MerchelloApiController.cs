namespace Merchello.Web.WebApi
{
    using System;
    using Merchello.Core;
    using Umbraco.Web;
    using Umbraco.Web.Editors;

    /// <summary>
    /// The base Merchello back office API controller.
    /// </summary>
    [JsonCamelCaseFormatter]
    public abstract class MerchelloApiController : UmbracoAuthorizedJsonController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MerchelloApiController"/> class.
        /// </summary>
        protected MerchelloApiController()
            : this(MerchelloContext.Current)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MerchelloApiController"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        protected MerchelloApiController(MerchelloContext merchelloContext) : this(merchelloContext, UmbracoContext.Current)
        {
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");
            
            MerchelloContext = merchelloContext;
            InstanceId = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MerchelloApiController"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="umbracoContext">
        /// The umbraco context.
        /// </param>
        protected MerchelloApiController(MerchelloContext merchelloContext, UmbracoContext umbracoContext) : base(umbracoContext)
        {
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");

            MerchelloContext = merchelloContext;
            InstanceId = Guid.NewGuid();
        }
   
        /// <summary>
        /// Gets the current <see cref="IMerchelloContext"/>
        /// </summary>
        public MerchelloContext MerchelloContext { get; private set; }

        /// <summary>
        /// Gets the instance id
        /// </summary>
        /// <remarks>
        /// Useful for debugging
        /// </remarks>
        internal Guid InstanceId { get; private set; }
    }
}
