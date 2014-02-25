using System;
using System.Collections.Generic;
using Merchello.Core.Models;

namespace Merchello.Core.Gateways.Payment
{
    /// <summary>
    /// Defines a payment gateway provider
    /// </summary>
    public interface IPaymentGatewayProvider : IProvider
    {
        /// <summary>
        /// Creates a <see cref="IPaymentGatewayMethod"/>
        /// </summary>
        /// <param name="name">The name of the payment method</param>
        /// <param name="description">The description of the payment method</param>
        /// <param name="paymentCode">The payment code of the payment method</param>
        /// <returns>A <see cref="IPaymentGatewayMethod"/></returns>
        IPaymentGatewayMethod CreatePaymentMethod(string name, string description, string paymentCode);

        /// <summary>
        /// Saves a <see cref="IPaymentGatewayMethod"/>
        /// </summary>
        /// <param name="method">The <see cref="IPaymentGatewayMethod"/> to be saved</param>
        void SavePaymentMethod(IPaymentGatewayMethod method);

        /// <summary>
        /// Deletes a <see cref="IPaymentGatewayMethod"/>
        /// </summary>
        /// <param name="method">The <see cref="IPaymentGatewayMethod"/> to delete</param>
        void DeletePaymentMethod(IPaymentGatewayMethod method);

        /// <summary>
        /// Gets a <see cref="IPaymentGatewayMethod"/> by it's unique 'key'
        /// </summary>
        /// <param name="paymentMethodKey">The key of the <see cref="IPaymentMethod"/></param>
        /// <returns>A <see cref="IPaymentGatewayMethod"/></returns>
        IPaymentGatewayMethod GetByKey(Guid paymentMethodKey);

        /// <summary>
        /// Gets a collection of all <see cref="IPaymentMethod"/>s associated with this provider
        /// </summary>
        IEnumerable<IPaymentMethod> PaymentMethods { get; }
    }
}