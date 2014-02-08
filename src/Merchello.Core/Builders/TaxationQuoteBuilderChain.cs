using System;
using System.Linq;
using Merchello.Core.Models;
using Umbraco.Core;

namespace Merchello.Core.Builders
{
    /// <summary>
    /// Represents the taxation quote builder
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

        /// <summary>
        /// Used for testing
        /// </summary>
        internal int TaskCount
        {
            get { return TaskHandlers.Count(); }
        }
    }
}