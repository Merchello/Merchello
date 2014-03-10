using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Merchello.Core.Builders;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Gateways.Taxation;
using Merchello.Core.Services;
using Umbraco.Core;
using Umbraco.Core.Logging;

namespace Merchello.Core.Models
{

    /// <summary>
    /// Extension methods for <see cref="IInvoice"/>
    /// </summary>
    public static class InvoiceExtensions
    {
        /// <summary>
        /// Calculates taxes for the invoice
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/></param>
        /// <returns>The <see cref="ITaxCalculationResult"/> from the calculation</returns>
        public static ITaxCalculationResult CalculateTaxes(this IInvoice invoice)
        {
            return invoice.CalculateTaxes(invoice.GetBillingAddress());
        }

        /// <summary>
        /// Calculates taxes for the invoice
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/></param>
        /// <param name="taxAddress">The address (generally country code and region) to be used to determine the taxation rates</param>
        /// <returns>The <see cref="ITaxCalculationResult"/> from the calculation</returns>
        public static ITaxCalculationResult CalculateTaxes(this IInvoice invoice, IAddress taxAddress)
        {
            return invoice.CalculateTaxes(MerchelloContext.Current, taxAddress);
        }

        /// <summary>
        /// Calculates taxes for the invoice
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/></param>
        /// <param name="merchelloContext">The <see cref="IMerchelloContext"/></param>
        /// <param name="taxAddress">The address (generally country code and region) to be used to determine the taxation rates</param>
        /// <returns>The <see cref="ITaxCalculationResult"/> from the calculation</returns>
        public static ITaxCalculationResult CalculateTaxes(this IInvoice invoice, IMerchelloContext merchelloContext, IAddress taxAddress)
        {
            // remove any other tax lines
            return merchelloContext.Gateways.Taxation.CalculateTaxesForInvoice(invoice, taxAddress);
        }

        /// <summary>
        /// Returns a constructed invoice number (including it's invoice number prefix - if any)
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/></param>
        /// <returns>The prefixed invoice number</returns>
        public static string PrefixedInvoiceNumber(this IInvoice invoice)
        {
            return string.IsNullOrEmpty(invoice.InvoiceNumberPrefix)
                ? invoice.InvoiceNumber.ToString(CultureInfo.InvariantCulture)
                : string.Format("{0}-{1}", invoice.InvoiceNumberPrefix, invoice.InvoiceNumber);
        }

        /// <summary>
        /// Prepares an <see cref="IOrder"/> without saving it to the database.  
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/> to base the order on</param>
        /// <returns>The <see cref="IOrder"/></returns>
        public static IOrder PrepareOrder(this IInvoice invoice)
        {
            return invoice.PrepareOrder(MerchelloContext.Current);
        }

        /// <summary>
        /// Prepare an <see cref="IOrder"/> with saving it to the database
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/> to base the order or</param>
        /// <param name="merchelloContext">The <see cref="IMerchelloContext"/></param>
        /// <returns>The <see cref="IOrder"/></returns>
        public static IOrder PrepareOrder(this IInvoice invoice, IMerchelloContext merchelloContext)
        {
            return invoice.PrepareOrder(merchelloContext, new OrderBuilderChain(invoice));
        }

        /// <summary>
        /// Prepares an <see cref="IOrder"/> without saving it to the database.  
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/> to base the order on</param>
        /// <param name="merchelloContext">The <see cref="IMerchelloContext"/></param>
        /// <param name="orderBuilder">The <see cref="IBuilderChain{IOrder}"/></param>
        /// <returns>The <see cref="IOrder"/></returns>
        /// <remarks>
        /// 
        /// This method will save the invoice in the event it has not previously been saved
        /// 
        /// </remarks>
        public static IOrder PrepareOrder(this IInvoice invoice, IMerchelloContext merchelloContext, IBuilderChain<IOrder> orderBuilder)
        {
            if(!invoice.HasIdentity) merchelloContext.Services.InvoiceService.Save(invoice);

            var attempt = orderBuilder.Build();
            if (attempt.Success) return attempt.Result;

            LogHelper.Error<OrderBuilderChain>("Extension method PrepareOrder failed", attempt.Exception);
            throw attempt.Exception;
        }


        /// <summary>
        /// Returns a collection of <see cref="IAppliedPayment"/> for the invoice
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/></param>
        /// <returns>A collection of <see cref="IAppliedPayment"/></returns>
        public static IEnumerable<IAppliedPayment> AppliedPayments(this IInvoice invoice)
        {
            return invoice.AppliedPayments(MerchelloContext.Current);
        }

