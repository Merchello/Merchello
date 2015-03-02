using System;
using Merchello.Core;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Models;
using Merchello.Web;
using Merchello.Web.Workflow;
using Umbraco.Core.Logging;
using Umbraco.Web.Mvc;
using Merchello.Core.Services;
using Merchello.Core.Gateways.Shipping;

namespace Controllers
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Something like this will likely be included in the Merchello Core soon
    /// </remarks>
    public abstract class MerchelloSurfaceContoller : SurfaceController
    {
        private readonly IBasket _basket;
        private readonly IMerchelloContext _merchelloContext;
        private readonly ICustomerBase _currentCustomer;
        
        protected MerchelloSurfaceContoller(IMerchelloContext merchelloContext)
        {
            if (merchelloContext == null)
            {
                var ex = new ArgumentNullException("merchelloContext");
                LogHelper.Error<MerchelloSurfaceContoller>("The MerchelloContext was null upon instantiating the CartController.", ex);
                throw ex;
            }

            _merchelloContext = merchelloContext;

            var customerContext = new CustomerContext(UmbracoContext); // UmbracoContext is from SurfaceController
            _currentCustomer = customerContext.CurrentCustomer;

            _basket = _currentCustomer.Basket();
        }

        /// <summary>
        /// Gets the current customer.
        /// </summary>
        protected ICustomerBase CurrentCustomer
        {
            get { return _currentCustomer; }
        }

        /// <summary>
        /// Gets the Basket for the CurrentCustomer
        /// </summary>
        protected IBasket Basket
        {
            get { return _basket; }
        }

        /// <summary>
        /// We are going to hide the Umbraco Service Context here so controller that sub class this controller are "Merchello Surface Controllers"
        /// </summary>
        protected new IServiceContext Services
        {
            get { return _merchelloContext.Services; }
        }


        /// <summary>
        /// Exposes the <see cref="IPaymentContext"/>
        /// </summary>
        protected IPaymentContext Payment
        {
            get { return _merchelloContext.Gateways.Payment; }
        }

        /// <summary>
        /// Exposes the <see cref="IShippingContext"/>
        /// </summary>
        protected IShippingContext Shipping
        {
            get { return _merchelloContext.Gateways.Shipping; }
        }

    }
}