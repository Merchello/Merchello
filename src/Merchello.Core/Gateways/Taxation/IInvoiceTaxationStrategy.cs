using Merchello.Core.Models;
using Umbraco.Core;

namespace Merchello.Core.Gateways.Taxation
{
    /// <summary>
    /// Defines a taxation strategy
    /// </summary>
    public interface IInvoiceTaxationStrategy : IStrategy
    {
        /// <summary>
        /// Computes the invoice tax result
        /// </summary>
        /// <returns>The <see cref="IInvoiceTaxResult"/></returns>
        Attempt<IInvoiceTaxResult> CalculateTaxesForInvoice();
    }
}
