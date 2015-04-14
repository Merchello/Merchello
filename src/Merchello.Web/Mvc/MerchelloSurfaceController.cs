namespace Merchello.Web.Mvc
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Gateways;
    using Merchello.Core.Models;
    using Merchello.Core.Services;
    using Merchello.Web.Workflow;

    using Umbraco.Core;
    using Umbraco.Core.Logging;
    using Umbraco.Web;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// A <see cref="SurfaceController"/> that Merchello presentation Add-in controllers should inherit from
    /// </summary>
    public abstract class MerchelloSurfaceController : SurfaceController
    {
        /// <summary>
        /// The <see cref="IMerchelloContext"/>.
        /// </summary>
        private readonly IMerchelloContext _merchelloContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="MerchelloSurfaceController"/> class.
        /// </summary>
        protected MerchelloSurfaceController()
            : this(MerchelloContext.Current)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MerchelloSurfaceController"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The <see cref="IMerchelloContext"/>.
        /// </param>
        protected MerchelloSurfaceController(IMerchelloContext merchelloContext)
            : this(merchelloContext, UmbracoContext.Current)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MerchelloSurfaceController"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The <see cref="IMerchelloContext"/>.
        /// </param>
        /// <param name="umbracoContext">
        /// The <see cref="UmbracoContext"/>.
        /// </param>
        protected MerchelloSurfaceController(IMerchelloContext merchelloContext, UmbracoContext umbracoContext)
            : base(umbracoContext)
        {
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");

            _merchelloContext = merchelloContext;
            CustomerContext = new CustomerContext(umbracoContext);
        }


        /// <summary>
        /// Gets the customer context.
        /// </summary>
        protected CustomerContext CustomerContext { get; private set; }

        /// <summary>
        /// Gets the current customer.
        /// </summary>
        protected ICustomerBase CurrentCustomer
        {
            get
            {
                return this.CustomerContext.CurrentCustomer;
            }
        }

        /// <summary>
        /// Gets the current customer <see cref="IBasket"/>.
        /// </summary>
        protected IBasket Basket
        {
            get
            {
                return this.CurrentCustomer.Basket();
            }
        }

        /// <summary>
        /// Gets the services.
        /// </summary>
        protected IServiceContext MerchelloServices
        {
            get
            {
                return this._merchelloContext.Services;
            }
        }

        /// <summary>
        /// Gets the gateway context.
        /// </summary>
        protected IGatewayContext GatewayContext
        {
            get
            {
                return _merchelloContext.Gateways;
            }
        }

        /// <summary>
        /// Ensures the owner of the item cache.
        /// </summary>
        /// <param name="collection">
        /// The <see cref="LineItemCollection"/>
        /// </param>
        /// <param name="lineItemKey">
        /// The line item key.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// Throws an exception if the line item does not belong to the current customer
        /// </exception>
        protected void EnsureOwner(IEnumerable<ILineItem> collection, Guid lineItemKey)
        {
            if (collection.FirstOrDefault(x => x.Key == lineItemKey) != null) return;
            var exception =
                new InvalidOperationException(
                    "Attempt to delete an item from a collection that does not match the CurrentUser");
            LogHelper.Error<MerchelloSurfaceController>("Customer item cache operation failed.", exception);

            throw exception;
        }
    }
}