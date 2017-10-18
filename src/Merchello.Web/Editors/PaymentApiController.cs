namespace Merchello.Web.Editors
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;

    using Merchello.Core;
    using Merchello.Core.Models;
    using Merchello.Core.Services;
    using Merchello.Web.Models.ContentEditing;
    using Merchello.Web.Models.Payments;
    using Merchello.Web.Models.SaleHistory;
    using Merchello.Web.WebApi;
    using Merchello.Web.Workflow;
    using Merchello.Web.Workflow.Payment;

    using Umbraco.Web;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// The payment api controller.
    /// </summary>
    [PluginController("Merchello")]
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

            return payments.OrderByDescending(x => x.CreateDate).Select(x => x.ToPaymentDisplay());
        }

        /// <summary>
        /// Gets a <see cref="PaymentMethodDisplay"/> method.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="PaymentMethodDisplay"/>.
        /// </returns>
        [HttpGet]
        public PaymentMethodDisplay GetPaymentMethod(Guid id)
        {
            var paymentMethod = MerchelloContext.Gateways.Payment.GetPaymentGatewayMethodByKey(id);

            if (paymentMethod == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            var display = paymentMethod.ToPaymentMethodDisplay();
            return display;
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
        /// Returns a payment for an AuthorizePayment PaymentRequestDisplay
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
        public PaymentResultDisplay AuthorizePayment(PaymentRequestDisplay request)
        {
            // Add the amount to the args, this is for the Authorize from the backoffice new payments
            // for some reason Authorize does not take in the amount and only uses the full invoice amount
            // which is incorrect when adding adjustments via the back office.
            var requestArgs = request.ProcessorArgs.ToList();
            requestArgs.Add(new KeyValuePair<string, string>("authorizePaymentAmount", request.Amount.ToString(CultureInfo.InvariantCulture)));
            request.ProcessorArgs = requestArgs;

            var processor = new PaymentProcessor(MerchelloContext, request);
            var authorize = processor.Authorize();

            var result = new PaymentResultDisplay
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
                if (request.Amount > 0)
                {
                    authorize.Payment.Result.AuditPaymentAuthorize(authorize.Invoice, request.Amount);
                }
                else
                {
                    authorize.Payment.Result.AuditPaymentAuthorize(authorize.Invoice);
                }                
            }
                       
            return result;
        }

        /// <summary>
        /// Returns a payment for an CapturePayment PaymentRequestDisplay
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
        public PaymentResultDisplay CapturePayment(PaymentRequestDisplay request)
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
        /// Returns a payment for an AuthorizeCapturePayment PaymentRequestDisplay
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
        public PaymentResultDisplay AuthorizeCapturePayment(PaymentRequestDisplay request)
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
                authorizeCapture.Payment.Result.AuditPaymentAuthorize(authorizeCapture.Invoice, request.Amount);
                authorizeCapture.Payment.Result.AuditPaymentCaptured(request.Amount);
            }
            
            return result;
        }

        /// <summary>
        /// Returns a payment result for an refund operation
        /// 
        /// GET /umbraco/Merchello/PaymentApi/RefundPayment/
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="PaymentDisplay"/>.
        /// </returns>
        public PaymentResultDisplay RefundPayment(PaymentRequestDisplay request)
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

        /// <summary>
        /// Returns a payment result for a void operation
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="PaymentResultDisplay"/>.
        /// </returns>
        [HttpPost]
        public PaymentResultDisplay VoidPayment(PaymentRequestDisplay request)
        {
            var processor = new PaymentProcessor(MerchelloContext, request);

            var voided = processor.Void();

            var result = new PaymentResultDisplay()
            {
                Success = voided.Payment.Success,
                Invoice = voided.Invoice.ToInvoiceDisplay(),
                Payment = voided.Payment.Result.ToPaymentDisplay(),
                ApproveOrderCreation = voided.ApproveOrderCreation
            };

            if (voided.Payment.Success)
            {
                voided.Payment.Result.AuditPaymentVoided();
            }

            return result;
        }
    }
}