using System;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Chains;
using Merchello.Core.Chains.CheckOut;
using Merchello.Core.Checkout;
using Merchello.Core.Models;
using Umbraco.Core;

namespace Merchello.Core.Builders
{
    internal class InvoiceBuilder : IInvoiceBuilder
    {
        private readonly CheckoutBase _checkout;
        private IEnumerable<AttemptChainTaskHandler<IInvoice>> _taskHandlers; 

        internal InvoiceBuilder(CheckoutBase checkout)
        {
            Mandate.ParameterNotNull(checkout, "checkout");

            _checkout = checkout;

            BuildChain();
        }

        /// <summary>
        /// Constructs the task chain
        /// </summary>
        private void BuildChain()
        {
            // Type[] ctrArgs, object[] ctrValues
            var ctrArgs = new[] {typeof (CheckoutBase)};
            var ctrValues = new object[] { _checkout };

            // Types from the merchello.config file
            var taskHandlers = new List<AttemptChainTaskHandler<IInvoice>>();
            var typeList = ChainTaskResolver.GetTypesForChain(Constants.TaskChainAlias.CheckoutInvoiceCreate).ToArray();
            if (!typeList.Any()) _taskHandlers = taskHandlers;

            // instantiate each task in the chain
            taskHandlers.AddRange(
                typeList.Select(
                typeName => new AttemptChainTaskHandler<IInvoice>(
                    ActivatorHelper.CreateInstance<CheckoutAttemptChainTaskBase>(Type.GetType(typeName), ctrArgs, ctrValues)
                    )));

            // RegisterNext
            foreach (var taskHandler in taskHandlers.Where(task => taskHandlers.IndexOf(task) != taskHandlers.IndexOf(taskHandlers.Last())))
            {
                taskHandler.RegisterNext(taskHandlers[taskHandlers.IndexOf(taskHandler) + 1]);
            }

            _taskHandlers = taskHandlers;
        }

        public Attempt<IInvoice> BuildInvoice()
        {
            return (_taskHandlers.Any())
                       ? _taskHandlers.First().Execute(new Invoice(Constants.DefaultKeys.UnpaidInvoiceStatusKey))
                       : Attempt<IInvoice>.Fail(
                           new InvalidOperationException("The configuration Chain Task List could not be instantiated"));
        }

        /// <summary>
        /// Used for testing
        /// </summary>
        internal int TaskCount
        {
            get { return _taskHandlers.Count(); }
        }
    }
}