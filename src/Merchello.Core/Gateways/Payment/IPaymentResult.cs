using Merchello.Core.Models;
using Umbraco.Core;

namespace Merchello.Core.Gateways.Payment
{
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
        /// True/false indicating whether or not the <see cref="ISalesManager"/> should generate the <see cref="IOrder"/> and <see cref="IShipment"/>(s)
        /// </summary>
        bool ApproveOrderCreation { get;  }
    }
}