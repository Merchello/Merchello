using System;
using System.Collections.Generic;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Umbraco.Core.Cache;
using Umbraco.Core.Logging;

namespace Merchello.Core.Gateways.Taxation
{
    /// <summary>
    /// Defines a base taxation gateway provider
    /// </summary>
    public abstract class TaxationGatewayProviderBase : GatewayProviderBase, ITaxationGatewayProvider
    {
        

        protected TaxationGatewayProviderBase(IGatewayProviderService gatewayProviderService, IGatewayProvider gatewayProvider, IRuntimeCacheProvider runtimeCacheProvider)
            : base(gatewayProviderService, gatewayProvider, runtimeCacheProvider)
        { }

        /// <summary>
        /// Attempts to create a <see cref="ITaxMethod"/> for a given provider and country.  If the provider already 
        /// defines a tax rate for the country, the creation fails.
        /// </summary>
        /// <param name="countryCode">The two character ISO country code</param>
        public virtual ITaxMethod CreateTaxMethod(string countryCode)
        {
            return CreateTaxMethod(countryCode, 0);
        }


        /// <summary>
        /// Creates a <see cref="ITaxMethod"/>
        /// </summary>
        /// <param name="countryCode">The two letter ISO Country Code</param>
        /// <param name="taxPercentageRate">The decimal percentage tax rate</param>
        /// <returns>The <see cref="ITaxMethod"/></returns>
        public ITaxMethod CreateTaxMethod(string countryCode, decimal taxPercentageRate)
        {
            var attempt = GatewayProviderService.CreateTaxMethodWithKey(GatewayProvider.Key, countryCode, taxPercentageRate);
        
            if (!attempt.Success)
            {
                LogHelper.Error<TaxationGatewayProviderBase>("CreateTaxMethod failed.", attempt.Exception);
                throw attempt.Exception;
            }

            return attempt.Result;
        }

        /// <summary>
        /// Saves a <see cref="ITaxMethod"/>
        /// </summary>
        /// <param name="taxMethod">The <see cref="ITaxMethod"/> to be saved</param>
        public void SaveTaxMethod(ITaxMethod taxMethod)
        {
            GatewayProviderService.Save(taxMethod);
        }

        /// <summary>
        /// Deletes a <see cref="ITaxMethod"/>
        /// </summary>
        /// <param name="taxMethod">The <see cref="ITaxMethod"/> to be deleted</param>
        public void DeleteTaxMethod(ITaxMethod taxMethod)
        {
            GatewayProviderService.Delete(taxMethod);
        }

        /// <summary>
        /// Deletes all <see cref="ITaxMethod"/>s associated with the provider
        /// </summary>
        internal void DeleteAllTaxMethods()
        {
            foreach(var taxMethod in TaxMethods) DeleteTaxMethod(taxMethod);
        }

        /// <summary>
        /// Calculates the tax amount for an invoice
        /// </summary>
        /// <param name="invoice"><see cref="IInvoice"/></param>
        /// <returns><see cref="IInvoiceTaxResult"/></returns>
        /// <remarks>
        /// 
        /// Assumes the billing address of the invoice will be used for the taxation address
        /// 
        /// </remarks>
        public virtual IInvoiceTaxResult CalculateTaxForInvoice(IInvoice invoice)
        {
            return CalculateTaxForInvoice(invoice, invoice.GetBillingAddress());
        }

        /// <summary>
        /// Calculates the tax amount for an invoice
        /// </summary>
        /// <param name="invoice"><see cref="IInvoice"/></param>
        /// <param name="taxAddress">The <see cref="IAddress"/> to base taxation rates.  Either origin or destination address.</param>
        /// <returns><see cref="IInvoiceTaxResult"/></returns>
        public abstract IInvoiceTaxResult CalculateTaxForInvoice(IInvoice invoice, IAddress taxAddress);


        /// <summary>
        /// Calculates the tax amount for an invoice
        /// </summary>
        /// <param name="strategy">The invoice taxation strategy to use when calculating the tax amount</param>
        /// <returns><see cref="IInvoiceTaxResult"/></returns>
        public virtual IInvoiceTaxResult CalculateTaxForInvoice(IInvoiceTaxationStrategy strategy)
        {
            var attempt = strategy.CalculateTaxesForInvoice();

            if (!attempt.Success) throw attempt.Exception;

            return attempt.Result;
        }


        private IEnumerable<ITaxMethod> _taxMethods;
        /// <summary>
        /// Gets a collection of <see cref="ITaxMethod"/> assoicated with this provider
        /// </summary>
        public IEnumerable<ITaxMethod> TaxMethods
        {
            get {
                return _taxMethods ??
                       (_taxMethods = GatewayProviderService.GetTaxMethodsByProviderKey(GatewayProvider.Key));
            }
        }
    }
}