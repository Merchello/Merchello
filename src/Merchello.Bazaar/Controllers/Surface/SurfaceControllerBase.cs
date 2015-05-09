namespace Merchello.Bazaar.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Gateways;
    using Merchello.Core.Models;
    using Merchello.Core.Services;
    using Merchello.Web;
    using Merchello.Web.Mvc;
    using Merchello.Web.Pluggable;
    using Merchello.Web.Workflow;

    using Umbraco.Core;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Models;
    using Umbraco.Web;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// The surface controller base.
    /// </summary>
    public abstract class SurfaceControllerBase : MerchelloSurfaceController
    {
         /// <summary>
        /// The <see cref="IPublishedContent"/> that represents the store root.
        /// </summary>
        private Lazy<IPublishedContent> _shopPage;

        /// <summary>
        /// The <see cref="IPublishedContent"/> checkout page.
        /// </summary>
        private Lazy<IPublishedContent> _checkoutPage;


        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SurfaceControllerBase"/> class. 
        /// </summary>
        protected SurfaceControllerBase()
        {
            this.Initialize();
        }

        #endregion

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
        /// Initializes the controller.
        /// </summary>
        private void Initialize()
        {
            this._shopPage = new Lazy<IPublishedContent>(() => this.UmbracoContext.PublishedContentRequest == null ? null : this.UmbracoContext.PublishedContentRequest.PublishedContent.AncestorOrSelf("MerchStore"));
            this._checkoutPage = new Lazy<IPublishedContent>(() => this.UmbracoContext.PublishedContentRequest == null ? null : this.UmbracoContext.PublishedContentRequest.PublishedContent.DescendantOrSelf("MerchCheckout"));
        }
    }
}