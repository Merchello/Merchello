using System;
using System.Web;
using System.Collections.Generic;

using Merchello.Core;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Models;
using Merchello.Plugin.Payments.PayPal.Models;

using Umbraco.Core;

using PayPal.PayPalAPIInterfaceService.Model;
using PayPal.PayPalAPIInterfaceService;

namespace Merchello.Plugin.Payments.PayPal
{
	/// <summary>
	/// The PayPal payment processor
	/// </summary>
	public class PayPalPaymentProcessor
	{
		private readonly PayPalProcessorSettings _settings;

		public PayPalPaymentProcessor(PayPalProcessorSettings settings)
        {
            _settings = settings;
        }

		/// <summary>
		/// Get the absolute base URL for this website
		/// </summary>
		/// <returns></returns>
		public static string GetWebsiteUrl()
		{
			var url = HttpContext.Current.Request.Url;
			var baseUrl = String.Format("{0}://{1}{2}", url.Scheme, url.Host, url.IsDefaultPort ? "" : ":" + url.Port);
			return baseUrl;
		}

		/// <summary>
        /// ExpressCheckout (prepare order)
		/// </summary>
		/// <param name="invoice">The <see cref="IInvoice"/> to be paid</param>
		/// <param name="payment">The <see cref="IPayment"/> record</param>
		/// <returns>The <see cref="IPaymentResult"/></returns>
        public IPaymentResult ExpressCheckout(IInvoice invoice, IPayment payment)
		{
            var setExpressCheckoutRequestDetails = new SetExpressCheckoutRequestDetailsType
            {
                ReturnURL = String.Format("{0}/umbraco/MerchelloPayPal/PayPalApi/AuthorizePayment?InvoiceKey={1}&PaymentKey={2}", GetWebsiteUrl(), invoice.Key, payment.Key),
                CancelURL = GetWebsiteUrl(),
                PaymentDetails = new List<PaymentDetailsType> { GetPaymentDetails(invoice) }
            };

            try
            {
                var response = GetPayPalService().SetExpressCheckout(new SetExpressCheckoutReq { SetExpressCheckoutRequest = new SetExpressCheckoutRequestType(setExpressCheckoutRequestDetails) });
                if (response.Ack == AckCodeType.SUCCESS)
                {
                    payment.ExtendedData.SetValue(Constants.ExtendedDataKeys.OrderConfirmUrl, "https://www.sandbox.paypal.com/cgi-bin/webscr?cmd=_express-checkout&token=" + response.Token);
                    return new PaymentResult(Attempt<IPayment>.Succeed(payment), invoice, true);
                }

                var errorMessage = response.Errors.Count > 0 ? response.Errors[0].LongMessage : "An unknown error";
                return new PaymentResult(Attempt<IPayment>.Fail(payment, new Exception(errorMessage)), invoice, false);
            }
            catch (Exception ex)
            {
                return new PaymentResult(Attempt<IPayment>.Fail(payment, ex), invoice, false);
            }
		}

