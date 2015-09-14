namespace Merchello.Bazaar.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Gateways;
    using Merchello.Core.Models;
    using Merchello.Core.Services;

    using Umbraco.Core;

    /// <summary>
    /// The base checkout controller.
    /// </summary>
    public abstract class CheckoutControllerBase : BazaarControllerBase
    {
        /// <summary>
        /// The <see cref="IStoreSettingService"/>.
        /// </summary>
        private readonly IStoreSettingService _storeSettingService;

        /// <summary>
        /// The <see cref="IGatewayContext"/>.
        /// </summary>
        private readonly IGatewayContext _gatewayContext;

        /// <summary>
        /// The _all countries.
        /// </summary>
        private Lazy<IEnumerable<ICountry>> _allCountries;

        /// <summary>
        /// The _shipping countries.
        /// </summary>
        private Lazy<IEnumerable<ICountry>> _shippingCountries; 

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutControllerBase"/> class.
        /// </summary>
        protected CheckoutControllerBase()
            : this(Core.MerchelloContext.Current)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutControllerBase"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        protected CheckoutControllerBase(IMerchelloContext merchelloContext)
        {
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");
            _storeSettingService = merchelloContext.Services.StoreSettingService;
            _gatewayContext = merchelloContext.Gateways;
            this.Initialize();
        }

        #endregion

        /// <summary>
        /// Gets the store setting service.
        /// </summary>
        protected IStoreSettingService StoreSettingService
        {
            get
            {
                return _storeSettingService;
            }
        }

        /// <summary>
        /// Gets a collection of all countries.
        /// </summary>
        protected IEnumerable<ICountry> AllCountries
        {
            get
            {
                return _allCountries.Value;
            }
        }

        /// <summary>
        /// Gets the allowed ship countries.
        /// </summary>
        protected IEnumerable<ICountry> AllowedShipCountries
        {
            get
            {
                return _shippingCountries.Value;
            }
        }

        /// <summary>
        /// Responsible for initializing the controller.
        /// </summary>
        private void Initialize()
        {
            _allCountries = new Lazy<IEnumerable<ICountry>>(() => _storeSettingService.GetAllCountries().OrderBy(x => x.Name));
            _shippingCountries = new Lazy<IEnumerable<ICountry>>(() => _gatewayContext.Shipping.GetAllowedShipmentDestinationCountries().OrderBy(x => x.Name));
        }
    }
}