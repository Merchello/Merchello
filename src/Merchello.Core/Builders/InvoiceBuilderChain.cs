namespace Merchello.Core.Builders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Models;
    using Sales;
    using Umbraco.Core;

    /// <summary>
    /// Represents an invoice builder
    /// </summary>
    internal sealed class InvoiceBuilderChain : BuildChainBase<IInvoice>
    {
        /// <summary>
        /// The sale preparation.
        /// </summary>
        private readonly SalePreparationBase _salePreparation;

        /// <summary>
        /// Constructor parameters for the base class activator
        /// </summary>
        private IEnumerable<object> _constructorParameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceBuilderChain"/> class.
        /// </summary>
        /// <param name="salePreparation">
        /// The sale preparation.
        /// </param>
        internal InvoiceBuilderChain(SalePreparationBase salePreparation)
        {
            Mandate.ParameterNotNull(salePreparation, "salesPreparation");
            _salePreparation = salePreparation;

            ResolveChain(Core.Constants.TaskChainAlias.SalesPreparationInvoiceCreate);
        }

        /// <summary>
        /// Builds the invoice
        /// </summary>
        /// <returns>Attempt{IInvoice}</returns>
        public override Attempt<IInvoice> Build()
        {
            var unpaid =
                _salePreparation.MerchelloContext.Services.InvoiceService.GetInvoiceStatusByKey(Core.Constants.DefaultKeys.InvoiceStatus.Unpaid);

            if (unpaid == null)
                return Attempt<IInvoice>.Fail(new NullReferenceException("Unpaid invoice status query returned null"));

            var attempt = (TaskHandlers.Any())
                       ? TaskHandlers.First().Execute(new Invoice(unpaid) { VersionKey = _salePreparation.ItemCache.VersionKey })
                       : Attempt<IInvoice>.Fail(new InvalidOperationException("The configuration Chain Task List could not be instantiated"));

            if (!attempt.Success) return attempt;

            // total the invoice
            attempt.Result.Total = attempt.Result.Items.Sum(x => x.TotalPrice);

            return attempt;
        }


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