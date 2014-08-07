namespace Merchello.Core.Gateways.Payment
{
    using Models;
    using Umbraco.Core;

    /// <summary>
    /// Defines a PaymentResult
    /// </summary>
    public interface IPaymentResult
    {
        /// <summary>
        /// Gets the <see cref="IPayment"/>
        /// </summary>
        Attempt<IPayment> Payment { get; }

        /// <summary>
        /// Gets the invoice
        /// </summary>
        IInvoice Invoice { get; }
        
        /// <summary>
        /// Gets a value indicating whether or not the sales preparation should generate the <see cref="IOrder"/> and <see cref="IShipment"/>(s)
        /// </summary>
        bool ApproveOrderCreation { get;  }
    }
}