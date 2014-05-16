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
        /// Returns an instance of an 'active' GatewayProvider associated with a GatewayMethod based given the unique Key (Guid) of the GatewayMethod
        /// </summary>
        /// <param name="gatewayMethodKey">The unique key (Guid) of the <see cref="IGatewayMethod"/></param>
        /// <returns>An instantiated GatewayProvider</returns>
        public override TaxationGatewayProviderBase GetProviderByMethodKey(Guid gatewayMethodKey)
        {
            return
                GetAllActivatedProviders()
                    .FirstOrDefault(x => ((TaxationGatewayProviderBase) x)
                        .TaxMethods.Any(y => y.Key == gatewayMethodKey)) as TaxationGatewayProviderBase;
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

            var provider = GatewayProviderResolver.GetProviderByKey<TaxationGatewayProviderBase>(providersKey);

            var gatewayTaxMethod = provider.GetGatewayTaxMethodByCountryCode(taxAddress.CountryCode);

            return gatewayTaxMethod.CalculateTaxForInvoice(invoice, taxAddress);
        }


    }
}