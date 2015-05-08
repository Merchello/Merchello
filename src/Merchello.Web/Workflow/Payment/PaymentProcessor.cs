namespace Merchello.Web.Workflow.Payment
{
    using System;

    using Merchello.Core;
    using Merchello.Core.Gateways.Payment;
    using Merchello.Core.Models;
    using Merchello.Web.Models.Payments;

    using Umbraco.Core;

    /// <summary>
    /// Processes a payment
    /// </summary>
    internal class PaymentProcessor
    {
        #region Fields

        /// <summary>
        /// The processor argument collection.
        /// </summary>
        private readonly ProcessorArgumentCollection _args = new ProcessorArgumentCollection();
        
        /// <summary>
        /// The <see cref="IMerchelloContext"/>.
        /// </summary>
        private readonly IMerchelloContext _merchelloContext;

        /// <summary>
        /// The <see cref="IInvoice"/>.
        /// </summary>
        private IInvoice _invoice;

        /// <summary>
        /// The <see cref="IPayment"/>.
        /// </summary>
        private IPayment _payment;

        /// <summary>
        /// The payment amount.
        /// </summary>
        private decimal _amount;

        /// <summary>
        /// The payment gateway method.
        /// </summary>
        private IPaymentGatewayMethod _paymentGatewayMethod;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentProcessor"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="request">
        /// The request.
        /// </param>
        public PaymentProcessor(IMerchelloContext merchelloContext, PaymentRequest request)
        {
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");
            
            this._merchelloContext = merchelloContext;

            this.Initialize(request);
        }


        /// <summary>
        /// Performs an Authorize payment 
        /// </summary>
        /// <returns>The <see cref="IPaymentResult"/></returns>
        public IPaymentResult Authorize()
        {
            return !this.IsReady() ? this.GetFailedResult() : this._paymentGatewayMethod.AuthorizePayment(this._invoice, this._args);
        }

        /// <summary>
        /// Performs a Capture payment
        /// </summary>
        /// <returns>The <see cref="IPaymentResult"/></returns>
        public IPaymentResult Capture()
        {
            return !this.IsReady() || this._payment == null
                ? this.GetFailedResult()
                : this._paymentGatewayMethod.CapturePayment(this._invoice, this._payment, this._amount, this._args);
        }

        /// <summary>
        /// Performs the Authorize and Capture of a payment
        /// </summary>
        /// <returns>The <see cref="IPaymentResult"/></returns>
        public IPaymentResult AuthorizeCapture()
        {
            return !this.IsReady()
                ? this.GetFailedResult()
                : this._paymentGatewayMethod.AuthorizeCapturePayment(this._invoice, this._amount, this._args);
        }

        /// <summary>
        /// Performs a refund payment
        /// </summary>
        /// <returns><see cref="IPaymentResult"/></returns>
        public IPaymentResult Refund()
        {
            return !this.IsReady() || this._payment == null 
                ? this.GetFailedResult() 
                : this._paymentGatewayMethod.RefundPayment(this._invoice, this._payment, this._amount, this._args);
        }

        /// <summary>
        /// True/false indicating whether or not the processor is ready
        /// </summary>
        /// <returns>
        /// The a value indicating whether or not the processor is ready.
        /// </returns>
        private bool IsReady()
        {
            return this._invoice != null && this._paymentGatewayMethod != null;
        }

        /// <summary>
        /// The get failed result.
        /// </summary>
        /// <returns>
        /// The <see cref="IPaymentResult"/>.
        /// </returns>
        private IPaymentResult GetFailedResult()
        {
            return new PaymentResult(Attempt<IPayment>.Fail(new InvalidOperationException("PaymentProcessor is not ready")), this._invoice, false);
        }

        /// <summary>
        /// Initializes "this" object
        /// </summary>
        /// <param name="request">
        /// The incoming <see cref="PaymentRequest"/>
        /// </param>
        private void Initialize(PaymentRequest request)
        {
            this._invoice = this._merchelloContext.Services.InvoiceService.GetByKey(request.InvoiceKey);

            if (request.PaymentKey != null)
                this._payment = this._merchelloContext.Services.PaymentService.GetByKey(request.PaymentKey.Value);

            this._paymentGatewayMethod =
                this._merchelloContext.Gateways.Payment.GetPaymentGatewayMethodByKey(request.PaymentMethodKey);

            this._amount = request.Amount;

            foreach (var arg in request.ProcessorArgs)
            {
                this._args.Add(arg.Key, arg.Value);
            }
        }
    }
}