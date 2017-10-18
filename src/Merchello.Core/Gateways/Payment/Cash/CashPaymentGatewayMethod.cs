﻿namespace Merchello.Core.Gateways.Payment.Cash
{
    using System;
    using System.Linq;
    using Models;
    using Services;
    using Umbraco.Core;

    /// <summary>
    /// Represents a CashPaymentMethod
    /// </summary>    
    [GatewayMethodUi("CashPaymentMethod")]
    [GatewayMethodEditor("Cash Method Editor", "~/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/payment.paymentmethod.addedit.html")]
    [PaymentGatewayMethod("Cash Payment Gateway Method Editors",
        "~/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/payment.cashpaymentmethod.authorizepayment.html", 
        "~/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/payment.cashpaymentmethod.authorizecapturepayment.html", 
        "~/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/payment.cashpaymentmethod.voidpayment.html", 
        "~/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/payment.cashpaymentmethod.refundpayment.html", 
        "~/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/payment.cashpaymentmethod.authorizecapturepayment.html")]
    public class CashPaymentGatewayMethod : PaymentGatewayMethodBase, ICashPaymentGatewayMethod
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CashPaymentGatewayMethod"/> class.
        /// </summary>
        /// <param name="gatewayProviderService">
        /// The gateway provider service.
        /// </param>
        /// <param name="paymentMethod">
        /// The payment method.
        /// </param>
        public CashPaymentGatewayMethod(IGatewayProviderService gatewayProviderService, IPaymentMethod paymentMethod)
            : base(gatewayProviderService, paymentMethod)
        {            
        }

        /// <summary>
        /// Does the actual work of creating and processing the payment
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/></param>
        /// <param name="args">Any arguments required to process the payment. (Maybe a username, password or some API Key)</param>
        /// <returns>The <see cref="IPaymentResult"/></returns>
        protected override IPaymentResult PerformAuthorizePayment(IInvoice invoice, ProcessorArgumentCollection args)
        {
            var authorizeAmount = invoice.Total;
            if (args.ContainsKey("authorizePaymentAmount"))
            {
                authorizeAmount = Convert.ToDecimal(args["authorizePaymentAmount"]);
            }

            var payment = GatewayProviderService.CreatePayment(PaymentMethodType.Cash, authorizeAmount, PaymentMethod.Key);
            payment.CustomerKey = invoice.CustomerKey;
            payment.PaymentMethodName = PaymentMethod.Name;
            payment.ReferenceNumber = PaymentMethod.PaymentCode + "-" + invoice.PrefixedInvoiceNumber();
            payment.Collected = false;
            payment.Authorized = true;

            GatewayProviderService.Save(payment);

            // In this case, we want to do our own Apply Payment operation as the amount has not been collected -
            // so we create an applied payment with a 0 amount.  Once the payment has been "collected", another Applied Payment record will
            // be created showing the full amount and the invoice status will be set to Paid.
            GatewayProviderService.ApplyPaymentToInvoice(payment.Key, invoice.Key, AppliedPaymentType.Debit, string.Format("To show promise of a {0} payment", PaymentMethod.Name), 0);

            //// If this were using a service we might want to store some of the transaction data in the ExtendedData for record
            ////payment.ExtendData
        
            return new PaymentResult(Attempt.Succeed(payment), invoice, false);
        }

        /// <summary>
        /// The perform authorize capture payment.
        /// </summary>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
        /// <param name="amount">
        /// The amount.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <returns>
        /// The <see cref="IPaymentResult"/>.
        /// </returns>
        protected override IPaymentResult PerformAuthorizeCapturePayment(IInvoice invoice, decimal amount, ProcessorArgumentCollection args)
        {
            var payment = GatewayProviderService.CreatePayment(PaymentMethodType.Cash, amount, PaymentMethod.Key);
            payment.CustomerKey = invoice.CustomerKey;
            payment.PaymentMethodName = PaymentMethod.Name;
            payment.ReferenceNumber = PaymentMethod.PaymentCode + "-" + invoice.PrefixedInvoiceNumber();
            payment.Collected = true;
            payment.Authorized = true;

            GatewayProviderService.Save(payment);

            GatewayProviderService.ApplyPaymentToInvoice(payment.Key, invoice.Key, AppliedPaymentType.Debit, "Cash payment", amount);

            return new PaymentResult(Attempt<IPayment>.Succeed(payment), invoice, CalculateTotalOwed(invoice).CompareTo(amount) <= 0);
        }

        /// <summary>
        /// Does the actual work of capturing a payment
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/></param>
        /// <param name="payment">the <see cref="IPayment"/></param>
        /// <param name="amount">The amount</param>
        /// <param name="args">Any arguments required to process the payment. (Maybe a username, password or some API Key)</param>
        /// <returns>The <see cref="IPaymentResult"/></returns>
        protected override IPaymentResult PerformCapturePayment(IInvoice invoice, IPayment payment, decimal amount, ProcessorArgumentCollection args)
        {
            // We need to determine if the entire amount authorized has been collected before marking
            // the payment collected.
            var appliedPayments = GatewayProviderService.GetAppliedPaymentsByPaymentKey(payment.Key);
            var applied = appliedPayments.Sum(x => x.Amount);

            var newTotalPaymentAmount = amount + applied;

            // There could be an adjustment, and the capture amount could be more than the payment amount
            if (newTotalPaymentAmount > payment.Amount)
            {
                // We are capturing more money so update payment total
                payment.Amount = newTotalPaymentAmount;
            }

            payment.Collected = newTotalPaymentAmount == payment.Amount;
            payment.Authorized = true;

            GatewayProviderService.Save(payment);

            GatewayProviderService.ApplyPaymentToInvoice(payment.Key, invoice.Key, AppliedPaymentType.Debit, "Cash payment", amount);

            return new PaymentResult(Attempt<IPayment>.Succeed(payment), invoice, CalculateTotalOwed(invoice).CompareTo(amount) <= 0);
        }

        /// <summary>
        /// Does the actual work of refunding the payment
        /// </summary>
        /// <param name="invoice">The invoice to be the payment was applied</param>
        /// <param name="payment">The payment to be refunded</param>
        /// <param name="amount">The amount of the payment to be refunded</param>
        /// <param name="args">Additional arguments required by the payment processor</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        protected override IPaymentResult PerformRefundPayment(IInvoice invoice, IPayment payment, decimal amount, ProcessorArgumentCollection args)
        {
            var appliedPayments = payment.AppliedPayments();
            foreach (var applied in appliedPayments)
            {
                applied.TransactionType = AppliedPaymentType.Refund;
                applied.Amount = 0;
                applied.Description += " - Refunded";
                GatewayProviderService.Save(applied);
            }

            payment.Amount = payment.Amount - amount;

            if (payment.Amount != 0)
            {
                GatewayProviderService.ApplyPaymentToInvoice(payment.Key, invoice.Key, AppliedPaymentType.Debit, "To show partial payment remaining after refund", payment.Amount);
            }

            GatewayProviderService.Save(payment);

            return new PaymentResult(Attempt<IPayment>.Succeed(payment), invoice, false);

        }
    }
}