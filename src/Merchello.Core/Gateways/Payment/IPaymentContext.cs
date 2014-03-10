using System;
using System.Collections.Generic;

namespace Merchello.Core.Gateways.Payment
{
    /// <summary>
    /// Defines the Payment Context
    /// </summary>
    public interface IPaymentContext : IGatewayProviderTypedContextBase<PaymentGatewayProviderBase>
    {
        /// <summary>
        /// Gets a list of all possible Payment Methods
        /// </summary>
        /// <returns>A collection of <see cref="IPaymentGatewayMethod"/>s</returns>
        IEnumerable<IPaymentGatewayMethod> GetPaymentGatewayMethods();

        /// <summary>
        /// Gets a <see cref="IPaymentGatewayMethod"/> by its unique key
        /// </summary>
        /// <param name="paymentMethodKey">The Key of the <see cref="IPaymentGatewayMethod"/></param>
        /// <returns>A <see cref="IPaymentGatewayMethod"/></returns>
        IPaymentGatewayMethod GetPaymentGatewayMethodByKey(Guid paymentMethodKey);
    }
}