        /// <summary>
        /// Returns a collection of <see cref="IAppliedPayment"/> for this <see cref="IInvoice"/>
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/></param>
        /// <param name="merchelloContext">The <see cref="IMerchelloContext"/></param>
        /// <returns>A collection of <see cref="IAppliedPayment"/></returns>
        public static IEnumerable<IAppliedPayment> AppliedPayments(this IInvoice invoice, IMerchelloContext merchelloContext)
        {
            return invoice.AppliedPayments(merchelloContext.Services.GatewayProviderService);
        }

        /// <summary>
        /// Returns a collection of <see cref="IAppliedPayment"/> for this <see cref="IInvoice"/>
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/></param>
        /// <param name="gatewayProviderService">The <see cref="IGatewayProviderService"/></param>
        /// <returns>A collection of <see cref="IAppliedPayment"/></returns>
        public static IEnumerable<IAppliedPayment> AppliedPayments(this IInvoice invoice, IGatewayProviderService gatewayProviderService)
        {
            return gatewayProviderService.GetAppliedPaymentsByInvoiceKey(invoice.Key);
        }


        /// <summary>
        /// Attempts to process a payment
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/></param>
        /// <param name="paymentGatewayMethod">The <see cref="IPaymentGatewayMethod"/> to use in processing the payment</param>
        /// <param name="args">Additional arguements required by the payment processor</param>
        /// <returns>The <see cref="IPaymentResult"/></returns>
        public static IPaymentResult AuthorizePayment(this IInvoice invoice, IPaymentGatewayMethod paymentGatewayMethod, ProcessorArgumentCollection args)
        {
            Mandate.ParameterNotNull(paymentGatewayMethod, "paymentGatewayMethod");
            
            return paymentGatewayMethod.AuthorizePayment(invoice, args);
        }

        /// <summary>
        /// Attempts to process a payment
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/></param>
        /// <param name="paymentGatewayMethod">The <see cref="IPaymentGatewayMethod"/> to use in processing the payment</param>
        /// <returns>The <see cref="IPaymentResult"/></returns>
        public static IPaymentResult AuthorizePayment(this IInvoice invoice, IPaymentGatewayMethod paymentGatewayMethod)
        {
            Mandate.ParameterCondition(invoice.HasIdentity, "The invoice must be saved before a payment can be authorized.");
            Mandate.ParameterNotNull(paymentGatewayMethod, "paymentGatewayMethod");

            return invoice.AuthorizePayment(paymentGatewayMethod, new ProcessorArgumentCollection());
        }


        /// <summary>
        /// Attempts to process a payment
        /// </summary>
        /// <param name="invoice"></param>
        /// <param name="paymentMethodKey">The <see cref="IPaymentMethod"/> key</param>
        /// <param name="args">Additional arguements required by the payment processor</param>
        /// <returns>The <see cref="IPaymentResult"/></returns>
        public static IPaymentResult AuthorizePayment(this IInvoice invoice, Guid paymentMethodKey, ProcessorArgumentCollection args)
        {            
            return invoice.AuthorizePayment(MerchelloContext.Current, paymentMethodKey, args);
        }

        /// <summary>
        /// Attempts to process a payment
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/></param>
        /// <param name="merchelloContext">The <see cref="IMerchelloContext"/></param>
        /// <param name="paymentMethodKey">The <see cref="IPaymentMethod"/> key</param>
        /// <param name="args">Additional arguements required by the payment processor</param>
        /// <returns>The <see cref="IPaymentResult"/></returns>
        internal static IPaymentResult AuthorizePayment(this IInvoice invoice, IMerchelloContext merchelloContext, Guid paymentMethodKey, ProcessorArgumentCollection args)
        {

            var paymentMethod = merchelloContext.Gateways.Payment.GetPaymentGatewayMethodByKey(paymentMethodKey);
            return invoice.AuthorizePayment(paymentMethod, args);

        }

        /// <summary>
        /// Attempts to process a payment
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/></param>
        /// <param name="paymentMethodKey">The <see cref="IPaymentMethod"/> key</param>
        /// <returns>The <see cref="IPaymentResult"/></returns>
        public static IPaymentResult AuthorizePayment(this IInvoice invoice, Guid paymentMethodKey)
        {
            return invoice.AuthorizePayment(paymentMethodKey, new ProcessorArgumentCollection());
        }

