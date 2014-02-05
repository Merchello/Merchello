using System;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Chains;
using Merchello.Core.Chains.CheckOut;
using Merchello.Core.Checkout;
using Merchello.Core.Models;

namespace Merchello.Core.Builders
{
    internal class InvoiceBuilder : IInvoiceBuilder
    {
        private readonly CheckoutBase _checkout;
        private IEnumerable<CheckoutAttemptChainTaskBase> _tasks; 

        internal InvoiceBuilder(CheckoutBase checkout)
        {
            Mandate.ParameterNotNull(checkout, "checkout");

            _checkout = checkout;

            BuildChain();
        }

        private void BuildChain()
        {
            // Type[] ctrArgs, object[] ctrValues
            var ctrArgs = new[] {typeof (CheckoutBase)};
            var ctrValues = new object[] { _checkout };
                               
            var tasks = new List<CheckoutAttemptChainTaskBase>();
            var typeList = ChainTaskResolver.GetTypesForChain(Constants.TaskChainAlias.CheckoutInvoiceCreate).ToArray();
            if(!typeList.Any()) _tasks = new List<CheckoutAttemptChainTaskBase>();

            tasks.AddRange(typeList.Select(type => ActivatorHelper.CreateInstance<CheckoutAttemptChainTaskBase>(Type.GetType(type), ctrArgs, ctrValues)));
            
            _tasks = tasks;
        }

        public IInvoice BuildInvoice()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Used for testing
        /// </summary>
        internal int TaskCount
        {
            get { return _tasks.Count(); }
        }
    }
}