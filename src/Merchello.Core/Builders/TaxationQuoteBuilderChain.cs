using System;
using Merchello.Core.Models;
using Umbraco.Core;

namespace Merchello.Core.Builders
{
    /// <summary>
    /// Represents the 
    /// </summary>
    internal sealed class TaxationQuoteBuilderChain : BuildChainBase<IInvoice>
    {
        public TaxationQuoteBuilderChain()
        {
            ResolveChain(Constants.TaskChainAlias.InvoiceTaxRateQuote);
        }

        public override Attempt<IInvoice> Build()
        {
            throw new NotImplementedException();
        }


        protected override Tuple<Type[], object[]> ConstructorParameters
        {
            get { throw new NotImplementedException(); }
        }
    }
}