        /// <summary>
        /// Authorizes and Captures a Payment
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/></param>
        /// <param name="paymentGatewayMethod">The <see cref="IPaymentMethod"/></param>
        /// <param name="args">Additional arguements required by the payment processor</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        public static IPaymentResult AuthorizeCapturePayment(this IInvoice invoice, IPaymentGatewayMethod paymentGatewayMethod, ProcessorArgumentCollection args)
        {
            Mandate.ParameterNotNull(paymentGatewayMethod, "paymentGatewayMethod");

            return paymentGatewayMethod.AuthorizeCapturePayment(invoice, invoice.Total, args);
        }

        /// <summary>
        /// Authorizes and Captures a Payment
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/></param>
        /// <param name="paymentGatewayMethod">The <see cref="IPaymentMethod"/></param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        public static IPaymentResult AuthorizeCapturePayment(this IInvoice invoice, IPaymentGatewayMethod paymentGatewayMethod)
        {
            return invoice.AuthorizeCapturePayment(paymentGatewayMethod, new ProcessorArgumentCollection());
        }

        /// <summary>
        /// Authorizes and Captures a Payment
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/></param>
        /// <param name="paymentMethodKey">The <see cref="IPaymentMethod"/> key</param>
        /// <param name="args">Additional arguements required by the payment processor</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        public static IPaymentResult AuthorizeCapturePayment(this IInvoice invoice, Guid paymentMethodKey, ProcessorArgumentCollection args)
        {
            return invoice.AuthorizeCapturePayment(MerchelloContext.Current, paymentMethodKey, args);
        }

        /// <summary>
        /// Authorizes and Captures a Payment
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/></param>
        /// <param name="merchelloContext">The <see cref="IMerchelloContext"/></param>
        /// <param name="paymentMethodKey">The <see cref="IPaymentMethod"/> key</param>
        /// <param name="args">Additional arguements required by the payment processor</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        internal static IPaymentResult AuthorizeCapturePayment(this IInvoice invoice, IMerchelloContext merchelloContext, Guid paymentMethodKey, ProcessorArgumentCollection args)
        {
            var paymentMethod = merchelloContext.Gateways.Payment.GetPaymentGatewayMethodByKey(paymentMethodKey);
            return invoice.AuthorizeCapturePayment(paymentMethod, args);
        }

        /// <summary>
        /// Authorizes and Captures a Payment
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/></param>
        /// <param name="paymentMethodKey">The <see cref="IPaymentMethod"/> key</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        public static IPaymentResult AuthorizeCapturePayment(this IInvoice invoice, Guid paymentMethodKey)
        {
            return invoice.AuthorizeCapturePayment(paymentMethodKey, new ProcessorArgumentCollection());
        }

        /// <summary>
        /// Captures a payment for the <see cref="IInvoice"/>
        /// </summary>
        /// <param name="invoice">The invoice to be payed</param>
        /// <param name="payment">The</param>
        /// <param name="amount">The amount to the payment to be captured</param>
        /// <param name="paymentGatewayMethod"></param>
        /// <param name="args">Additional arguements required by the payment processor</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        public static IPaymentResult CapturePayment(this IInvoice invoice, IPayment payment, IPaymentGatewayMethod paymentGatewayMethod, decimal amount, ProcessorArgumentCollection args)
        {
            return paymentGatewayMethod.CapturePayment(invoice, payment, amount, args);
        }

        /// <summary>
        /// Captures a payment for the <see cref="IInvoice"/>
        /// </summary>
        /// <param name="invoice">The invoice to be payed</param>
        /// <param name="payment">The</param>
        /// <param name="amount">The amount to the payment to be captured</param>
        /// <param name="paymentGatewayMethod"></param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        public static IPaymentResult CapturePayment(this IInvoice invoice, IPayment payment, IPaymentGatewayMethod paymentGatewayMethod, decimal amount)
        {
            return invoice.CapturePayment(payment, paymentGatewayMethod, amount, new ProcessorArgumentCollection());
        }

        /// <summary>
        /// Captures a payment for the <see cref="IInvoice"/>
        /// </summary>
        /// <param name="invoice">The invoice to be payed</param>
        /// <param name="payment">The</param>
        /// <param name="paymentMethodKey">The <see cref="IPaymentMethod"/> key</param>
        /// <param name="amount">The amount to the payment to be captured</param>
        /// <param name="args">Additional arguements required by the payment processor</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        public static IPaymentResult CapturePayment(this IInvoice invoice, IPayment payment, Guid paymentMethodKey, decimal amount, ProcessorArgumentCollection args)
        {
            return invoice.CapturePayment(MerchelloContext.Current, payment, paymentMethodKey, amount, args);
        }

