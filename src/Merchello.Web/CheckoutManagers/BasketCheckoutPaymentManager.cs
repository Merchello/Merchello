namespace Merchello.Web.CheckoutManagers
{
    using System;
    using System.Linq;

    using Core;

    using Merchello.Core.Builders;
    using Merchello.Core.Checkout;
    using Merchello.Core.Gateways.Payment;
    using Merchello.Core.Models;

    using Umbraco.Core;

    using Constants = Merchello.Core.Constants;

    /// <summary>
    /// The basket checkout payment manager.
    /// </summary>
    internal class BasketCheckoutPaymentManager : CheckoutPaymentManagerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BasketCheckoutPaymentManager"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="invoiceBuilder">
        ///  A lazy BuilderChain.
        /// </param>
        public BasketCheckoutPaymentManager(ICheckoutContext context, IBuilderChain<IInvoice> invoiceBuilder)
            : base(context, invoiceBuilder)
        {
        }

        /// <summary>
        /// Removes a previously saved payment method..
        /// </summary>
        [Obsolete("Use Reset()")]
        public override void ClearPaymentMethod()
        {
            Reset();
        }

        /// <summary>
        /// Saves a <see cref="IPaymentMethod"/> to <see cref="ICustomerBase"/> extended data
        /// </summary>
        /// <param name="paymentMethod">
        /// The payment Method.
        /// </param>
        public override void SavePaymentMethod(IPaymentMethod paymentMethod)
        {
            this.Context.Customer.ExtendedData.AddPaymentMethod(paymentMethod);
            this.SaveCustomer();
        }

        /// <summary>
        /// Gets a <see cref="IPaymentMethod"/> from <see cref="ICustomerBase"/> extended data
        /// </summary>
        /// <returns>
        /// The previously saved <see cref="IPaymentMethod"/>.
        /// </returns>
        public override IPaymentMethod GetPaymentMethod()
        {
            var paymentMethodKey = this.Context.Customer.ExtendedData.GetPaymentMethodKey();
            var paymentMethod = this.Context.Gateways.Payment.GetPaymentGatewayMethodByKey(paymentMethodKey);
            return paymentMethodKey.Equals(Guid.Empty) || paymentMethod == null ? null : paymentMethod.PaymentMethod;
        }

        /// <summary>
        /// Attempts to process a payment
        /// </summary>
        /// <param name="paymentGatewayMethod">The <see cref="IPaymentGatewayMethod"/> to use in processing the payment</param>
        /// <param name="args">Additional arguments required by the payment processor</param>
        /// <returns>The <see cref="IPaymentResult"/></returns>
        public override IPaymentResult AuthorizePayment(IPaymentGatewayMethod paymentGatewayMethod, ProcessorArgumentCollection args)
        {
            Ensure.ParameterNotNull(paymentGatewayMethod, "paymentGatewayMethod");

            if (!this.IsReadyToInvoice()) return new PaymentResult(Attempt<IPayment>.Fail(new InvalidOperationException("SalesPreparation is not ready to invoice")), null, false);

            // invoice
            var invoice = this.PrepareInvoice(this.InvoiceBuilder);

            this.Context.Services.InvoiceService.Save(invoice);

            var result = invoice.AuthorizePayment(paymentGatewayMethod, args);

            if (result.Payment.Success && this.Context.Settings.EmptyBasketOnPaymentSuccess) this.Context.Customer.Basket().Empty();
            this.OnFinalizing(result);

            return result;
        }

        /// <summary>
        /// Attempts to process a payment
        /// </summary>
        /// <param name="paymentGatewayMethod">The <see cref="IPaymentGatewayMethod"/> to use in processing the payment</param>
        /// <returns>The <see cref="IPaymentResult"/></returns>
        public override IPaymentResult AuthorizePayment(IPaymentGatewayMethod paymentGatewayMethod)
        {
            return this.AuthorizePayment(paymentGatewayMethod, new ProcessorArgumentCollection());
        }

        /// <summary>
        /// Attempts to process a payment
        /// </summary>
        /// <param name="paymentMethodKey">The <see cref="IPaymentMethod"/> key</param>
        /// <param name="args">Additional arguments required by the payment processor</param>
        /// <returns>The <see cref="IPaymentResult"/></returns>
        public override IPaymentResult AuthorizePayment(Guid paymentMethodKey, ProcessorArgumentCollection args)
        {
            var paymentMethod = this.Context.Gateways.Payment.GetPaymentGatewayMethods().FirstOrDefault(x => x.PaymentMethod.Key.Equals(paymentMethodKey));

            return this.AuthorizePayment(paymentMethod, args);
        }

        /// <summary>
        /// Attempts to process a payment
        /// </summary>
        /// <param name="paymentMethodKey">The <see cref="IPaymentMethod"/> key</param>
        /// <returns>The <see cref="IPaymentResult"/></returns>
        public override IPaymentResult AuthorizePayment(Guid paymentMethodKey)
        {
            return this.AuthorizePayment(paymentMethodKey, new ProcessorArgumentCollection());
        }

        /// <summary>
        /// Authorizes and Captures a Payment
        /// </summary>
        /// <param name="paymentGatewayMethod">The <see cref="IPaymentMethod"/></param>
        /// <param name="args">Additional arguments required by the payment processor</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        public override IPaymentResult AuthorizeCapturePayment(IPaymentGatewayMethod paymentGatewayMethod, ProcessorArgumentCollection args)
        {
            Ensure.ParameterNotNull(paymentGatewayMethod, "paymentGatewayMethod");

            if (!this.IsReadyToInvoice()) return new PaymentResult(Attempt<IPayment>.Fail(new InvalidOperationException("SalesPreparation is not ready to invoice")), null, false);

            // invoice
            var invoice = this.PrepareInvoice(this.InvoiceBuilder);

            this.Context.Services.InvoiceService.Save(invoice);

            var result = invoice.AuthorizeCapturePayment(paymentGatewayMethod, args);

            if (result.Payment.Success && this.Context.Settings.EmptyBasketOnPaymentSuccess) this.Context.Customer.Basket().Empty();

            this.OnFinalizing(result);

            return result;
        }

        /// <summary>
        /// Authorizes and Captures a Payment
        /// </summary>
        /// <param name="paymentGatewayMethod">The <see cref="IPaymentMethod"/></param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        public override IPaymentResult AuthorizeCapturePayment(IPaymentGatewayMethod paymentGatewayMethod)
        {
            return this.AuthorizeCapturePayment(paymentGatewayMethod, new ProcessorArgumentCollection());
        }

        /// <summary>
        /// Authorizes and Captures a Payment
        /// </summary>
        /// <param name="paymentMethodKey">The <see cref="IPaymentMethod"/> key</param>
        /// <param name="args">Additional arguments required by the payment processor</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        public override IPaymentResult AuthorizeCapturePayment(Guid paymentMethodKey, ProcessorArgumentCollection args)
        {
            var paymentMethod = this.Context.Gateways.Payment.GetPaymentGatewayMethods().FirstOrDefault(x => x.PaymentMethod.Key.Equals(paymentMethodKey));

            return this.AuthorizeCapturePayment(paymentMethod, args);
        }

        /// <summary>
        /// Authorizes and Captures a Payment
        /// </summary>
        /// <param name="paymentMethodKey">The <see cref="IPaymentMethod"/> key</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        public override IPaymentResult AuthorizeCapturePayment(Guid paymentMethodKey)
        {
            return this.AuthorizeCapturePayment(paymentMethodKey, new ProcessorArgumentCollection());
        }

        /// <summary>
        /// Clears the payment method.
        /// </summary>
        public override void Reset()
        {
            this.Context.Customer.ExtendedData.RemoveValue(Constants.ExtendedDataKeys.PaymentMethod);
            this.SaveCustomer();
        }
    }
}