using System;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Checkout;
using Merchello.Core.Models;
using Merchello.Core.Orders;
using Umbraco.Core;

namespace Merchello.Core.Builders
{
    /// <summary>
    /// Represents an invoice builder
    /// </summary>
    internal sealed class InvoiceBuilderChain : BuildChainBase<IInvoice>
    {
        private readonly OrderPreparationBase _orderPreparation;

        internal InvoiceBuilderChain(OrderPreparationBase orderPreparation)
        {
            Mandate.ParameterNotNull(orderPreparation, "checkout");
            _orderPreparation = orderPreparation;

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
        private IEnumerable<object> _constructorParameters; 
        protected override IEnumerable<object> ConstructorArgumentValues
        {
            get
            {
                return _constructorParameters ?? 
                    (_constructorParameters =  new List<object>(new object[] {_orderPreparation} ));
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