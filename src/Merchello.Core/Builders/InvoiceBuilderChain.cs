using System;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Models;
using Merchello.Core.Sales;
using Umbraco.Core;

namespace Merchello.Core.Builders
{
    /// <summary>
    /// Represents an invoice builder
    /// </summary>
    internal sealed class InvoiceBuilderChain : BuildChainBase<IInvoice>
    {
        private readonly SalePreparationBase _salePreparation;

        internal InvoiceBuilderChain(SalePreparationBase salePreparation)
        {
            Mandate.ParameterNotNull(salePreparation, "salesManager");
            _salePreparation = salePreparation;

            ResolveChain(Constants.TaskChainAlias.SalesManagerInvoiceCreate);
        }

        /// <summary>
        /// Builds the invoice
        /// </summary>
        /// <returns>Attempt{IInvoice}</returns>
        public override Attempt<IInvoice> Build()
        {
            var attempt = (TaskHandlers.Any())
                       ? TaskHandlers.First().Execute(new Invoice(Constants.DefaultKeys.InvoiceStatus.Unpaid) { VersionKey = _salePreparation.ItemCache.VersionKey })
                       : Attempt<IInvoice>.Fail(new InvalidOperationException("The configuration Chain Task List could not be instantiated"));

            if (!attempt.Success) return attempt;

            // total the invoice
            attempt.Result.Total = attempt.Result.Items.Sum(x => x.TotalPrice);

            return attempt;
        }

 
        /// <summary>
        /// Constructor parameters for the base class activator
        /// </summary>
        private IEnumerable<object> _constructorParameters; 
        protected override IEnumerable<object> ConstructorArgumentValues
        {
            get
            {
                return _constructorParameters ?? 
                    (_constructorParameters =  new List<object>(new object[] {_salePreparation} ));
            }
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