        public IPaymentResult AuthorizePayment(IInvoice invoice, IPayment payment, string token, string payerId)
        {
            var service = GetPayPalService();

            try
            {
                // get checkout details 
                var expressCheckoutResponse = service.GetExpressCheckoutDetails(new GetExpressCheckoutDetailsReq { GetExpressCheckoutDetailsRequest = new GetExpressCheckoutDetailsRequestType(token) });
                if (expressCheckoutResponse.Ack != AckCodeType.SUCCESS)
                {
                    var errorMessage = expressCheckoutResponse.Errors.Count > 0 ? expressCheckoutResponse.Errors[0].LongMessage : "An unknown error";
                    return new PaymentResult(Attempt<IPayment>.Fail(payment, new Exception(errorMessage)), invoice, false);
                }

                // do express checkout
                var doExpressCheckoutPaymentResponse = service.DoExpressCheckoutPayment(new DoExpressCheckoutPaymentReq
                    {
                        DoExpressCheckoutPaymentRequest = new DoExpressCheckoutPaymentRequestType(
                            new DoExpressCheckoutPaymentRequestDetailsType
                        {
                            Token = token,
                            PayerID = payerId,
                            PaymentDetails = new List<PaymentDetailsType> { GetPaymentDetails(invoice) }
                        })
                    });
                if (doExpressCheckoutPaymentResponse.Ack != AckCodeType.SUCCESS)
                {
                    var errorMessage = doExpressCheckoutPaymentResponse.Errors.Count > 0 ? doExpressCheckoutPaymentResponse.Errors[0].LongMessage : "An unknown error";
                    return new PaymentResult(Attempt<IPayment>.Fail(payment, new Exception(errorMessage)), invoice, false);
                }

                var transactionId = doExpressCheckoutPaymentResponse.DoExpressCheckoutPaymentResponseDetails.PaymentInfo[0].TransactionID;
                var currency = doExpressCheckoutPaymentResponse.DoExpressCheckoutPaymentResponseDetails.PaymentInfo[0].GrossAmount.currencyID;
                var amount = doExpressCheckoutPaymentResponse.DoExpressCheckoutPaymentResponseDetails.PaymentInfo[0].GrossAmount.value;

                // do authorization
                var doAuthorizationResponse = service.DoAuthorization(new DoAuthorizationReq
                    {
                        DoAuthorizationRequest = new DoAuthorizationRequestType
                        {
                            TransactionID = transactionId,
                            Amount = new BasicAmountType(currency, amount)
                        }
                    });
                if (doAuthorizationResponse.Ack != AckCodeType.SUCCESS)
                {
                    var errorMessage = doAuthorizationResponse.Errors.Count > 0 ? doAuthorizationResponse.Errors[0].LongMessage : "An unknown error";
                    return new PaymentResult(Attempt<IPayment>.Fail(payment, new Exception(errorMessage)), invoice, false);
                }

                payment.ExtendedData.SetValue(Constants.ExtendedDataKeys.AuthorizationId, doAuthorizationResponse.TransactionID);
                payment.ExtendedData.SetValue(Constants.ExtendedDataKeys.AmountCurrencyId, currency.ToString());
                payment.Authorized = true;
                return new PaymentResult(Attempt<IPayment>.Succeed(payment), invoice, true);
            }
            catch (Exception ex)
            {
                return new PaymentResult(Attempt<IPayment>.Fail(payment, ex), invoice, false);
            }
        }

        public IPaymentResult CapturePayment(IInvoice invoice, IPayment payment, decimal amount, bool isPartialPayment)
        {
            var authorizationId = payment.ExtendedData.GetValue(Constants.ExtendedDataKeys.AuthorizationId);
            CurrencyCodeType amountCurrency;

            if (!payment.Authorized || string.IsNullOrEmpty(authorizationId)) return new PaymentResult(Attempt<IPayment>.Fail(payment, new InvalidOperationException("Payment is not Authorized")), invoice, false);
            if (!Enum.TryParse(payment.ExtendedData.GetValue(Constants.ExtendedDataKeys.AmountCurrencyId), out amountCurrency)) 
                return new PaymentResult(Attempt<IPayment>.Fail(payment, new InvalidOperationException("Amount Currency not present")), invoice, false);

            try
            {
                var doCaptureResponse = GetPayPalService().DoCapture(new DoCaptureReq
                {
                    DoCaptureRequest =
                        new DoCaptureRequestType
                        {
                            AuthorizationID = authorizationId,
                            Amount = new BasicAmountType(amountCurrency, amount.ToString("0.00")),
                            CompleteType = isPartialPayment ? CompleteCodeType.NOTCOMPLETE : CompleteCodeType.COMPLETE
                            //InvoiceID = invoice.Key.ToString()
                        }
                });
                if (doCaptureResponse.Ack != AckCodeType.SUCCESS)
                {
                    var errorMessage = doCaptureResponse.Errors.Count > 0 ? doCaptureResponse.Errors[0].LongMessage : "An unknown error";
                    return new PaymentResult(Attempt<IPayment>.Fail(payment, new Exception(errorMessage)), invoice, false);
                }

                payment.ExtendedData.SetValue(Constants.ExtendedDataKeys.TransactionId, doCaptureResponse.DoCaptureResponseDetails.PaymentInfo.TransactionID);
                payment.Collected = true;
                payment.Authorized = true;
                return new PaymentResult(Attempt<IPayment>.Succeed(payment), invoice, false);
            }
            catch (Exception ex)
            {
                return new PaymentResult(Attempt<IPayment>.Fail(payment, ex), invoice, false);
            }
		}

