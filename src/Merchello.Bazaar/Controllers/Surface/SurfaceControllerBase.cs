namespace Merchello.Bazaar.Controllers.Surface
{
    using System;

    using Merchello.Core;
    using Merchello.Web.Mvc;

    using Umbraco.Core.Models;
    using Umbraco.Web;

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
            : base(merchelloContext, umbracoContext)
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