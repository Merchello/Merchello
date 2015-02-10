namespace Merchello.Core.Gateways.Payment
{
    using Merchello.Core.Models;

    /// <summary>
    /// The payment operation event data.
    /// </summary>
    public class PaymentOperationData
    {
        /// <summary>
        /// Gets or sets the invoice.
        /// </summary>
        public IInvoice Invoice { get; set; }

        /// <summary>
        /// Gets or sets the payment.
        /// </summary>
        public IPayment Payment { get; set; }

        /// <summary>
        /// Gets or sets the amount.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the processor argument collection.
        /// </summary>
        public ProcessorArgumentCollection ProcessorArgumentCollection { get; set; }
    }
}