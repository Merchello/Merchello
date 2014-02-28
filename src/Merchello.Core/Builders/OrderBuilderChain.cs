using System;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Models;
using Umbraco.Core;

namespace Merchello.Core.Builders
{
    /// <summary>
    /// Represents the OrderBuilderChain
    /// </summary>
    internal sealed class OrderBuilderChain : BuildChainBase<IOrder>
    {
        private readonly IInvoice _invoice;

        public OrderBuilderChain(IInvoice invoice)
        {
            Mandate.ParameterNotNull(invoice, "invoice");

            _invoice = invoice;

            ResolveChain(Constants.TaskChainAlias.OrderPreparationOrderCreate);
        }

        /// <summary>
        /// Builds the order
        /// </summary>
        /// <returns>Attempt{IOrder}</returns>
        public override Attempt<IOrder> Build()
        {
            var attempt = (TaskHandlers.Any()) 
                ? TaskHandlers.First().Execute(new Order(Constants.DefaultKeys.OrderStatus.NotFulfilled, _invoice.Key) { VersionKey = _invoice.VersionKey }) :
                Attempt<IOrder>.Fail(new InvalidOperationException("The configuration Chain Task List could not be instantiated"));

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
                return _constructorParameters ?? (_constructorParameters = new List<object>(new object[] { _invoice }));
            }
        }
    }
}