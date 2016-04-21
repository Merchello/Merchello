namespace Merchello.Web.Mvc
{
    using Merchello.Core;
    using Merchello.Core.Models;
    using Merchello.Web.Pluggable;

    using Umbraco.Web.Mvc;

    /// <summary>
    /// The merchello template page.
    /// </summary>
    public abstract class MerchelloTemplatePage : UmbracoTemplatePage
    {
        /// <summary>
        /// The customer context.
        /// </summary>
        private ICustomerContext _customerContext;

        /// <summary>
        /// The MerchelloHelper class
        /// </summary>
        private MerchelloHelper _helper;

        /// <summary>
        /// Gets the CurrentCustomer from the <see cref="CustomerContext"/>
        /// </summary>
        public ICustomerBase CurrentCustomer 
        {
            get
            {
                if (_customerContext == null)
                    _customerContext = PluggableObjectHelper.GetInstance<CustomerContextBase>("CustomerContext", UmbracoContext);

                return _customerContext.CurrentCustomer;
            }
        }

        /// <summary>
        /// Gets the <see cref="MerchelloHelper"/>
        /// </summary>
        public MerchelloHelper Merchello
        {
            get
            {
                if (_helper == null)
                {
                    _helper = new MerchelloHelper();
                }

                return _helper;
            }
        }
    }
}