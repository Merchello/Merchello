namespace Merchello.Web.WebApi
{
    using System;
    using Merchello.Core;
    using Umbraco.Core;
    using Umbraco.Core.Models.Membership;
    using Umbraco.Core.Security;
    using Umbraco.Web;
    using Umbraco.Web.Editors;
    

    /// <summary>
    /// The base Merchello back office API controller.
    /// </summary>
    [JsonCamelCaseFormatter]
    public abstract class MerchelloApiController : UmbracoAuthorizedJsonController
    {
        private IUser _currentUser;

        /// <summary>
        /// Initializes a new instance of the <see cref="MerchelloApiController"/> class.
        /// </summary>
        protected MerchelloApiController()
            : this(Core.MerchelloContext.Current)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MerchelloApiController"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        protected MerchelloApiController(IMerchelloContext merchelloContext) : this(merchelloContext, UmbracoContext.Current)
        {
            Ensure.ParameterNotNull(merchelloContext, "merchelloContext");
            
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
        protected MerchelloApiController(IMerchelloContext merchelloContext, UmbracoContext umbracoContext) : base(umbracoContext)
        {
            Ensure.ParameterNotNull(merchelloContext, "merchelloContext");

            MerchelloContext = merchelloContext;
            InstanceId = Guid.NewGuid();
        }
   
        /// <summary>
        /// Gets the current <see cref="IMerchelloContext"/>
        /// </summary>
        public IMerchelloContext MerchelloContext { get; private set; }

        /// <summary>
        /// Gets the instance id
        /// </summary>
        /// <remarks>
        /// Useful for debugging
        /// </remarks>
        internal Guid InstanceId { get; private set; }

        /// <summary>
        /// Gets the current backend user
        /// </summary>
        public IUser CurrentUser
        {
            get
            {
                if (_currentUser == null)
                {
                    // Get the user who 
                    var userTicket = new System.Web.HttpContextWrapper(System.Web.HttpContext.Current).GetUmbracoAuthTicket();
                    if (userTicket != null)
                    {
                        _currentUser = ApplicationContext.Services.UserService.GetByUsername(userTicket.Name);
                    }
                }
                return _currentUser;
            }
        }
    }
}