        public IPaymentResult RefundPayment(IInvoice invoice, IPayment payment)
        {
            var transactionId = payment.ExtendedData.GetValue(Constants.ExtendedDataKeys.TransactionId);

            var wrapper = new RefundTransactionReq
            {
                RefundTransactionRequest =
                    {
                        TransactionID = transactionId,
                        RefundType = RefundType.FULL
                    }
            };
            RefundTransactionResponseType refundTransactionResponse = GetPayPalService().RefundTransaction(wrapper);

            if (refundTransactionResponse.Ack == AckCodeType.SUCCESS)
            {
                return new PaymentResult(Attempt<IPayment>.Succeed(payment), invoice, true);
            }

            return new PaymentResult(Attempt<IPayment>.Fail(payment), invoice, false);
        }

		/// <summary>
		/// The PayPal API version
		/// </summary>
		public static string ApiVersion
		{
			get { return "1.0"; }
		}

        private PayPalAPIInterfaceServiceService GetPayPalService()
        {
            return new PayPalAPIInterfaceServiceService(new Dictionary<string, string>
					{
						{"mode", "sandbox"},
						{"account1.apiUsername", _settings.ApiUsername},
						{"account1.apiPassword", _settings.ApiPassword},
						{"account1.apiSignature", _settings.ApiSignature}
					});
        }

        private PaymentDetailsType GetPaymentDetails(IInvoice invoice)
        {
            decimal itemTotal = 0;
            decimal taxTotal = 0;
            decimal shippingTotal = 0;
            var paymentDetailItems = new List<PaymentDetailsItemType>();
            foreach (var item in invoice.Items)
            {
                switch (item.LineItemType)
                {
                    case LineItemType.Tax:
                        taxTotal = item.TotalPrice;
                        break;
                    case LineItemType.Shipping:
                        shippingTotal = item.TotalPrice;
                        break;
                    case LineItemType.Product:
                        var paymentItem = new PaymentDetailsItemType
                        {
                            Name = item.Name,
                            Amount = new BasicAmountType(CurrencyCodeType.USD, item.Price.ToString("0.00")),
                            Quantity = item.Quantity,
                        };
                        paymentDetailItems.Add(paymentItem);
                        itemTotal += item.TotalPrice;
                        break;
                    default:
                        throw new Exception("Unsupported item with type: " + item.LineItemType);
                }
            }

            var paymentDetails = new PaymentDetailsType
            {
                PaymentDetailsItem = paymentDetailItems,
                ItemTotal = new BasicAmountType(CurrencyCodeType.USD, itemTotal.ToString("0.00")),
                TaxTotal = new BasicAmountType(CurrencyCodeType.USD, taxTotal.ToString("0.00")),
                ShippingTotal = new BasicAmountType(CurrencyCodeType.USD, shippingTotal.ToString("0.00")),
                OrderTotal = new BasicAmountType(CurrencyCodeType.USD, (itemTotal + taxTotal + shippingTotal).ToString("0.00")),
                PaymentAction = PaymentActionCodeType.ORDER,
                SellerDetails = new SellerDetailsType { PayPalAccountID = _settings.AccountId },
                PaymentRequestID = "PaymentRequest",
                //ShipToAddress = shipToAddress,
                NotifyURL = "http://IPNhost"
            };

            return paymentDetails;
        }
	}
}
