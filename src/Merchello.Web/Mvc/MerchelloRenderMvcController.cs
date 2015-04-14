namespace Merchello.Web.Mvc
{
    using Merchello.Core;
    using Merchello.Core.Gateways;
    using Merchello.Core.Models;
    using Merchello.Core.Services;
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
        private readonly IMerchelloContext _merchelloContext;

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
            : this(umbracoContext, Core.MerchelloContext.Current)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MerchelloRenderMvcController"/> class.
        /// </summary>
        /// <param name="umbracoContext">
        /// The <see cref="UmbracoContext"/>
        /// </param>
        /// <param name="merchelloContext">
        /// The <see cref="IMerchelloContext"/>
        /// </param>
        protected MerchelloRenderMvcController(UmbracoContext umbracoContext, IMerchelloContext merchelloContext)
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
        protected ICurrency Currency { get; private set; }

        /// <summary>
        /// Initializes the controller.
        /// </summary>
        private void Initialize()
        {
            var storeSettingsService = this._merchelloContext.Services.StoreSettingService;
            var storeSetting = storeSettingsService.GetByKey(Core.Constants.StoreSettingKeys.CurrencyCodeKey);

            this.Currency = storeSettingsService.GetCurrencyByCode(storeSetting.Value);
        }
    }
}