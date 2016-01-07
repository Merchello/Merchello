namespace Merchello.Core.Checkout
{
    using System;

    using Merchello.Core.Gateways.Payment;
    using Merchello.Core.Models;

    /// <summary>
    /// Defines a manager that is responsible for the payment aspects of the checkout process.
    /// </summary>
    public interface ICheckoutPaymentManager
    {
        /// <summary>
        /// Saves a <see cref="IPaymentMethod"/> to <see cref="ICustomerBase"/> extended data
        /// </summary>
        /// <param name="paymentMethod">
        /// The payment Method.
        /// </param>
        void SavePaymentMethod(IPaymentMethod paymentMethod);

        /// <summary>
        /// Gets a <see cref="IPaymentMethod"/> from <see cref="ICustomerBase"/> extended data
        /// </summary>
        /// <returns>
        /// The previously saved <see cref="IPaymentMethod"/>.
        /// </returns>
        IPaymentMethod GetPaymentMethod();

        /// <summary>
        /// Attempts to process a payment
        /// </summary>
        /// <param name="paymentGatewayMethod">The <see cref="IPaymentGatewayMethod"/> to use in processing the payment</param>
        /// <param name="args">Additional arguments required by the payment processor</param>
        /// <returns>The <see cref="IPaymentResult"/></returns>
        IPaymentResult AuthorizePayment(IPaymentGatewayMethod paymentGatewayMethod, ProcessorArgumentCollection args);

        /// <summary>
        /// Attempts to process a payment
        /// </summary>
        /// <param name="paymentGatewayMethod">The <see cref="IPaymentGatewayMethod"/> to use in processing the payment</param>
        /// <returns>The <see cref="IPaymentResult"/></returns>
        IPaymentResult AuthorizePayment(IPaymentGatewayMethod paymentGatewayMethod);

        /// <summary>
        /// Attempts to process a payment
        /// </summary>
        /// <param name="paymentMethodKey">The <see cref="IPaymentMethod"/> key</param>
        /// <param name="args">Additional arguments required by the payment processor</param>
        /// <returns>The <see cref="IPaymentResult"/></returns>
        IPaymentResult AuthorizePayment(Guid paymentMethodKey, ProcessorArgumentCollection args);

        /// <summary>
        /// Attempts to process a payment
        /// </summary>
        /// <param name="paymentMethodKey">The <see cref="IPaymentMethod"/> key</param>
        /// <returns>The <see cref="IPaymentResult"/></returns>
        IPaymentResult AuthorizePayment(Guid paymentMethodKey);

        /// <summary>
        /// Authorizes and Captures a Payment
        /// </summary>
        /// <param name="paymentGatewayMethod">The <see cref="IPaymentMethod"/></param>
        /// <param name="args">Additional arguments required by the payment processor</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        IPaymentResult AuthorizeCapturePayment(IPaymentGatewayMethod paymentGatewayMethod, ProcessorArgumentCollection args);

        /// <summary>
        /// Authorizes and Captures a Payment
        /// </summary>
        /// <param name="paymentGatewayMethod">The <see cref="IPaymentMethod"/></param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        IPaymentResult AuthorizeCapturePayment(IPaymentGatewayMethod paymentGatewayMethod);

        /// <summary>
        /// Authorizes and Captures a Payment
        /// </summary>
        /// <param name="paymentMethodKey">The <see cref="IPaymentMethod"/> key</param>
        /// <param name="args">Additional arguments required by the payment processor</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        IPaymentResult AuthorizeCapturePayment(Guid paymentMethodKey, ProcessorArgumentCollection args);

        /// <summary>
        /// Authorizes and Captures a Payment
        /// </summary>
        /// <param name="paymentMethodKey">The <see cref="IPaymentMethod"/> key</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        IPaymentResult AuthorizeCapturePayment(Guid paymentMethodKey);
    }
}