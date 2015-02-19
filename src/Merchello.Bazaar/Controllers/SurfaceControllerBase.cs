namespace Merchello.Bazaar.Controllers
{
    using System;

    using Merchello.Core;
    using Merchello.Core.Models;
    using Merchello.Core.Services;
    using Merchello.Web;
    using Merchello.Web.Workflow;

    using Umbraco.Core;
    using Umbraco.Core.Models;
    using Umbraco.Web;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// The surface controller base.
    /// </summary>
    [PluginController("Bazaar")]
    public abstract class SurfaceControllerBase : SurfaceController
    {
         /// <summary>
        /// The <see cref="IPublishedContent"/> that represents the store root.
        /// </summary>
        private Lazy<IPublishedContent> _shopPage;

        /// <summary>
        /// The <see cref="IPublishedContent"/> checkout page.
        /// </summary>
        private Lazy<IPublishedContent> _checkoutPage;


        private readonly IMerchelloContext _merchelloContext;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SurfaceControllerBase"/> class. 
        /// </summary>
        protected SurfaceControllerBase()
            : this(UmbracoContext.Current)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SurfaceControllerBase"/> class. 
        /// </summary>
        /// <param name="umbracoContext">
        /// The <see cref="UmbracoContext"/>
        /// </param>
        protected SurfaceControllerBase(UmbracoContext umbracoContext)
            : this(umbracoContext, MerchelloContext.Current)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SurfaceControllerBase"/> class. 
        /// </summary>
        /// <param name="umbracoContext">
        /// The <see cref="UmbracoContext"/>
        /// </param>
        /// <param name="merchelloContext">
        /// The <see cref="IMerchelloContext"/>
        /// </param>
        protected SurfaceControllerBase(UmbracoContext umbracoContext, IMerchelloContext merchelloContext)
        {
            Mandate.ParameterNotNull(umbracoContext, "umbracoContext");
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");

            this.CustomerContext = new CustomerContext(umbracoContext);
            this._merchelloContext = merchelloContext;
            this.Initialize();
        }

        #endregion

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
        /// Gets the root shop page.
        /// </summary>
        protected IPublishedContent ShopPage
        {
            get
            {
                return this._shopPage.Value;
            }
        }

        /// <summary>
        /// Gets the checkout page.
        /// </summary>
        protected IPublishedContent CheckoutPage
        {
            get
            {
                return this._checkoutPage.Value;
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
        /// Initializes the controller.
        /// </summary>
        private void Initialize()
        {
            this._shopPage = new Lazy<IPublishedContent>(() => this.UmbracoContext.PublishedContentRequest == null ? null : this.UmbracoContext.PublishedContentRequest.PublishedContent.AncestorOrSelf("MerchStore"));
            this._checkoutPage = new Lazy<IPublishedContent>(() => this.UmbracoContext.PublishedContentRequest == null ? null : this.UmbracoContext.PublishedContentRequest.PublishedContent.DescendantOrSelf("MerchCheckout"));
        }
    }
}