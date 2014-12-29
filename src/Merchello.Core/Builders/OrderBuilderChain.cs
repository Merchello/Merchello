namespace Merchello.Core.Builders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Models;
    using Umbraco.Core;

    /// <summary>
    /// Represents the OrderBuilderChain
    /// </summary>
    internal sealed class OrderBuilderChain : BuildChainBase<IOrder>
    {
        #region Fields

        /// <summary>
        /// The invoice.
        /// </summary>
        private readonly IInvoice _invoice;

        /// <summary>
        /// The order status.
        /// </summary>
        private readonly IOrderStatus _orderStatus;

        /// <summary>
        /// Constructor parameters for the base class activator
        /// </summary>
        private IEnumerable<object> _constructorParameters; 

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderBuilderChain"/> class.
        /// </summary>
        /// <param name="orderStatus">
        /// The order status.
        /// </param>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
        public OrderBuilderChain(IOrderStatus orderStatus, IInvoice invoice)
        {
            Mandate.ParameterNotNull(orderStatus, "orderStatus");
            Mandate.ParameterNotNull(invoice, "invoice");

            _orderStatus = orderStatus;
            _invoice = invoice;
            
            ResolveChain(Core.Constants.TaskChainAlias.OrderPreparationOrderCreate);
        }

        /// <summary>
        /// Gets the task count. Used for testing
        /// </summary>
        internal int TaskCount
        {
            get { return TaskHandlers.Count(); }
        }


        /// <summary>
        /// Gets the constructor argument values.
        /// </summary>
        protected override IEnumerable<object> ConstructorArgumentValues
        {
            get
            {
                return _constructorParameters ?? (_constructorParameters = new List<object>(new object[] { _invoice }));
            }
        }

        /// <summary>
        /// Builds the order
        /// </summary>
        /// <returns>The Attempt{IOrder}</returns>
        public override Attempt<IOrder> Build()
        {
            var attempt = TaskHandlers.Any()
                ? TaskHandlers.First().Execute(new Order(_orderStatus, _invoice.Key) 
                { 
                    OrderNumberPrefix = _invoice.InvoiceNumberPrefix,
                    VersionKey = _invoice.VersionKey 
                }) :
                Attempt<IOrder>.Fail(new InvalidOperationException("The configuration Chain Task List could not be instantiated."));

            return attempt;
        }
    }
}