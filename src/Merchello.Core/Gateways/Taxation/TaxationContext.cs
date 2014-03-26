using System;
using System.Linq;
using System.Collections.Generic;
using Merchello.Core.Models;
using Merchello.Core.Services;

namespace Merchello.Core.Gateways.Taxation
{
    /// <summary>
    /// Represents the TaxationContext
    /// </summary>
    internal class TaxationContext : GatewayProviderTypedContextBase<TaxationGatewayProviderBase>, ITaxationContext
    {
        public TaxationContext(IGatewayProviderService gatewayProviderService, IGatewayProviderResolver resolver)
            : base(gatewayProviderService, resolver)
        { }

        /// <summary>
        /// Resolves all active Taxation Gateway Providers
        /// </summary>
        /// <returns>A collection of all active TypedGatewayProviderinstances</returns>
        public override IEnumerable<TaxationGatewayProviderBase> ResolveAllActiveProviders()
        {
            return GatewayProviderResolver.CreateInstances<TaxationGatewayProviderBase>(GatewayProviderType.Taxation);
        }

        /// <summary>
        /// Resolves a taxation gateway provider by it's unique key
        /// </summary>
        /// <param name="key"></param>
        /// <returns>A taxation gateway provider</returns>
        public override TaxationGatewayProviderBase ResolveByKey(Guid key)
        {
            return GatewayProviderResolver.CreateInstance<TaxationGatewayProviderBase>(key);
        }

        /// <summary>
        /// Calculates taxes for the <see cref="IInvoice"/>
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/> to tax</param>
        /// <returns>The <see cref="ITaxCalculationResult"/></returns>
        /// <remarks>
        /// 
        /// This assumes that the tax rate is assoicated with the invoice's billing address
        /// 
        /// </remarks>
        public ITaxCalculationResult CalculateTaxesForInvoice(IInvoice invoice)
        {
            return CalculateTaxesForInvoice(invoice, invoice.GetBillingAddress());
        }

        /// <summary>
        /// Calculates taxes for the <see cref="IInvoice"/>
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/> to tax</param>
        /// <param name="taxAddress">The address to base the taxation calculation</param>
        /// <returns>The <see cref="ITaxCalculationResult"/></returns>
        public ITaxCalculationResult CalculateTaxesForInvoice(IInvoice invoice, IAddress taxAddress)
        {
            var providersKey =
                GatewayProviderService.GetTaxMethodsByCountryCode(taxAddress.CountryCode)
                                      .Select(x => x.ProviderKey).FirstOrDefault();

            if(Guid.Empty.Equals(providersKey)) return new TaxCalculationResult(0,0);

            var provider = GatewayProviderResolver.CreateInstance<TaxationGatewayProviderBase>(providersKey);

            var gatewayTaxMethod = provider.GetGatewayTaxMethodByCountryCode(taxAddress.CountryCode);

            return gatewayTaxMethod.CalculateTaxForInvoice(invoice, taxAddress);
        }
    }
}