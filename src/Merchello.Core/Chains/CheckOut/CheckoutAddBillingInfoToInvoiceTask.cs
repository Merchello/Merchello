using System.IO;
using Merchello.Core.Checkout;
using Merchello.Core.Models;
using Umbraco.Core;

namespace Merchello.Core.Chains.CheckOut
{
    /// <summary>
    /// Represents a task responsible for adding billing information collected a checkout process to the
    /// invoice.
    /// </summary>
    internal class CheckoutAddBillingInfoToInvoiceTask : CheckoutAttemptChainTaskBase
    {
        public CheckoutAddBillingInfoToInvoiceTask(CheckoutBase checkout) 
            : base(checkout)
        { }

        /// <summary>
        /// Adds billing information to the invoice
        /// </summary>
        /// <param name="value">The <see cref="IInvoice"/></param>
        /// <returns></returns>
        public override Attempt<IInvoice> PerformTask(IInvoice value)
        {
            var address = Checkout.Customer.ExtendedData.GetAddress(Constants.ExtendedDataKeys.BillingAddress);
            if (address == null) return Attempt<IInvoice>.Fail(new InvalidDataException("Billing information could not be retrieved from the Checkout"));

            value.SetBillingAddress(address);

            return Attempt<IInvoice>.Succeed(value);
            
        }
    }
}