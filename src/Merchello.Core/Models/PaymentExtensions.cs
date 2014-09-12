namespace Merchello.Core.Models
{
    using System;
    using System.Collections.Generic;
    using Gateways.Payment;
    using Services;

    /// <summary>
    /// Extension methods for <see cref="IPayment"/>
    /// </summary>
    public static class PaymentExtensions
    {
        /// <summary>
        /// Returns a collection of <see cref="IAppliedPayment"/> for this <see cref="IPayment"/>
        /// </summary>
        /// <param name="payment">The <see cref="IPayment"/></param>
        /// <returns>A collection of <see cref="IAppliedPayment"/></returns>
        public static IEnumerable<IAppliedPayment> AppliedPayments(this IPayment payment)
        {
            return payment.AppliedPayments(MerchelloContext.Current);
        }

        /// <summary>
        /// Returns a collection of <see cref="IAppliedPayment"/> for this <see cref="IPayment"/>
        /// </summary>
        /// <param name="payment">The <see cref="IPayment"/></param>
        /// <param name="merchelloContext">The <see cref="IMerchelloContext"/></param>
        /// <returns>A collection of <see cref="IAppliedPayment"/></returns>
        internal static IEnumerable<IAppliedPayment> AppliedPayments(this IPayment payment, IMerchelloContext merchelloContext)
        {
            return payment.AppliedPayments(merchelloContext.Services.GatewayProviderService);
        }

        /// <summary>
        /// Returns a collection of <see cref="IAppliedPayment"/> for this <see cref="IPayment"/>
        /// </summary>
        /// <param name="payment">The <see cref="IPayment"/></param>
        /// <param name="gatewayProviderService">The <see cref="IGatewayProviderService"/></param>
        /// <returns>A collection of <see cref="IAppliedPayment"/></returns>
        public static IEnumerable<IAppliedPayment> AppliedPayments(this IPayment payment, IGatewayProviderService gatewayProviderService)
        {
            return gatewayProviderService.GetAppliedPaymentsByPaymentKey(payment.Key);
        }

        /// <summary>
        /// Returns a collection of <see cref="IInvoice"/>s this <see cref="IPayment"/> has been applied to
        /// </summary>
        /// <param name="payment">The <see cref="IPayment"/></param>
        /// <returns>A collection of <see cref="IInvoice"/></returns>
        public static IEnumerable<IInvoice> AppliedToInvoices(this IPayment payment)
        {
            return payment.AppliedToInvoices(MerchelloContext.Current);
        }

        /// <summary>
        /// Returns a collection of <see cref="IInvoice"/>s this <see cref="IPayment"/> has been applied to
        /// </summary>
        /// <param name="payment">The <see cref="IPayment"/></param>
        /// <param name="merchelloContext">The <see cref="IMerchelloContext"/></param>
        /// <returns>A collection of <see cref="IInvoice"/></returns>
        internal static IEnumerable<IInvoice> AppliedToInvoices(this IPayment payment, IMerchelloContext merchelloContext)
        {
            return merchelloContext.Services.InvoiceService.GetInvoicesByPaymentKey(payment.Key);
        }

        /// <summary>
        /// Captures a payment for the <see cref="IInvoice"/>
        /// </summary>
        /// <param name="invoice">The invoice to be payed</param>
        /// <param name="payment">The</param>
        /// <param name="amount">The amount to the payment to be captured</param>
        /// <param name="paymentGatewayMethod">The <see cref="IPaymentGatewayMethod"/> to process the payment</param>
        /// <param name="args">Additional arguements required by the payment processor</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        public static IPaymentResult CapturePayment(this IPayment payment, IInvoice invoice, IPaymentGatewayMethod paymentGatewayMethod, decimal amount, ProcessorArgumentCollection args)
        {
            return paymentGatewayMethod.CapturePayment(invoice, payment, amount, args);
        }

        /// <summary>
        /// Captures a payment for the <see cref="IInvoice"/>
        /// </summary>
        /// <param name="invoice">The invoice to be payed</param>
        /// <param name="payment">The</param>
        /// <param name="amount">The amount to the payment to be captured</param>
        /// <param name="paymentGatewayMethod">The <see cref="IPaymentGatewayMethod"/> to process the payment</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        public static IPaymentResult CapturePayment(this IPayment payment, IInvoice invoice, IPaymentGatewayMethod paymentGatewayMethod, decimal amount)
        {
            return payment.CapturePayment(invoice, paymentGatewayMethod, amount, new ProcessorArgumentCollection());
        }

        /// <summary>
        /// Captures a payment for the <see cref="IInvoice"/>
        /// </summary>
        /// <param name="invoice">The invoice to be payed</param>
        /// <param name="payment">The</param>
        /// <param name="paymentMethodKey"></param>
        /// <param name="amount">The amount to the payment to be captured</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        public static IPaymentResult CapturePayment(this IPayment payment, IInvoice invoice, Guid paymentMethodKey, decimal amount)
        {
            return payment.CapturePayment(invoice, paymentMethodKey, amount, new ProcessorArgumentCollection());
        }

        /// <summary>
        /// Captures a payment for the <see cref="IInvoice"/>
        /// </summary>
        /// <param name="invoice">The invoice to be payed</param>
        /// <param name="payment">The</param>
        /// <param name="paymentMethodKey"></param>
        /// <param name="amount">The amount to the payment to be captured</param>
        /// <param name="args">Additional arguements required by the payment processor</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        public static IPaymentResult CapturePayment(this IPayment payment, IInvoice invoice, Guid paymentMethodKey, decimal amount, ProcessorArgumentCollection args)
        {
            return payment.CapturePayment(MerchelloContext.Current, invoice, paymentMethodKey, amount, args);
        }

        /// <summary>
        /// Captures a payment for the <see cref="IInvoice"/>
        /// </summary>
        /// <param name="merchelloContext">The <see cref="IMerchelloContext"/></param>
        /// <param name="invoice">The invoice to be payed</param>
        /// <param name="payment">The</param>
        /// <param name="paymentMethodKey"></param>
        /// <param name="amount">The amount to the payment to be captured</param>
        /// <param name="args">Additional arguements required by the payment processor</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        internal static IPaymentResult CapturePayment(this IPayment payment, IMerchelloContext merchelloContext, IInvoice invoice, Guid paymentMethodKey, decimal amount, ProcessorArgumentCollection args)
        {
            var paymentGatewayMethod = merchelloContext.Gateways.Payment.GetPaymentGatewayMethodByKey(paymentMethodKey);
            return payment.CapturePayment(invoice, paymentGatewayMethod, amount, args);
        }

        /// <summary>
        /// Refunds a payment
        /// </summary>
        /// <param name="invoice">The invoice to be the payment was applied</param>
        /// <param name="payment">The payment to be refunded</param>
        /// <param name="paymentGatewayMethod">The <see cref="IPaymentGatewayMethod"/></param>
        /// <param name="amount">The amount to be refunded</param>
        /// <param name="args">Additional arguements required by the payment processor</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        public static IPaymentResult RefundPayment(this IPayment payment, IInvoice invoice, IPaymentGatewayMethod paymentGatewayMethod, decimal amount, ProcessorArgumentCollection args)
        {
            return paymentGatewayMethod.RefundPayment(invoice, payment, amount, args);
        }

        /// <summary>
        /// Refunds a payment
        /// </summary>
        /// <param name="invoice">The invoice to be the payment was applied</param>
        /// <param name="payment">The payment to be refunded</param>
        /// <param name="paymentGatewayMethod">The <see cref="IPaymentGatewayMethod"/></param>
        /// <param name="amount">The amount to be refunded</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        public static IPaymentResult RefundPayment(this IPayment payment, IInvoice invoice, IPaymentGatewayMethod paymentGatewayMethod, decimal amount)
        {
            return payment.RefundPayment(invoice, paymentGatewayMethod, amount, new ProcessorArgumentCollection());
        }

        /// <summary>
        /// Refunds a payment
        /// </summary>
        /// <param name="invoice">The invoice to be the payment was applied</param>
        /// <param name="payment">The payment to be refunded</param>
        /// <param name="paymentMethodKey">The key of the <see cref="IPaymentGatewayMethod"/></param>
        /// <param name="amount">The amount to be refunded</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        public static IPaymentResult RefundPayment(this IPayment payment, IInvoice invoice, Guid paymentMethodKey, decimal amount)
        {
            return payment.RefundPayment(invoice, paymentMethodKey, amount, new ProcessorArgumentCollection());
        }

        /// <summary>
        /// Refunds a payment
        /// </summary>
        /// <param name="invoice">The invoice to be the payment was applied</param>
        /// <param name="payment">The payment to be refunded</param>
        /// <param name="paymentMethodKey">The key of the <see cref="IPaymentGatewayMethod"/></param>
        /// <param name="amount">The amount to be refunded</param>
        /// <param name="args">Additional arguements required by the payment processor</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        public static IPaymentResult RefundPayment(this IPayment payment, IInvoice invoice, Guid paymentMethodKey, decimal amount, ProcessorArgumentCollection args)
        {
            return payment.RefundPayment(MerchelloContext.Current, invoice, paymentMethodKey, amount, args);
        }

        /// <summary>
        /// Refunds a payment
        /// </summary>
        /// <param name="invoice">The invoice to be the payment was applied</param>
        /// <param name="merchelloContext">The <see cref="IMerchelloContext"/></param>
        /// <param name="payment">The payment to be refunded</param>
        /// <param name="paymentMethodKey">The key of the <see cref="IPaymentGatewayMethod"/></param>
        /// <param name="amount">The amount to be refunded</param>
        /// <param name="args">Additional arguements required by the payment processor</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        internal static IPaymentResult RefundPayment(this IPayment payment, IMerchelloContext merchelloContext, IInvoice invoice, Guid paymentMethodKey, decimal amount, ProcessorArgumentCollection args)
        {
            var paymentGatewayMethod = merchelloContext.Gateways.Payment.GetPaymentGatewayMethodByKey(paymentMethodKey);
            return payment.RefundPayment(invoice, paymentGatewayMethod, amount, args);
        }

        /// <summary>
        /// Voids a payment
        /// </summary>
        /// <param name="invoice">The invoice to be the payment was applied</param>
        /// <param name="payment">The payment to be voided</param>
        /// <param name="paymentGatewayMethod">The <see cref="IPaymentGatewayMethod"/></param>
        /// <param name="args">Additional arguements required by the payment processor</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        public static IPaymentResult VoidPayment(this IPayment payment, IInvoice invoice,
            IPaymentGatewayMethod paymentGatewayMethod, ProcessorArgumentCollection args)
        {
            return paymentGatewayMethod.VoidPayment(invoice, payment, args);
        }

        /// <summary>
        /// Voids a payment
        /// </summary>
        /// <param name="invoice">The invoice to be the payment was applied</param>
        /// <param name="payment">The payment to be voided</param>
        /// <param name="paymentMethodKey">The <see cref="IPaymentGatewayMethod"/> key</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        public static IPaymentResult VoidPayment(this IPayment payment, IInvoice invoice,  Guid paymentMethodKey)
        {
            return payment.VoidPayment(invoice, paymentMethodKey, new ProcessorArgumentCollection());
        }

        /// <summary>
        /// Voids a payment
        /// </summary>
        /// <param name="invoice">The invoice to be the payment was applied</param>
        /// <param name="payment">The payment to be voided</param>
        /// <param name="paymentMethodKey">The <see cref="IPaymentGatewayMethod"/> key</param>
        /// <param name="args">Additional arguements required by the payment processor</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        public static IPaymentResult VoidPayment(this IPayment payment, IInvoice invoice, Guid paymentMethodKey, ProcessorArgumentCollection args)
        {
            return payment.VoidPayment(MerchelloContext.Current, invoice, paymentMethodKey, args);
        }

        /// <summary>
        /// Voids a payment
        /// </summary>
        /// <param name="invoice">The invoice to be the payment was applied</param>
        /// <param name="merchelloContext">The <see cref="IMerchelloContext"/></param>
        /// <param name="payment">The payment to be voided</param>
        /// <param name="paymentMethodKey">The <see cref="IPaymentGatewayMethod"/> key</param>
        /// <param name="args">Additional arguements required by the payment processor</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        internal static IPaymentResult VoidPayment(this IPayment payment, IMerchelloContext merchelloContext, IInvoice invoice, Guid paymentMethodKey, ProcessorArgumentCollection args)
        {
            var paymentGatewayMethod = merchelloContext.Gateways.Payment.GetPaymentGatewayMethodByKey(paymentMethodKey);
            return paymentGatewayMethod.VoidPayment(invoice, payment, args);
        }

    }
}