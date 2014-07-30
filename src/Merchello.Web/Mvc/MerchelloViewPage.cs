namespace Merchello.Web.Mvc
{
    using Merchello.Core;
    using Merchello.Core.Models;

    /// <summary>
    /// The merchello view page.
    /// </summary>
    /// <typeparam name="TModel">
    /// The type of the model to be rendered
    /// </typeparam>
    public abstract class MerchelloViewPage<TModel> : Umbraco.Web.Mvc.UmbracoViewPage<TModel>
    {
        /// <summary>
        /// The <see cref="CustomerContext"/>.
        /// </summary>
        private CustomerContext _customerContext;

        /// <summary>
        /// The <see cref="MerchelloHelper"/>
        /// </summary>
        private MerchelloHelper _helper;

        /// <summary>
        /// Gets the CurrentCustomer from the CustomerContext
        /// </summary>
        public ICustomerBase CurrentCustomer
        {
            get
            {
                if (_customerContext == null)
                    _customerContext = new CustomerContext(UmbracoContext);

                return _customerContext.CurrentCustomer;
            }
        }

        /// <summary>
        /// Gets the <see cref="MerchelloHelper"/>.
        /// </summary>
        public MerchelloHelper Merchello
        {
            get { return _helper ?? (_helper = new MerchelloHelper()); }
        }
    }
}