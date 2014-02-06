using System;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Core.Strategies.Taxation;
using Umbraco.Core.Cache;

namespace Merchello.Core.Gateways.Taxation
{
    /// <summary>
    /// Defines
    /// </summary>
    public abstract class TaxationGatewayProviderBase : GatewayProviderBase, ITaxationGatewayProvider
    {
        protected TaxationGatewayProviderBase(IGatewayProviderService gatewayProviderService, IGatewayProvider gatewayProvider, IRuntimeCacheProvider runtimeCacheProvider) 
            : base(gatewayProviderService, gatewayProvider, runtimeCacheProvider)
        { }

        /// <summary>
        /// Calculates the tax amount for an invoice
        /// </summary>
        /// <param name="invoice"><see cref="IInvoice"/></param>
        /// <returns><see cref="IInvoiceTaxResult"/></returns>
        public virtual IInvoiceTaxResult CalculateTaxForInvoice(IInvoice invoice)
        {
            throw new NotImplementedException();
        }

        public IInvoiceTaxResult CalculateTaxForInvoice(IInvoice invoice, IInvoiceTaxationStrategy strategy)
        {
            var attempt = strategy.GetInvoiceTaxResult(invoice);

            if (!attempt.Success) throw attempt.Exception;

            return attempt.Result;
        }
    }
}