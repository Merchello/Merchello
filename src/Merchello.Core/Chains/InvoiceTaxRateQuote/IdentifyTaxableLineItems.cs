using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Models;
using Umbraco.Core;

namespace Merchello.Core.Chains.InvoiceTaxRateQuote
{
    internal class IdentifyTaxableLineItems : AttemptChainTaskBase<IInvoice>
    {
        public override Attempt<IInvoice> PerformTask(IInvoice value)
        {
            throw new System.NotImplementedException();
        }
    }
}