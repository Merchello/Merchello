using Merchello.Core.Gateways.Taxation;
using Merchello.Core.Models;
using Umbraco.Core;

namespace Merchello.Core.Strategies.Taxation
{
    /// <summary>
    /// Defines a taxation strategy
    /// </summary>
    public interface IInvoiceTaxationStrategy : IStrategy
    {
        /// <summary>
        /// Computes the invoice tax result
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/> to be used in the tax calculation</param>
        /// <returns>The <see cref="IInvoiceTaxResult"/></returns>
        Attempt<IInvoiceTaxResult> GetInvoiceTaxResult(IInvoice invoice);
    }
}
