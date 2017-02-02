namespace Merchello.Web.Mvc
{
    using Merchello.Core;
    using Merchello.Core.Gateways;
    using Merchello.Core.Models;
    using Merchello.Core.Services;
    using Merchello.Web.Pluggable;
    using Merchello.Web.Workflow;

    using Umbraco.Core;
    using Umbraco.Web;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// A controller to render front-end requests for Merchello
    /// </summary>
    public abstract class MerchelloRenderMvcController : RenderMvcController
    {
        /// <summary>
        /// The <see cref="IMerchelloContext"/>.
        /// </summary>
        private readonly IMerchelloContext _merchelloContext = MerchelloContext.Current;

        /// <summary>
        /// The <see cref="ICustomerContext"/>.
        /// </summary>
        private ICustomerContext _customerContext;

        /// <summary>
        /// The <see cref="ICurrency"/>.
        /// </summary>
        private ICurrency _currency;

        /// <summary>
        /// The setting to save initialized state.
        /// </summary>
        private bool _isInitialized;        

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MerchelloRenderMvcController"/> class.
        /// </summary>
        protected MerchelloRenderMvcController()
            : this(UmbracoContext.Current)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MerchelloRenderMvcController"/> class.
        /// </summary>
        /// <param name="umbracoContext">
        /// The <see cref="UmbracoContext"/>
        /// </param>
        protected MerchelloRenderMvcController(UmbracoContext umbracoContext)
            : base(umbracoContext)
        {
        }

        #endregion

        /// <summary>
        /// Gets the customer context.
        /// </summary>
        protected ICustomerContext CustomerContext
        {
            get
            {
                if (!_isInitialized) this.Initialize();
                return _customerContext;
            }
        }

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
        /// Gets the currency.
        /// </summary>
        protected ICurrency Currency
        {
            get
            {
                if (!_isInitialized) this.Initialize();
                return _currency;
            }
        }

        /// <summary>
        /// Initializes the controller.
        /// </summary>
        private void Initialize()
        {
            var storeSettingsService = this._merchelloContext.Services.StoreSettingService;
            var storeSetting = storeSettingsService.GetByKey(Core.Constants.StoreSetting.CurrencyCodeKey);
            _customerContext = PluggableObjectHelper.GetInstance<CustomerContextBase>("CustomerContext", UmbracoContext);
            _currency = storeSettingsService.GetCurrencyByCode(storeSetting.Value);
            _isInitialized = true;
        }
    }
}