namespace Merchello.Web.Editors
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;

    using Merchello.Core;
    using Merchello.Core.Services;
    using Merchello.Web.Models.ContentEditing;
    using Merchello.Web.WebApi;

    using Umbraco.Core.Services;
    using Umbraco.Web;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// The customer API controller.
    /// </summary>
    [PluginController("Merchello")]
    public class CustomerApiController : MerchelloApiController
    {
        #region Fields

        /// <summary>
        /// The customer service.
        /// </summary>
        private ICustomerService _customerService;

        /// <summary>
        /// The membership member service.
        /// </summary>
        private IMemberService _memberService;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerApiController"/> class.
        /// </summary>
        public CustomerApiController()
            : this(MerchelloContext.Current, global::Umbraco.Core.ApplicationContext.Current.Services.MemberService)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerApiController"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="memberService">
        /// The member Service.
        /// </param>
        public CustomerApiController(IMerchelloContext merchelloContext, IMemberService memberService)
            : base((MerchelloContext)merchelloContext)
        {
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");
            Mandate.ParameterNotNull(memberService, "memberService");

            _customerService = merchelloContext.Services.CustomerService;
            _memberService = memberService;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerApiController"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="umbracoContext">
        /// The umbraco context.
        /// </param>
        /// <param name="memberService">
        /// The member Service.
        /// </param>
        internal CustomerApiController(IMerchelloContext merchelloContext, UmbracoContext umbracoContext, IMemberService memberService)
            : base((MerchelloContext)merchelloContext, umbracoContext)
        {
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");
            Mandate.ParameterNotNull(memberService, "memberService");

            _customerService = merchelloContext.Services.CustomerService;
            _memberService = memberService;
        }

        #endregion

        /// <summary>
        /// The get all customers.
        /// </summary>
        /// <returns>
        /// The collection of all customers
        /// </returns>
        [HttpGet]
        public IEnumerable<CustomerDisplay> GetAllCustomers()
        {
            // TODO - merchello helper
            return ((CustomerService)_customerService).GetAll().Select(x => x.ToCustomerDisplay());
        }
    }
}