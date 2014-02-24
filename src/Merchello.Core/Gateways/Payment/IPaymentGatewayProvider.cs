using System.Collections.Generic;
using Merchello.Core.Models;

namespace Merchello.Core.Gateways.Payment
{
    /// <summary>
    /// Defines a payment gateway provider
    /// </summary>
    public interface IPaymentGatewayProvider : IGateway
    {
        /// <summary>
        /// Creates a <see cref="IPaymentMethod"/> for 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="paymentCode"></param>
        /// <returns></returns>
        IPaymentMethod CreatePaymentMethod(string name, string description, string paymentCode);

        void SavePaymentMethod(IPaymentMethod paymentMethod);

        void DeletePaymentMethod(IPaymentMethod paymentMethod);

        IEnumerable<IPaymentMethod> PaymentMethods { get; }

    }
}