namespace Merchello.Core.Checkout
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Builders;
    using Merchello.Core.Gateways.Payment;
    using Merchello.Core.Models;

    /// <summary>
    /// Defines a manager that is responsible for the payment aspects of the checkout process.
    /// </summary>
    public interface ICheckoutPaymentManager
    {
        /// <summary>
        /// True/false indicating whether or not the <see cref="ICheckoutPaymentManager"/> is ready to prepare an <see cref="IInvoice"/>
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool IsReadyToInvoice();

        /// <summary>
        /// Generates an <see cref="IInvoice"/>
        /// </summary>
        /// <returns>An <see cref="IInvoice"/></returns>
        IInvoice PrepareInvoice();

        /// <summary>
        /// Generates an <see cref="IInvoice"/> representing the bill for the current "checkout order"
        /// </summary>
        /// <param name="invoiceBuilder">The invoice builder class</param>
        /// <returns>An <see cref="IInvoice"/> that is not persisted to the database.</returns>
        IInvoice PrepareInvoice(IBuilderChain<IInvoice> invoiceBuilder);

        /// <summary>
        /// Removes a previously saved payment method.
        /// </summary>
        void ClearPaymentMethod();

        /// <summary>
        /// Saves a <see cref="IPaymentMethod"/> to <see cref="ICustomerBase"/> extended data
        /// </summary>
        /// <param name="paymentMethod">
        /// The payment Method.
        /// </param>
        void SavePaymentMethod(IPaymentMethod paymentMethod);

        /// <summary>
        /// Gets a list of all possible Payment Methods
        /// </summary>
        /// <returns>A collection of <see cref="IPaymentGatewayMethod"/>s</returns>
        IEnumerable<IPaymentGatewayMethod> GetPaymentGatewayMethods();

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