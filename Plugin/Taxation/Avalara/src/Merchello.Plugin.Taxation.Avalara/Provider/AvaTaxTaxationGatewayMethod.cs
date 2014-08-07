using System;

namespace Merchello.Plugin.Taxation.Avalara.Provider
{
    using Core.Gateways.Taxation;
    using Core.Models;

    using Merchello.Core.Services;
    using Merchello.Plugin.Taxation.Avalara.Models;

    /// <summary>
    /// Represents the Avalara taxation gateway method
    /// </summary>
    public class AvaTaxTaxationGatewayMethod : TaxationGatewayMethodBase, IAvaTaxTaxationGatewayMethod
    {
        /// <summary>
        /// The  provider settings.
        /// </summary>
        private readonly AvaTaxProviderSettings _settings;

        /// <summary>
        /// The gateway provider service.
        /// </summary>
        private readonly IGatewayProviderService _gatewayProviderService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AvaTaxTaxationGatewayMethod"/> class.
        /// </summary>
        /// <param name="taxMethod">
        /// The tax method.
        /// </param>
        /// <param name="gatewayProviderService">
        /// The gateway Provider Service.
        /// </param>
        /// <param name="extendedData">
        /// The extended Data collection from the provider.
        /// </param>
        public AvaTaxTaxationGatewayMethod(ITaxMethod taxMethod, IGatewayProviderService gatewayProviderService, ExtendedDataCollection extendedData) : base(taxMethod)
        {
            _gatewayProviderService = gatewayProviderService;
            _settings = extendedData.GetAvaTaxProviderSettings();
        }

        /// <summary>
        /// Calculates tax for invoice.
        /// </summary>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
        /// <param name="taxAddress">
        /// The tax address.
        /// </param>
        /// <param name="estimateOnly">
        /// A value indicating whether or not this tax calculation is an estimate
        /// </param>
        /// <returns>
        /// The <see cref="ITaxCalculationResult"/>.
        /// </returns>
        public override ITaxCalculationResult CalculateTaxForInvoice(IInvoice invoice, IAddress taxAddress, bool estimateOnly = false)
        {
            var defaultStoreAddress = _settings.DefaultStoreAddress.ToTaxAddress();
            
            var request = invoice.AsTaxRequest(defaultStoreAddress, estimateOnly);

            throw new NotImplementedException();
        }
    }
}