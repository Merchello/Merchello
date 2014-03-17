using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Web.Models;
using Merchello.Web.Models.ContentEditing;
using Merchello.Web.WebApi;
using Merchello.Web.Workflow;
using Umbraco.Web;

namespace Merchello.Web.Editors
{
    public class PaymentApiController : MerchelloApiController
    {
        private readonly IPaymentService _paymentService;
        private readonly IInvoiceService _invoiceService;

        /// <summary>
        /// Constructor
        /// </summary>
        public PaymentApiController()
            : this(MerchelloContext.Current)
        {}

        /// <summary>
        /// Constructor
        /// </summary>
        public PaymentApiController(IMerchelloContext merchelloContext)
            : base((MerchelloContext) merchelloContext)
        {
            _paymentService = merchelloContext.Services.PaymentService;
            _invoiceService = merchelloContext.Services.InvoiceService;
        }

        /// <summary>
        /// This is a helper contructor for unit testing
        /// </summary>
        public PaymentApiController(IMerchelloContext merchelloContext, UmbracoContext umbracoContext)
            : base((MerchelloContext) merchelloContext, umbracoContext)
        {
            _paymentService = merchelloContext.Services.PaymentService;
            _invoiceService = merchelloContext.Services.InvoiceService;
        }


        /// <summary>
        /// Returns an Invoice by id (key)
        /// 
        /// GET /umbraco/Merchello/PaymentApi/GetPayment/{guid}
        /// </summary>
        public PaymentDisplay GetPayment(Guid id)
        {
            var payment = _paymentService.GetByKey(id) as Payment;
            if (payment == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return payment.ToPaymentDisplay();
        }

        /// <summary>
        /// Returns a collection of payments given an Invoice id (key)
        /// 
        /// GET /umbraco/Merchello/PaymentApi/GetPaymentsByInvoice/{guid}
        /// </summary>
        public IEnumerable<PaymentDisplay> GetPaymentsByInvoice(Guid id)
        {
            var payments = _paymentService.GetPaymentsByInvoiceKey(id);

            if (payments == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return payments.Select(x => x.ToPaymentDisplay());
        }


        /// <summary>
        /// Returns a payment for an AuthorizePayment PaymentRequest
        /// 
        /// GET /umbraco/Merchello/PaymentApi/AuthorizePayment/
        /// </summary>
        [AcceptVerbs("POST", "GET")]
        public PaymentDisplay AuthorizePayment(PaymentRequest request)
        {
            var processor = new PaymentProcessor(MerchelloContext, request);

            var authorize = processor.Authorize();

            if (!authorize.Payment.Success)
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));

            return authorize.Payment.Result.ToPaymentDisplay();

        }

        /// <summary>
        /// Returns a payment for an CapturePayment PaymentRequest
        /// 
        /// GET /umbraco/Merchello/PaymentApi/CapturePayment/
        /// </summary>
        [AcceptVerbs("POST", "GET")]
        public PaymentDisplay CapturePayment(PaymentRequest request)
        {
            var processor = new PaymentProcessor(MerchelloContext, request);

            var capture = processor.Capture();

            if(!capture.Payment.Success)
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));

            return capture.Payment.Result.ToPaymentDisplay();
        }

        /// <summary>
        /// Returns a payment for an AuthorizeCapturePayment PaymentRequest
        /// 
        /// GET /umbraco/Merchello/PaymentApi/AuthorizeCapturePayment/
        /// </summary>
        [AcceptVerbs("POST", "GET")]
        public PaymentDisplay AuthorizeCapturePayment(PaymentRequest request)
        {
            var processor = new PaymentProcessor(MerchelloContext, request);

            var authorizeCapture = processor.AuthorizeCapture();

            if(!authorizeCapture.Payment.Success)
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));

            return authorizeCapture.Payment.Result.ToPaymentDisplay();
        }

        /// <summary>
        /// Returns a payment for an CapturePayment for a PaymentRequest
        /// 
        /// GET /umbraco/Merchello/PaymentApi/RefundPayment/
        /// </summary>
        public PaymentDisplay RefundPayment(PaymentRequest request)
        {
            var processor = new PaymentProcessor(MerchelloContext, request);

            var refund = processor.Refund();

            if(!refund.Payment.Success)
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));

            return refund.Payment.Result.ToPaymentDisplay();
        }
       
    }
}