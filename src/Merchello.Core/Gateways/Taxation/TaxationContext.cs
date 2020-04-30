namespace Merchello.Core.Gateways.Taxation
{
    using System;
    using System.Linq;

    using Merchello.Core.Models;
    using Merchello.Core.Services;

    using Umbraco.Core;
    using Umbraco.Core.Logging;

    /// <summary>
    /// Represents the TaxationContext
    /// </summary>
    internal class TaxationContext : GatewayProviderTypedContextBase<TaxationGatewayProviderBase>, ITaxationContext
    {
        /// <summary>
        /// The store setting service.
        /// </summary>
        private readonly IStoreSettingService _storeSettingService;

        /// <summary>
        /// The _tax by product method.
        /// </summary>
        private ITaxationByProductMethod _taxByProductMethod;

        /// <summary>
        /// The _tax method not set.
        /// </summary>
        private bool _taxMethodNotQueried = true;

        /// <summary>
        /// The <see cref="TaxationApplication"/>.
        /// </summary>
        private TaxationApplication _taxationApplication;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaxationContext"/> class.
        /// </summary>
        /// <param name="gatewayProviderService">
        /// The gateway provider service.
        /// </param>
        /// <param name="storeSettingService">
        /// The <see cref="IStoreSettingService"/>
        /// </param>
        /// <param name="resolver">
        /// The resolver.
        /// </param>
        public TaxationContext(IGatewayProviderService gatewayProviderService, IStoreSettingService storeSettingService, IGatewayProviderResolver resolver)
            : base(gatewayProviderService, resolver)
        {
            Ensure.ParameterNotNull(storeSettingService, "storeSettingService");
            _storeSettingService = storeSettingService;
            TaxApplicationInitialized = false;
        }

        /// <summary>
        /// Gets a value indicating whether product pricing enabled.
        /// </summary>
        public bool ProductPricingEnabled
        {
            get
            {
                return TaxationApplication == TaxationApplication.Product && ProductPricingTaxMethod != null;
            }         
        }

        /// <summary>
        /// Gets or sets the taxation application.
        /// </summary>
        public TaxationApplication TaxationApplication
        {
            get
            {
                if (!TaxApplicationInitialized)
                {
                    this.SetTaxApplicationSetting();
                }
                return _taxationApplication;
            }

            internal set
            {
                _taxationApplication = value;
            }
        }

        /// <summary>
        /// Gets the product pricing tax method.
        /// </summary>
        public ITaxationByProductMethod ProductPricingTaxMethod
        {
            get
            {
                if (_taxByProductMethod == null && this._taxMethodNotQueried)
                {
                    _taxByProductMethod = this.GetTaxationByProductMethod();
                }

                return _taxByProductMethod;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether store settings query for the tax application has
        /// been initialized.
        /// </summary>
        private bool TaxApplicationInitialized { get; set; }

        /// <summary>
        /// Returns an instance of an 'active' GatewayProvider associated with a GatewayMethod based given the unique Key (GUID) of the GatewayMethod
        /// </summary>
        /// <param name="gatewayMethodKey">The unique key (GUID) of the <see cref="IGatewayMethod"/></param>
        /// <returns>An instantiated GatewayProvider</returns>
        public override TaxationGatewayProviderBase GetProviderByMethodKey(Guid gatewayMethodKey)
        {
            return
                GetAllActivatedProviders()
                    .FirstOrDefault(x => ((TaxationGatewayProviderBase)x)
                        .TaxMethods.Any(y => y.Key == gatewayMethodKey)) as TaxationGatewayProviderBase;
        }

        /// <summary>
        /// Calculates taxes for the <see cref="IInvoice"/>
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/> to tax</param>
        /// <param name="quoteOnly">
        /// An optional parameter indicating that the tax calculation should be an estimate.
        /// This is useful for some 3rd party tax APIs
        /// </param>
        /// <returns>The <see cref="ITaxCalculationResult"/></returns>
        /// <remarks>
        /// 
        /// This assumes that the tax rate is associated with the invoice's billing address
        /// 
        /// </remarks>
        public ITaxCalculationResult CalculateTaxesForInvoice(IInvoice invoice, bool quoteOnly = false)
        {
            return CalculateTaxesForInvoice(invoice, invoice.GetBillingAddress());
        }

        /// <summary>
        /// Calculates taxes for the <see cref="IInvoice"/>
        /// </summary>
        /// <param name="invoice">
        /// The <see cref="IInvoice"/> to tax
        /// </param>
        /// <param name="taxAddress">
        /// The address to base the taxation calculation
        /// </param>
        /// <param name="quoteOnly">
        /// An optional parameter indicating that the tax calculation should be an estimate.
        /// This is useful for some 3rd party tax APIs
        /// </param>
        /// <returns>
        /// The <see cref="ITaxCalculationResult"/>
        /// </returns>
        public ITaxCalculationResult CalculateTaxesForInvoice(IInvoice invoice, IAddress taxAddress, bool quoteOnly = false)
        {
            var providersKey =
                GatewayProviderService.GetTaxMethodsByCountryCode(taxAddress.CountryCode)
                                      .Select(x => x.ProviderKey).FirstOrDefault();

            if (Guid.Empty.Equals(providersKey)) return new TaxCalculationResult(0, 0);

            var provider = GatewayProviderResolver.GetProviderByKey<TaxationGatewayProviderBase>(providersKey);

            var gatewayTaxMethod = provider.GetGatewayTaxMethodByCountryCode(taxAddress.CountryCode);

            return gatewayTaxMethod.CalculateTaxForInvoice(invoice, taxAddress);
        }

        /// <summary>
        /// Calculates taxes based on a product.
        /// </summary>
        /// <param name="product">
        /// The product.
        /// </param>
        /// <returns>
        /// The <see cref="ITaxCalculationResult"/>.
        /// </returns>
        public IProductTaxCalculationResult CalculateTaxesForProduct(IProductVariantDataModifierData product)
        {
            var empty = ProductTaxCalculationResult.GetEmptyResult();


            if (!ProductPricingEnabled) return empty;

            return this.ProductPricingTaxMethod == null ? 
                empty : 
                this.ProductPricingTaxMethod.CalculateTaxForProduct(product);
        }


        /// <summary>
        /// Gets the tax method for a given tax address
        /// </summary>
        /// <param name="taxAddress">
        /// The tax address
        /// </param>
        /// <returns>
        /// The <see cref="ITaxMethod"/>.
        /// </returns>
        public ITaxMethod GetTaxMethodForTaxAddress(IAddress taxAddress)
        {
            return GetTaxMethodForCountryCode(taxAddress.CountryCode);
        }

        /// <summary>
        /// Gets the tax method for country code.
        /// </summary>
        /// <param name="countryCode">
        /// The country code.
        /// </param>
        /// <returns>
        /// The <see cref="ITaxMethod"/>.
        /// </returns>
        public ITaxMethod GetTaxMethodForCountryCode(string countryCode)
        {
            return GatewayProviderService.GetTaxMethodsByCountryCode(countryCode).FirstOrDefault();
        }


        /// <summary>
        /// Resets the product pricing method to null so that it can be required.
        /// </summary>
        internal void ClearProductPricingMethod()
        {
            TaxApplicationInitialized = false;
            _taxByProductMethod = null;
        }

        /// <summary>
        /// The get taxation by product method.
        /// </summary>
        /// <returns>
        /// The <see cref="ITaxationByProductMethod"/>.
        /// </returns>
        /// <exception cref="NullReferenceException">
        /// Throws a null reference exception if a provider cannot be resolved
        /// </exception>
        private ITaxationByProductMethod GetTaxationByProductMethod()
        {
            var taxMethod = GatewayProviderService.GetTaxMethodForProductPricing();
            if (taxMethod == null)
            {
                LogHelper.Debug<TaxationContext>("Product based pricing is set in settings, but a TaxMethod has not been assigned.");
                this._taxMethodNotQueried = true;
                return null;
            }

            var provider = GatewayProviderResolver.GetProviderByKey<TaxationGatewayProviderBase>(taxMethod.ProviderKey);

            if (provider == null)
            {
                var error = new NullReferenceException("Could not reTaxationGatewayProvider for CalculateTaxForProduct could not be resolved");
                LogHelper.Error<TaxationContext>("Resolution failure", error);
                throw error;
            }

            var productProvider = provider as ITaxationByProductProvider;

            if (productProvider != null)
            {
                this._taxMethodNotQueried = false;
                return productProvider.GetTaxationByProductMethod(taxMethod.Key);
            }

            LogHelper.Debug<TaxationContext>("Resolved provider did not Implement ITaxationByProductProvider returning no tax");
            return null;
        }

        /// <summary>
        /// The set tax application setting.
        /// </summary>
        private void SetTaxApplicationSetting()
        {
            var setting = _storeSettingService.GetByKey(Core.Constants.StoreSetting.GlobalTaxationApplicationKey);
            if (setting == null)
            {
                TaxationApplication = TaxationApplication.Invoice;
            }
            else
            {
                TaxationApplication taxApp;
                this.TaxationApplication = Enum.TryParse(setting.Value, true, out taxApp) ? 
                    taxApp : 
                    TaxationApplication.Invoice;
            }

            _taxMethodNotQueried = true;
            TaxApplicationInitialized = true;
        }
    }
}