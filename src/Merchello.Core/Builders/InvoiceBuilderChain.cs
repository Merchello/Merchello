using System;
using System.Linq;
using Merchello.Core.Checkout;
using Merchello.Core.Models;
using Umbraco.Core;

namespace Merchello.Core.Builders
{
    /// <summary>
    /// Represents an invoice builder
    /// </summary>
    internal sealed class InvoiceBuilderChain : BuildChainBase<IInvoice>
    {
        private readonly CheckoutBase _checkout;

        internal InvoiceBuilderChain(CheckoutBase checkout)
        {
            Mandate.ParameterNotNull(checkout, "checkout");
            _checkout = checkout;

            ResolveChain(Constants.TaskChainAlias.CheckoutInvoiceCreate);
        }

        public override Attempt<IInvoice> Build()
        {
            return (TaskHandlers.Any())
                       ? TaskHandlers.First().Execute(new Invoice(Constants.DefaultKeys.UnpaidInvoiceStatusKey))
                       : Attempt<IInvoice>.Fail(new InvalidOperationException("The configuration Chain Task List could not be instantiated"));
        }

 
        /// <summary>
        /// Constructor parameters for the base class activator
        /// </summary>
        private Tuple<Type[], object[]> _constructorParameters; 
        protected override Tuple<Type[], object[]> ConstructorParameters
        {
            get
            {
                return _constructorParameters ?? 
                    (_constructorParameters =  new Tuple<Type[], object[]>(new[] {typeof (CheckoutBase)},new object[] {_checkout} ));
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