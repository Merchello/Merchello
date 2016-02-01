namespace Merchello.Core.Chains.InvoiceCreation.SalesPreparation
{
    using System;
    using System.IO;

    using Merchello.Core.Models;
    using Merchello.Core.Sales;

    using Umbraco.Core;

    /// <summary>
    /// Represents a task responsible for adding billing information collected a checkout process to the
    /// invoice.
    /// </summary>
    [Obsolete("Superseded by CheckoutManger.AddBillingInfoToInvoiceTask")]
    internal class AddBillingInfoToInvoiceTask : InvoiceCreationAttemptChainTaskBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddBillingInfoToInvoiceTask"/> class.
        /// </summary>
        /// <param name="salePreparation">
        /// The sale preparation.
        /// </param>
        public AddBillingInfoToInvoiceTask(SalePreparationBase salePreparation)
            : base(salePreparation)
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
            var address = this.SalePreparation.Customer.ExtendedData.GetAddress(Core.Constants.ExtendedDataKeys.BillingAddress);
            if (address == null) return Attempt<IInvoice>.Fail(new InvalidDataException("Billing information could not be retrieved from the Checkout"));

            value.SetBillingAddress(address);

            return Attempt<IInvoice>.Succeed(value);            
        }
    }
}