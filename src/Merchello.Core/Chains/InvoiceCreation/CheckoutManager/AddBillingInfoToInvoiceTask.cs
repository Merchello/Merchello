namespace Merchello.Core.Chains.InvoiceCreation.CheckoutManager
{
    using System.IO;

    using Merchello.Core.Checkout;
    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// Represents a task responsible for adding billing information collected a checkout process to the
    /// invoice.
    /// </summary>
    internal class AddBillingInfoToInvoiceTask : CheckoutManagerInvoiceCreationAttemptChainTaskBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddBillingInfoToInvoiceTask"/> class.
        /// </summary>
        /// <param name="checkoutManager">
        /// The checkout manager.
        /// </param>
        public AddBillingInfoToInvoiceTask(ICheckoutManagerBase checkoutManager)
            : base(checkoutManager)
        {
        }

        /// <summary>
        /// Adds billing information to the invoice
        /// </summary>
        /// <param name="value">
        /// The <see cref="IInvoice"/>
        /// </param>
        /// <returns>
        /// The <see cref="Attempt"/>.
        /// </returns>
        public override Attempt<IInvoice> PerformTask(IInvoice value)
        {
            var address = this.CheckoutManager.Context.Customer.ExtendedData.GetAddress(Core.Constants.ExtendedDataKeys.BillingAddress);
            if (address == null) return Attempt<IInvoice>.Fail(new InvalidDataException("Billing information could not be retrieved from the Checkout"));

            value.SetBillingAddress(address);

            return Attempt<IInvoice>.Succeed(value);
        }
    }
}