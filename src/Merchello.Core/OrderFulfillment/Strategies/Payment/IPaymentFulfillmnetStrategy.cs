using Merchello.Core.Models;
using Merchello.Core.Services;

namespace Merchello.Core.OrderFulfillment.Strategies.Payment
{
    /// <summary>
    /// Defines a payment fulfillment strategy
    /// </summary>
    interface IPaymentFulfillmnetStrategy : IFulfillmentStrategy
    {
        /// <summary>
        /// True/false whether or not to raise events
        /// </summary>
        bool RaiseEvents { get; }

        /// <summary>
        /// The transaction type
        /// </summary>
        TransactionType TransactionType { get; }

        /// <summary>
        /// The payment service
        /// </summary>
        IPaymentService PaymentService { get; }

        /// <summary>
        /// The transaction service
        /// </summary>
        ITransactionService TransactionService { get; }

        /// <summary>
        /// The InvoiceService
        /// </summary>
        IInvoiceService InvoiceService { get; }
    }
}
