using System.Collections.Generic;
using Merchello.Core.Models;

namespace Merchello.Core.Gateways.Taxation
{
    /// <summary>
    /// Defines a taxation gateway provider
    /// </summary>
    public interface ITaxationGatewayProvider : IGateway
    {
        /// <summary>
        /// Attempts to create a <see cref="ITaxMethod"/> for a given provider and country.  If the provider already 
        /// defines a tax rate for the country, the creation fails.
        /// </summary>
        /// <param name="countryCode">The two character ISO country code</param>
        ITaxMethod CreateTaxMethod(string countryCode);

        /// <summary>
        /// Creates a <see cref="ITaxMethod"/>
        /// </summary>
        /// <param name="countryCode">The two letter ISO Country Code</param>
        /// <param name="taxPercentageRate">The decimal percentage tax rate</param>
        /// <returns>The <see cref="ITaxMethod"/></returns>
        ITaxMethod CreateTaxMethod(string countryCode, decimal taxPercentageRate);

        /// <summary>
        /// Saves a <see cref="ITaxMethod"/>
        /// </summary>
        /// <param name="taxMethod">The <see cref="ITaxMethod"/> to be saved</param>
        void SaveTaxMethod(ITaxMethod taxMethod);

        /// <summary>
        /// Deletes a <see cref="ITaxMethod"/>
        /// </summary>
        /// <param name="taxMethod">The <see cref="ITaxMethod"/> to be deleted</param>
        void DeleteTaxMethod(ITaxMethod taxMethod);

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
        IInvoiceTaxResult CalculateTaxForInvoice(IInvoice invoice);

        /// <summary>
        /// Calculates the tax amount for an invoice
        /// </summary>
        /// <param name="invoice"><see cref="IInvoice"/></param>
        /// <param name="taxAddress">The <see cref="IAddress"/> to base taxation rates.  Either origin or destination address.</param>
        /// <returns><see cref="IInvoiceTaxResult"/></returns>
        IInvoiceTaxResult CalculateTaxForInvoice(IInvoice invoice, IAddress taxAddress);

        /// <summary>
        /// Calculates the tax amount for an invoice
        /// </summary>
        /// <param name="strategy">The strategy to use when calculating the tax amount</param>
        /// <returns><see cref="IInvoiceTaxResult"/></returns>
        IInvoiceTaxResult CalculateTaxForInvoice(IInvoiceTaxationStrategy strategy);

        /// <summary>
        /// 
        /// </summary>
        IEnumerable<ITaxMethod> TaxMethods { get; } 
    }
}