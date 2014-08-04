using System;
using System.Web;
using Merchello.Core;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Models;
using Merchello.Plugin.Payments.PayPal.Models;
using System.Collections.Generic;
using PayPal.PayPalAPIInterfaceService.Model;
using PayPal.PayPalAPIInterfaceService;
using Umbraco.Core;

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
		private static string GetWebsiteUrl()
		{
			var url = HttpContext.Current.Request.Url;
			var baseUrl = String.Format("{0}://{1}{2}", url.Scheme, url.Host, url.IsDefaultPort ? "" : ":" + url.Port);
			return baseUrl;
		}

		/// <summary>
		/// Processes the Authorize and AuthorizeAndCapture transactions
		/// </summary>
		/// <param name="invoice">The <see cref="IInvoice"/> to be paid</param>
		/// <param name="payment">The <see cref="IPayment"/> record</param>
		/// <param name="args"></param>
		/// <returns>The <see cref="IPaymentResult"/></returns>
		public IPaymentResult ProcessPayment(IInvoice invoice, IPayment payment, ProcessorArgumentCollection args)
		{
			var setExpressCheckoutRequestDetails = new SetExpressCheckoutRequestDetailsType
			{
				ReturnURL = String.Format("{0}/App_Plugins/Merchello.PayPal/PayPalExpressCheckout.html?InvoiceKey={1}&PaymentKey={2}&PaymentMethodKey={3}", GetWebsiteUrl(), invoice.Key, payment.Key, payment.PaymentMethodKey),
				CancelURL = "http://localhost/cancel",
				PaymentDetails = new List<PaymentDetailsType> { GetPaymentDetails(invoice) }
			};


			var setExpressCheckout = new SetExpressCheckoutReq();
			var setExpressCheckoutRequest = new SetExpressCheckoutRequestType(setExpressCheckoutRequestDetails);
			setExpressCheckout.SetExpressCheckoutRequest = setExpressCheckoutRequest;
			var config = new Dictionary<string, string>
					{
						{"mode", "sandbox"},
						{"account1.apiUsername", _settings.ApiUsername},
						{"account1.apiPassword", _settings.ApiPassword},
						{"account1.apiSignature", _settings.ApiSignature}
					};
			var service = new PayPalAPIInterfaceServiceService(config);
			var responseSetExpressCheckoutResponseType = service.SetExpressCheckout(setExpressCheckout);

			// If this were using a service we might want to store some of the transaction data in the ExtendedData for record
			payment.ExtendedData.SetValue("RedirectUrl", "https://www.sandbox.paypal.com/cgi-bin/webscr?cmd=_express-checkout&token=" + responseSetExpressCheckoutResponseType.Token);

			return new PaymentResult(Attempt<IPayment>.Succeed(payment), invoice, false);
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
							ItemURL = GetWebsiteUrl() + "/all-products/" + item.Sku,
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

		public IPaymentResult CompletePayment(IInvoice invoice, IPayment payment, string token, string payerId)
		{
			var config = new Dictionary<string, string>
					{
						{"mode", "sandbox"},
						{"account1.apiUsername", _settings.ApiUsername},
						{"account1.apiPassword", _settings.ApiPassword},
						{"account1.apiSignature", _settings.ApiSignature}
					};
			var service = new PayPalAPIInterfaceServiceService(config);

			var getExpressCheckoutDetails = new GetExpressCheckoutDetailsReq
				{
					GetExpressCheckoutDetailsRequest = new GetExpressCheckoutDetailsRequestType(token)
				};
			var expressCheckoutDetailsResponse = service.GetExpressCheckoutDetails(getExpressCheckoutDetails);

			if (expressCheckoutDetailsResponse != null)
			{
				if (expressCheckoutDetailsResponse.Ack == AckCodeType.SUCCESS)
				{
					// do express checkout
					var doExpressCheckoutPayment = new DoExpressCheckoutPaymentReq();
					var doExpressCheckoutPaymentRequestDetails = new DoExpressCheckoutPaymentRequestDetailsType
						{
							Token = token,
							PayerID = payerId,
							PaymentDetails = new List<PaymentDetailsType> {GetPaymentDetails(invoice)}
						};

					var doExpressCheckoutPaymentRequest =
						new DoExpressCheckoutPaymentRequestType(doExpressCheckoutPaymentRequestDetails);
					doExpressCheckoutPayment.DoExpressCheckoutPaymentRequest = doExpressCheckoutPaymentRequest;

					var doExpressCheckoutPaymentResponse = service.DoExpressCheckoutPayment(doExpressCheckoutPayment);

					if (doExpressCheckoutPaymentResponse != null)
					{
						if (doExpressCheckoutPaymentResponse.Ack == AckCodeType.SUCCESS)
						{
							payment.Authorized = true;
							payment.Collected = true;
							return new PaymentResult(Attempt<IPayment>.Succeed(payment), invoice, true);
						}
					}
				}
			}

			return new PaymentResult(Attempt<IPayment>.Succeed(payment), invoice, false);
		}

		/// <summary>
		/// The PayPal API version
		/// </summary>
		public static string ApiVersion
		{
			get { return "1.0"; }
		}
	}
}
