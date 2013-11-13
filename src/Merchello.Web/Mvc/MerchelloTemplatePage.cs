using Merchello.Core;
using Merchello.Core.Models;
using Umbraco.Web.Mvc;

namespace Merchello.Web.Mvc
{
    public abstract class MerchelloTemplatePage : UmbracoTemplatePage
    {

        private CustomerContext _customerContext;       

        /// <summary>
        /// Gets the CurrentCustomer from the CustomerContext
        /// </summary>
        public ICustomerBase CurrentCustomer {
            get
            {
                if(_customerContext == null)
                    _customerContext = new CustomerContext(MerchelloContext.Current, UmbracoContext);

                return _customerContext.CurrentCustomer;
            }
        }


        /// <summary>
        /// The MerchelloHelper class
        /// </summary>
        private MerchelloHelper _helper;

        public MerchelloHelper Merchello
        {
            get { return _helper ?? (_helper = new MerchelloHelper()); }
        }
    }
}