namespace Merchello.Bazaar.Controllers
{
    using System;

    using Merchello.Bazaar.Factories;
    using Merchello.Core;
    using Merchello.Core.Models;
    using Merchello.Web;
    using Merchello.Web.Workflow;

    using Umbraco.Core;
    using Umbraco.Core.Models;
    using Umbraco.Web;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// The base controller for the Merchello Starter Kit.
    /// </summary>
    public abstract class RenderControllerBase : RenderMvcController
    {
        /// <summary>
        /// The <see cref="IMerchelloContext"/>.
        /// </summary>
        private readonly IMerchelloContext _merchelloContext;

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
            : this(umbracoContext, MerchelloContext.Current)
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
        /// Gets the currency.
        /// </summary>
        protected ICurrency Currency { get; private set; }

        /// <summary>
        /// Initializes the controller.
        /// </summary>
        private void Initialize()
        {
            this._shopPage = new Lazy<IPublishedContent>(() => this.UmbracoContext.PublishedContentRequest == null ? null : this.UmbracoContext.PublishedContentRequest.PublishedContent.AncestorOrSelf("MerchStore"));

            var storeSettingsService = this._merchelloContext.Services.StoreSettingService;
            var storeSetting = storeSettingsService.GetByKey(Core.Constants.StoreSettingKeys.CurrencyCodeKey);

            this.Currency = storeSettingsService.GetCurrencyByCode(storeSetting.Value);

            _viewModelFactory = new Lazy<IViewModelFactory>(() => new ViewModelFactory(Umbraco, CurrentCustomer, Currency));
        }
    }
}