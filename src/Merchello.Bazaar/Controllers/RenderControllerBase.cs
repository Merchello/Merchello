namespace Merchello.Bazaar.Controllers
{
    using System;

    using Merchello.Bazaar.Factories;
    using Merchello.Core;
    using Merchello.Web.Mvc;

    using Umbraco.Core.Models;
    using Umbraco.Web;

    /// <summary>
    /// The base controller for the Merchello Starter Kit.
    /// </summary>
    public abstract class RenderControllerBase : MerchelloRenderMvcController
    {
        /// <summary>
        /// The <see cref="IViewModelFactory"/>.
        /// </summary>
        private Lazy<IViewModelFactory> _viewModelFactory;

        /// <summary>
        /// The <see cref="IPublishedContent"/> that represents the store root.
        /// </summary>
        private Lazy<IPublishedContent> _shopPage;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderControllerBase"/> class.
        /// </summary>
        protected RenderControllerBase()
            : this(UmbracoContext.Current)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderControllerBase"/> class.
        /// </summary>
        /// <param name="umbracoContext">
        /// The <see cref="UmbracoContext"/>
        /// </param>
        protected RenderControllerBase(UmbracoContext umbracoContext)
            : this(umbracoContext, Core.MerchelloContext.Current)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderControllerBase"/> class.
        /// </summary>
        /// <param name="umbracoContext">
        /// The <see cref="UmbracoContext"/>
        /// </param>
        /// <param name="merchelloContext">
        /// The <see cref="IMerchelloContext"/>
        /// </param>
        protected RenderControllerBase(UmbracoContext umbracoContext, IMerchelloContext merchelloContext)
            : base(umbracoContext, merchelloContext)
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
        /// Gets the view model factory.
        /// </summary>
        protected IViewModelFactory ViewModelFactory
        {
            get
            {
                return _viewModelFactory.Value;
            }
        }

        /// <summary>
        /// Initializes the controller.
        /// </summary>
        private void Initialize()
        {
            this._shopPage = new Lazy<IPublishedContent>(() => this.UmbracoContext.PublishedContentRequest == null ? null : this.UmbracoContext.PublishedContentRequest.PublishedContent.AncestorOrSelf("MerchStore"));

            _viewModelFactory = new Lazy<IViewModelFactory>(() => new ViewModelFactory(Umbraco, CurrentCustomer, Currency));
        }
    }
}