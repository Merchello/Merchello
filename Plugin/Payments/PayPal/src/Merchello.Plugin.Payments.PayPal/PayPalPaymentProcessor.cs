using System;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.WebPages;
using Merchello.Core;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Models;
using Merchello.Plugin.Payments.PayPal.Models;
using System.Collections.Generic;
using PayPal.Exception;
using PayPal.PayPalAPIInterfaceService.Model;
using PayPal.PayPalAPIInterfaceService;
using Umbraco.Core;
using AddressType = PayPal.PayPalAPIInterfaceService.Model.AddressType;

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
		/// Get the mode string: "live" or "sandbox".
		/// </summary>
		/// <param name="liveMode"></param>
		/// <returns></returns>
		private static string GetModeString(bool liveMode)
		{
			return (liveMode ? "live" : "sandbox");
		}

		/// <summary>
		/// Create a dictionary with credentials for PayPal service.
		/// </summary>
		/// <param name="settings"></param>
		/// <returns></returns>
		private static Dictionary<string, string> CreatePayPalApiConfig(PayPalProcessorSettings settings)
		{
			return new Dictionary<string, string>
					{
						{"mode", GetModeString(settings.LiveMode)},
						{"account1.apiUsername", settings.ApiUsername},
						{"account1.apiPassword", settings.ApiPassword},
						{"account1.apiSignature", settings.ApiSignature}
					};
		}

		private static CurrencyCodeType PayPalCurrency(string currencyCode)
		{
			return (CurrencyCodeType)Enum.Parse(typeof(CurrencyCodeType), currencyCode, true);
		}
		
		private static int CurrencyDecimals(CurrencyCodeType currency)
		{
			switch (currency)
			{
				case CurrencyCodeType.HUF:
				case CurrencyCodeType.JPY:
				case CurrencyCodeType.TWD:
					return 0;
				default:
					return 2;
			}
		}


		private static string PriceToString(decimal price, int decimals)
		{
			var priceFormat = (decimals == 0 ? "0" : "0." + new string('0', decimals));
			return price.ToString(priceFormat, System.Globalization.CultureInfo.InvariantCulture);
		}


		/// <summary>
		/// Processes the Authorize and AuthorizeAndCapture transactions
		/// </summary>
		/// <param name="invoice">The <see cref="IInvoice"/> to be paid</param>
		/// <param name="payment">The <see cref="IPayment"/> record</param>
		/// <param name="args"></param>
		/// <returns>The <see cref="IPaymentResult"/></returns>
		public IPaymentResult ProcessPayment(IInvoice invoice, IPayment payment, ProcessorArgumentCollection args) {

			var setExpressCheckoutRequestDetails = new SetExpressCheckoutRequestDetailsType();

			Func<string, string> adjustUrl = (url) => {
				url = url.Replace("{invoiceKey}", invoice.Key.ToString(), StringComparison.InvariantCultureIgnoreCase);
				url = url.Replace("{paymentKey}", payment.Key.ToString(), StringComparison.InvariantCultureIgnoreCase);
				url = url.Replace("{paymentMethodKey}", payment.PaymentMethodKey.ToString(), StringComparison.InvariantCultureIgnoreCase);
				return url;
			};

			setExpressCheckoutRequestDetails.ReturnURL = args.GetReturnUrl();
			if (setExpressCheckoutRequestDetails.ReturnURL.IsEmpty()) setExpressCheckoutRequestDetails.ReturnURL = _settings.ReturnUrl;
			if (setExpressCheckoutRequestDetails.ReturnURL.IsEmpty()) setExpressCheckoutRequestDetails.ReturnURL = GetWebsiteUrl() + ("/App_Plugins/Merchello.PayPal/PayPalExpressCheckout.html?Result=success&InvoiceKey={invoiceKey}&PaymentKey={paymentKey}&PaymentMethodKey={paymentMethodKey}");
			setExpressCheckoutRequestDetails.ReturnURL = adjustUrl(setExpressCheckoutRequestDetails.ReturnURL);

			setExpressCheckoutRequestDetails.CancelURL = args.GetCancelUrl();
			if (setExpressCheckoutRequestDetails.CancelURL.IsEmpty()) setExpressCheckoutRequestDetails.CancelURL = _settings.CancelUrl;
			if (setExpressCheckoutRequestDetails.CancelURL.IsEmpty()) setExpressCheckoutRequestDetails.CancelURL = GetWebsiteUrl() + ("/App_Plugins/Merchello.PayPal/PayPalExpressCheckout.html?Result=fail&InvoiceKey={invoiceKey}&PaymentKey={paymentKey}&PaymentMethodKey={paymentMethodKey}");
			setExpressCheckoutRequestDetails.CancelURL = adjustUrl(setExpressCheckoutRequestDetails.CancelURL);

			setExpressCheckoutRequestDetails.PaymentDetails = new List<PaymentDetailsType> { GetPaymentDetails(invoice, args) };


			var setExpressCheckout = new SetExpressCheckoutReq();
			var setExpressCheckoutRequest = new SetExpressCheckoutRequestType(setExpressCheckoutRequestDetails);
			setExpressCheckout.SetExpressCheckoutRequest = setExpressCheckoutRequest;

			var config = CreatePayPalApiConfig(_settings);
			var service = new PayPalAPIInterfaceServiceService(config);
			var setExpressCheckoutResponseType = service.SetExpressCheckout(setExpressCheckout);

			payment.ExtendedData.SetValue("token", setExpressCheckoutResponseType.Token);

			switch (setExpressCheckoutResponseType.Ack.Value)
			{
				case AckCodeType.FAILURE:
				case AckCodeType.FAILUREWITHWARNING:
					var error = new InvalidOperationException("- " + string.Join("\n- ", setExpressCheckoutResponseType.Errors.Select(item => item.LongMessage)));
					return new PaymentResult(Attempt<IPayment>.Fail(payment, error), invoice, false);
			}

			// If this were using a service we might want to store some of the transaction data in the ExtendedData for record
			var redirectUrl = string.Format("https://www.{0}paypal.com/cgi-bin/webscr?cmd=_express-checkout&token={1}", (_settings.LiveMode ? "" : "sandbox."), setExpressCheckoutResponseType.Token);
			payment.ExtendedData.SetValue("RedirectUrl", redirectUrl);

			return new PaymentResult(Attempt<IPayment>.Succeed(payment), invoice, false);
		}

		private PaymentDetailsType GetPaymentDetails(IInvoice invoice, ProcessorArgumentCollection args = null)
		{
			
			string articleBySkuPath = args.GetArticleBySkuPath(_settings.ArticleBySkuPath.IsEmpty() ? null : GetWebsiteUrl() + _settings.ArticleBySkuPath);
			var currencyCodeType = PayPalCurrency(invoice.CurrencyCode());
			var currencyDecimals = CurrencyDecimals(currencyCodeType);

			decimal itemTotal = 0;
			decimal taxTotal = 0;
			decimal shippingTotal = 0;
			AddressType shipAddress = null;

			var paymentDetailItems = new List<PaymentDetailsItemType>();
			foreach (var item in invoice.Items)
			{
				if (item.LineItemTfKey == Merchello.Core.Constants.TypeFieldKeys.LineItem.TaxKey) {
					taxTotal = item.TotalPrice;
				} else if (item.LineItemTfKey == Merchello.Core.Constants.TypeFieldKeys.LineItem.ShippingKey) {
					shippingTotal = item.TotalPrice;
					var address = item.ExtendedData.GetAddress(Merchello.Core.AddressType.Shipping);
					if (address != null) {
						shipAddress = new AddressType() {
							Name = address.Name,
							Street1 = address.Address1,
							Street2 = address.Address2,
							PostalCode = address.PostalCode,
							CityName = address.Locality,
							StateOrProvince = address.Region,
							CountryName = address.Country().Name,
							Country = (CountryCodeType)Enum.Parse(typeof(CountryCodeType), address.Country().CountryCode, true),
							Phone = address.Phone
						};
					}
				} else if (item.LineItemTfKey == Merchello.Core.Constants.TypeFieldKeys.LineItem.ProductKey) {
					var paymentItem = new PaymentDetailsItemType {
						Name = item.Name,
						ItemURL = (articleBySkuPath.IsEmpty() ? null : articleBySkuPath + item.Sku),
						Amount = new BasicAmountType(currencyCodeType, PriceToString(item.Price, currencyDecimals)),
						Quantity = item.Quantity,
					};
					paymentDetailItems.Add(paymentItem);
					itemTotal += item.TotalPrice;
				}
			}

			var paymentDetails = new PaymentDetailsType
			{
				PaymentDetailsItem = paymentDetailItems,
				ItemTotal = new BasicAmountType(currencyCodeType, PriceToString(itemTotal, currencyDecimals)),
				TaxTotal = new BasicAmountType(currencyCodeType, PriceToString(taxTotal, currencyDecimals)),
				ShippingTotal = new BasicAmountType(currencyCodeType, PriceToString(shippingTotal, currencyDecimals)),
				OrderTotal = new BasicAmountType(currencyCodeType, PriceToString(itemTotal + taxTotal + shippingTotal, currencyDecimals)),
				PaymentAction = PaymentActionCodeType.ORDER,
				InvoiceID = invoice.InvoiceNumberPrefix + invoice.InvoiceNumber.ToString("0"),
				SellerDetails = new SellerDetailsType { PayPalAccountID = _settings.AccountId },
				PaymentRequestID = "PaymentRequest",
				ShipToAddress = shipAddress,
				NotifyURL = "http://IPNhost"
			};

			return paymentDetails;
		}

		public IPaymentResult CompletePayment(IInvoice invoice, IPayment payment, string token, string payerId)
		{
			var config = CreatePayPalApiConfig(_settings);
			var service = new PayPalAPIInterfaceServiceService(config);

			var getExpressCheckoutDetails = new GetExpressCheckoutDetailsReq
				{
					GetExpressCheckoutDetailsRequest = new GetExpressCheckoutDetailsRequestType(token)
				};
			
			var expressCheckoutDetailsResponse = service.GetExpressCheckoutDetails(getExpressCheckoutDetails);
			if (expressCheckoutDetailsResponse == null || expressCheckoutDetailsResponse.Ack != AckCodeType.SUCCESS)
			{
				var error = new InvalidOperationException("- " + string.Join("\n- ", expressCheckoutDetailsResponse.Errors.Select(item => item.LongMessage)));
				return new PaymentResult(Attempt<IPayment>.Fail(payment, error), invoice, false);
			}
			
			// do express checkout
			var doExpressCheckoutPayment = new DoExpressCheckoutPaymentReq();
			var doExpressCheckoutPaymentRequestDetails = new DoExpressCheckoutPaymentRequestDetailsType
				{
					Token = token,
					PayerID = payerId,
					PaymentDetails = new List<PaymentDetailsType> {GetPaymentDetails(invoice)}
				};

			var doExpressCheckoutPaymentRequest = new DoExpressCheckoutPaymentRequestType(doExpressCheckoutPaymentRequestDetails);
			doExpressCheckoutPayment.DoExpressCheckoutPaymentRequest = doExpressCheckoutPaymentRequest;

			var doExpressCheckoutPaymentResponse = service.DoExpressCheckoutPayment(doExpressCheckoutPayment);

			if (doExpressCheckoutPaymentResponse == null || doExpressCheckoutPaymentResponse.Ack != AckCodeType.SUCCESS)
			{
				var error = new InvalidOperationException("- " + string.Join("\n- ", doExpressCheckoutPaymentResponse.Errors.Select(item => item.LongMessage)));
				return new PaymentResult(Attempt<IPayment>.Fail(payment, error), invoice, false);
			}
			
			payment.Authorized = true;
			payment.Collected = true;
			return new PaymentResult(Attempt<IPayment>.Succeed(payment), invoice, true);
			
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
