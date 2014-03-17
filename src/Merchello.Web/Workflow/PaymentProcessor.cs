using System;
using Merchello.Core;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Models;
using Merchello.Web.Models;
using Umbraco.Core;

namespace Merchello.Web.Workflow
{
    /// <summary>
    /// Processes a payment
    /// </summary>
    internal class PaymentProcessor
    {
        private readonly IMerchelloContext _merchelloContext;
        private IInvoice _invoice;
        private IPayment _payment;
        private decimal _amount;
        private IPaymentGatewayMethod _paymentGatewayMethod;
        private readonly ProcessorArgumentCollection _args = new ProcessorArgumentCollection();

        

        public PaymentProcessor(IMerchelloContext merchelloContext, PaymentRequest request)
        {
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");
            
            _merchelloContext = merchelloContext;

            Build(request);
        }

        private void Build(PaymentRequest request)
        {
            _invoice = _merchelloContext.Services.InvoiceService.GetByKey(request.InvoiceKey);

            if (request.PaymentKey != null)
                _payment = _merchelloContext.Services.PaymentService.GetByKey(request.PaymentKey.Value);

            _paymentGatewayMethod =
                _merchelloContext.Gateways.Payment.GetPaymentGatewayMethodByKey(request.PaymentMethodKey);

            _amount = request.Amount;

            foreach (var arg in request.ProcessorArgs)
            {
                _args.Add(arg.Key, arg.Value);
            }

        }

        /// <summary>
        /// Performs an Authorize payment 
        /// </summary>
        /// <returns>The <see cref="IPaymentResult"/></returns>
        public IPaymentResult Authorize()
        {
            return !IsReady() ? GetFailedResult() : _paymentGatewayMethod.AuthorizePayment(_invoice, _args);
        }

        /// <summary>
        /// Performs a Capture payment
        /// </summary>
        /// <returns>The <see cref="IPaymentResult"/></returns>
        public IPaymentResult Capture()
        {
            return !IsReady() || _payment == null
                ? GetFailedResult()
                : _paymentGatewayMethod.CapturePayment(_invoice, _payment, _amount, _args);
        }

        /// <summary>
        /// Performs the Authorize and Capture of a payment
        /// </summary>
        /// <returns>The <see cref="IPaymentResult"/></returns>
        public IPaymentResult AuthorizeCapture()
        {
            return !IsReady()
                ? GetFailedResult()
                : _paymentGatewayMethod.AuthorizeCapturePayment(_invoice, _amount, _args);
        }

        /// <summary>
        /// Performs a refund payment
        /// </summary>
        /// <returns></returns>
        public IPaymentResult Refund()
        {
            return !IsReady() || _payment == null 
                ? GetFailedResult() 
                : _paymentGatewayMethod.RefundPayment(_invoice, _payment, _args);
        }

        /// <summary>
        /// True/false indicating whether or not the processor is ready
        /// </summary>
        private bool IsReady()
        {
            return _invoice != null && _paymentGatewayMethod != null;
        }

        private IPaymentResult GetFailedResult()
        {
            return new PaymentResult(Attempt<IPayment>.Fail(new InvalidOperationException("PaymentProcessor is not ready")), Invoice, false);
        }

        /// <summary>
        /// Gets the <see cref="IInvoice"/>
        /// </summary>
        private IInvoice Invoice { get; set; }

    }
}