using Lucene.Net.Index;
using Merchello.Web.Models.SaleHistory;

namespace Merchello.Web.Editors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using Core;
    using Core.Gateways.Payment;
    using Core.Models;
    using Core.Services;
    using Models;
    using Models.ContentEditing;
    using Models.Payments;
    using WebApi;
    using Workflow;
    using Umbraco.Web;

    /// <summary>
    /// The payment api controller.
    /// </summary>
    public class PaymentApiController : MerchelloApiController
    {
        /// <summary>
        /// The payment service.
        /// </summary>
        private readonly IPaymentService _paymentService;

        /// <summary>
        /// The invoice service.
        /// </summary>
        private readonly IInvoiceService _invoiceService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentApiController"/> class. 
        /// Constructor
        /// </summary>
        public PaymentApiController()
            : this(Core.MerchelloContext.Current)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentApiController"/> class. 
        /// Constructor
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello Context.
        /// </param>
        public PaymentApiController(IMerchelloContext merchelloContext)
            : base(merchelloContext)
        {
            _paymentService = merchelloContext.Services.PaymentService;
            _invoiceService = merchelloContext.Services.InvoiceService;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentApiController"/> class. 
        /// This is a helper contructor for unit testing
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello Context.
        /// </param>
        /// <param name="umbracoContext">
        /// The umbraco Context.
        /// </param>
        public PaymentApiController(IMerchelloContext merchelloContext, UmbracoContext umbracoContext)
            : base(merchelloContext, umbracoContext)
        {
            _paymentService = merchelloContext.Services.PaymentService;
            _invoiceService = merchelloContext.Services.InvoiceService;
        }


        /// <summary>
        /// Returns an Invoice by id (key)
        /// 
        /// GET /umbraco/Merchello/PaymentApi/GetPayment/{guid}
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="PaymentDisplay"/>.
        /// </returns>
        [HttpGet]
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
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The collection of payments.
        /// </returns>
        [HttpGet]
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
		/// Returns a collection of applied payments given an Invoice id (key)
		/// 
		/// GET /umbraco/Merchello/PaymentApi/GetAppliedPaymentsByInvoice/{guid}
		/// </summary>
		/// <param name="id">
		/// The id.
		/// </param>
		/// <returns>
		/// The collection of applied payments.
		/// </returns>
		[HttpGet]
        public IEnumerable<AppliedPaymentDisplay> GetAppliedPaymentsByInvoice(Guid id)
		{
			var appliedPayments = _paymentService.GetAppliedPaymentsByInvoiceKey(id);

			if (appliedPayments == null)
			{
				throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
			}

			var stuff = appliedPayments.Select(x => x.ToAppliedPaymentDisplay());
			return stuff;
		}

        /// <summary>
        /// Returns a payment for an AuthorizePayment PaymentRequest
        /// 
        /// GET /umbraco/Merchello/PaymentApi/AuthorizePayment/
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="PaymentDisplay"/>.
        /// </returns>
        [HttpPost]
        public PaymentResultDisplay AuthorizePayment(PaymentRequest request)
        {
            var processor = new PaymentProcessor(MerchelloContext, request);
            var authorize = processor.Authorize();

            var result = new PaymentResultDisplay()
            {
                Success = authorize.Payment.Success,
                Invoice = authorize.Invoice.ToInvoiceDisplay(),
                Payment = authorize.Payment.Result.ToPaymentDisplay(),
                ApproveOrderCreation = authorize.ApproveOrderCreation
            };
            
            if (!authorize.Payment.Success)
            {
                authorize.Payment.Result.AuditPaymentDeclined();                
            }
            else
            {
                authorize.Payment.Result.AuditPaymentAuthorize(authorize.Invoice);    
            }
                       
            return result;
        }

        /// <summary>
        /// Returns a payment for an CapturePayment PaymentRequest
        /// 
        /// GET /umbraco/Merchello/PaymentApi/CapturePayment/
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="PaymentDisplay"/>.
        /// </returns>
        [HttpPost]
        public PaymentResultDisplay CapturePayment(PaymentRequest request)
        {
            var processor = new PaymentProcessor(MerchelloContext, request);

            var capture = processor.Capture();

            var result = new PaymentResultDisplay()
            {
                Success = capture.Payment.Success,
                Invoice = capture.Invoice.ToInvoiceDisplay(),
                Payment = capture.Payment.Result.ToPaymentDisplay(),
                ApproveOrderCreation = capture.ApproveOrderCreation
            };

            if (!capture.Payment.Success)
            {
                capture.Payment.Result.AuditPaymentDeclined();
            }
            else
            {
                capture.Payment.Result.AuditPaymentCaptured(request.Amount);
            }
           
            return result;
        }


        /// <summary>
        /// Returns a payment for an AuthorizeCapturePayment PaymentRequest
        /// 
        /// GET /umbraco/Merchello/PaymentApi/AuthorizeCapturePayment/
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="PaymentDisplay"/>.
        /// </returns>
        [HttpPost]
        public PaymentResultDisplay AuthorizeCapturePayment(PaymentRequest request)
        {
            var processor = new PaymentProcessor(MerchelloContext, request);

            var authorizeCapture = processor.AuthorizeCapture();

            var result = new PaymentResultDisplay()
            {
                Success = authorizeCapture.Payment.Success,
                Invoice = authorizeCapture.Invoice.ToInvoiceDisplay(),
                Payment = authorizeCapture.Payment.Result.ToPaymentDisplay(),
                ApproveOrderCreation = authorizeCapture.ApproveOrderCreation
            };

            if (!authorizeCapture.Payment.Success)
            {
                authorizeCapture.Payment.Result.AuditPaymentDeclined();
            }
            else
            {
                authorizeCapture.Payment.Result.AuditPaymentAuthorize(authorizeCapture.Invoice);
                authorizeCapture.Payment.Result.AuditPaymentCaptured(request.Amount);
            }
            
            return result;
        }

        /// <summary>
        /// Returns a payment for an CapturePayment for a PaymentRequest
        /// 
        /// GET /umbraco/Merchello/PaymentApi/RefundPayment/
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="PaymentDisplay"/>.
        /// </returns>
        public PaymentResultDisplay RefundPayment(PaymentRequest request)
        {
            var processor = new PaymentProcessor(MerchelloContext, request);

            var refund = processor.Refund();

            var result = new PaymentResultDisplay()
            {
                Success = refund.Payment.Success,
                Invoice = refund.Invoice.ToInvoiceDisplay(),
                Payment = refund.Payment.Result.ToPaymentDisplay(),
                ApproveOrderCreation = refund.ApproveOrderCreation
            };

            if (refund.Payment.Success)            
            {
               refund.Payment.Result.AuditPaymentRefunded(request.Amount);
            }
            

            return result;
        }       
    }
}