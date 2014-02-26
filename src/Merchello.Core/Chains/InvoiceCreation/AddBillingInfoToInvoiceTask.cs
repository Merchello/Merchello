using System.IO;
using Merchello.Core.Models;
using Merchello.Core.Sales;
using Umbraco.Core;

namespace Merchello.Core.Chains.InvoiceCreation
{
    /// <summary>
    /// Represents a task responsible for adding billing information collected a checkout process to the
    /// invoice.
    /// </summary>
    internal class AddBillingInfoToInvoiceTask : OrderPreparationAttemptChainTaskBase
    {
        public AddBillingInfoToInvoiceTask(SalesManagerBase salesManager) 
            : base(salesManager)
        { }

        /// <summary>
        /// Adds billing information to the invoice
        /// </summary>
        /// <param name="value">The <see cref="IInvoice"/></param>
        /// <returns></returns>
        public override Attempt<IInvoice> PerformTask(IInvoice value)
        {
            var address = SalesManager.Customer.ExtendedData.GetAddress(Constants.ExtendedDataKeys.BillingAddress);
            if (address == null) return Attempt<IInvoice>.Fail(new InvalidDataException("Billing information could not be retrieved from the Checkout"));

            value.SetBillingAddress(address);

            return Attempt<IInvoice>.Succeed(value);
            
        }
    }
}