        /// <summary>
        /// Captures a payment for the <see cref="IInvoice"/>
        /// </summary>
        /// <param name="invoice">The invoice to be payed</param>
        /// <param name="merchelloContext">The <see cref="IMerchelloContext"/></param>
        /// <param name="payment">The</param>
        /// <param name="paymentMethodKey">The <see cref="IPaymentMethod"/> key</param>
        /// <param name="amount">The amount to the payment to be captured</param>
        /// <param name="args">Additional arguements required by the payment processor</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        internal static IPaymentResult CapturePayment(this IInvoice invoice, IMerchelloContext merchelloContext, IPayment payment, Guid paymentMethodKey, decimal amount, ProcessorArgumentCollection args)
        {
            var paymentGatewayMethod = merchelloContext.Gateways.Payment.GetPaymentGatewayMethodByKey(paymentMethodKey);
            return invoice.CapturePayment(payment, paymentGatewayMethod, amount, args);
        }

        /// <summary>
        /// Refunds a payment
        /// </summary>
        /// <param name="invoice">The invoice to be the payment was applied</param>
        /// <param name="payment">The payment to be refunded</param>
        /// <param name="paymentGatewayMethod">The <see cref="IPaymentGatewayMethod"/></param>
        /// <param name="args">Additional arguements required by the payment processor</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        public static IPaymentResult RefundPayment(this IInvoice invoice, IPayment payment, IPaymentGatewayMethod paymentGatewayMethod, ProcessorArgumentCollection args)
        {
            return paymentGatewayMethod.RefundPayment(invoice, payment, args);
        }

        /// <summary>
        /// Refunds a payment
        /// </summary>
        /// <param name="invoice">The invoice to be the payment was applied</param>
        /// <param name="payment">The payment to be refunded</param>
        /// <param name="paymentGatewayMethod">The <see cref="IPaymentGatewayMethod"/></param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        public static IPaymentResult RefundPayment(this IInvoice invoice, IPayment payment, IPaymentGatewayMethod paymentGatewayMethod)
        {
            return invoice.RefundPayment(payment, paymentGatewayMethod, new ProcessorArgumentCollection());
        }

        /// <summary>
        /// Refunds a payment
        /// </summary>
        /// <param name="invoice">The invoice to be the payment was applied</param>
        /// <param name="payment">The payment to be refunded</param>
        /// <param name="paymentMethodKey">The key of the <see cref="IPaymentGatewayMethod"/></param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        public static IPaymentResult RefundPayment(this IInvoice invoice, IPayment payment, Guid paymentMethodKey)
        {
            return invoice.RefundPayment(payment, paymentMethodKey, new ProcessorArgumentCollection());
        }

        /// <summary>
        /// Refunds a payment
        /// </summary>
        /// <param name="invoice">The invoice to be the payment was applied</param>
        /// <param name="payment">The payment to be refunded</param>
        /// <param name="paymentMethodKey">The key of the <see cref="IPaymentGatewayMethod"/></param>
        /// <param name="args">Additional arguements required by the payment processor</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        public static IPaymentResult RefundPayment(this IInvoice invoice, IPayment payment, Guid paymentMethodKey, ProcessorArgumentCollection args)
        {
            return invoice.RefundPayment(MerchelloContext.Current, payment, paymentMethodKey, args);
        }

        /// <summary>
        /// Refunds a payment
        /// </summary>
        /// <param name="invoice">The invoice to be the payment was applied</param>
        /// <param name="merchelloContext">The <see cref="IMerchelloContext"/></param>
        /// <param name="payment">The payment to be refunded</param>
        /// <param name="paymentMethodKey">The key of the <see cref="IPaymentGatewayMethod"/></param>
        /// <param name="args">Additional arguements required by the payment processor</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        internal static IPaymentResult RefundPayment(this IInvoice invoice, IMerchelloContext merchelloContext, IPayment payment, Guid paymentMethodKey, ProcessorArgumentCollection args)
        {
            var paymentGatewayMethod = merchelloContext.Gateways.Payment.GetPaymentGatewayMethodByKey(paymentMethodKey);
            return invoice.RefundPayment(payment, paymentGatewayMethod, args);
        }
    }
}