namespace Merchello.Providers.Payment.PayPal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.WebPages;

    using Merchello.Core.Gateways.Payment;
    using Merchello.Core.Models;
    using Merchello.Providers.Payment.Models;

    using global::PayPal.PayPalAPIInterfaceService;
    using global::PayPal.PayPalAPIInterfaceService.Model;

    using Umbraco.Core;

    using AddressType = global::PayPal.PayPalAPIInterfaceService.Model.AddressType;

    using Constants = Payment.Constants;

    /// <summary>
	/// The PayPal payment processor
	/// </summary>
	public class PayPalPaymentProcessor
	{
        /// <summary>
        /// The _settings.
        /// </summary>
        private readonly PayPalProviderSettings _settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="PayPalPaymentProcessor"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public PayPalPaymentProcessor(PayPalProviderSettings settings)
        {
            this._settings = settings;
        }

		/// <summary>
		/// Get the absolute base URL for this website
		/// </summary>
		/// <returns>
		/// The root URL for the current website
		/// </returns>
		private static string GetWebsiteUrl()
		{
            // TODO USE Umbraco's URL here - websites can have multiple domains ...
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
		/// <param name="settings">
		/// The settings.
		/// </param>
		/// <returns>
		/// A dictionary containing PayPal API credentials.
		/// </returns>
		private static Dictionary<string, string> CreatePayPalApiConfig(PayPalProviderSettings settings)
		{
			return new Dictionary<string, string>
					{
						{ "mode", GetModeString(settings.LiveMode) },
						{ "account1.apiUsername", settings.ApiUsername },
						{ "account1.apiPassword", settings.ApiPassword },
						{ "account1.apiSignature", settings.ApiSignature }
					};
		}

        private PayPalAPIInterfaceServiceService GetPayPalService()
        {
            var config = CreatePayPalApiConfig(this._settings);
			return new PayPalAPIInterfaceServiceService(config);
        }

		private Exception CreateErrorResult(List<ErrorType> errors) {
			var errorText = errors.Count == 0 ? "Unknown error" : ("- " + string.Join("\n- ", errors.Select(item => item.LongMessage)));
			return new Exception(errorText);
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



		private PaymentDetailsType CreatePayPalPaymentDetails(IInvoice invoice, ProcessorArgumentCollection args = null)
		{
			
			string articleBySkuPath = args.GetPayPalProductContentSlug(this._settings.ArticleBySkuPath.IsEmpty() ? null : GetWebsiteUrl() + this._settings.ArticleBySkuPath);
			var currencyCodeType = PayPalCurrency(invoice.CurrencyCode());
			var currencyDecimals = CurrencyDecimals(currencyCodeType);

			decimal itemTotal = 0;
            decimal taxTotal = 0;
			decimal shippingTotal = 0;
			AddressType shipAddress = null;

			var paymentDetailItems = new List<PaymentDetailsItemType>();
			foreach (var item in invoice.Items)
			{
				if (item.LineItemTfKey == Merchello.Core.Constants.TypeFieldKeys.LineItem.TaxKey) 
                {
					taxTotal = item.TotalPrice;
				} 
                else if (item.LineItemTfKey == Merchello.Core.Constants.TypeFieldKeys.LineItem.ShippingKey) 
                {
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
				}
                else if (item.LineItemTfKey == Merchello.Core.Constants.TypeFieldKeys.LineItem.DiscountKey)
                {
                    var discountItem = new PaymentDetailsItemType
                    {
                        Name = item.Name,
                        ItemURL = (articleBySkuPath.IsEmpty() ? null : articleBySkuPath + item.Sku),
                        Amount = new BasicAmountType(currencyCodeType, PriceToString(item.Price*-1, currencyDecimals)),
                        Quantity = item.Quantity,
                    };
                    paymentDetailItems.Add(discountItem);
                    itemTotal -= item.TotalPrice;
                } 
                else {
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
                OrderTotal = new BasicAmountType(currencyCodeType, PriceToString(invoice.Total, currencyDecimals)),
                PaymentAction = PaymentActionCodeType.ORDER,
				InvoiceID = invoice.InvoiceNumberPrefix + invoice.InvoiceNumber.ToString("0"),
				SellerDetails = new SellerDetailsType { PayPalAccountID = this._settings.AccountId },
				PaymentRequestID = "PaymentRequest",
				ShipToAddress = shipAddress,
				NotifyURL = "http://IPNhost"  // TODO what is this
			};

			return paymentDetails;
		}

		
		/// <summary>
		/// Processes the Authorize and AuthorizeAndCapture transactions
		/// </summary>
		/// <param name="invoice">The <see cref="IInvoice"/> to be paid</param>
		/// <param name="payment">The <see cref="IPayment"/> record</param>
		/// <param name="args"></param>
		/// <returns>The <see cref="IPaymentResult"/></returns>
		public IPaymentResult InitializePayment(IInvoice invoice, IPayment payment, ProcessorArgumentCollection args) {

			var setExpressCheckoutRequestDetails = new SetExpressCheckoutRequestDetailsType();

			Func<string, string> adjustUrl = (url) => {
				if (!url.StartsWith("http")) url = GetWebsiteUrl() + (url[0] == '/' ? "" : "/") + url;
				url = url.Replace("{invoiceKey}", invoice.Key.ToString(), StringComparison.InvariantCultureIgnoreCase);
				url = url.Replace("{paymentKey}", payment.Key.ToString(), StringComparison.InvariantCultureIgnoreCase);
				url = url.Replace("{paymentMethodKey}", payment.PaymentMethodKey.ToString(), StringComparison.InvariantCultureIgnoreCase);
				return url;
			};
			
			// Save ReturnUrl and CancelUrl in ExtendedData.
			// They will be usefull in PayPalApiController.

			var returnUrl = args.GetPayPalReturnUrl();
			if (returnUrl.IsEmpty()) returnUrl = this._settings.ReturnUrl;
			if (returnUrl.IsEmpty()) returnUrl = "/";
			returnUrl = adjustUrl(returnUrl);
			payment.ExtendedData.SetValue(Constants.PayPal.ExtendedDataKeys.ReturnUrl, returnUrl);

			var cancelUrl = args.GetPayPalCancelUrl();
			if (cancelUrl.IsEmpty()) cancelUrl = this._settings.CancelUrl;
			if (cancelUrl.IsEmpty()) cancelUrl = "/";
			cancelUrl = adjustUrl(cancelUrl);
			payment.ExtendedData.SetValue(Constants.PayPal.ExtendedDataKeys.CancelUrl, cancelUrl);

			// Set ReturnUrl and CancelUrl of PayPal request to PayPalApiController.
			setExpressCheckoutRequestDetails.ReturnURL = adjustUrl("/umbraco/MerchelloPayPal/PayPalApi/SuccessPayment?InvoiceKey={invoiceKey}&PaymentKey={paymentKey}");
			setExpressCheckoutRequestDetails.CancelURL = adjustUrl("/umbraco/MerchelloPayPal/PayPalApi/AbortPayment?InvoiceKey={invoiceKey}&PaymentKey={paymentKey}");

			//setExpressCheckoutRequestDetails.OrderDescription = "#" + invoice.InvoiceNumber;
			setExpressCheckoutRequestDetails.PaymentDetails = new List<PaymentDetailsType> { this.CreatePayPalPaymentDetails(invoice, args) };

			var setExpressCheckout = new SetExpressCheckoutReq() {
				SetExpressCheckoutRequest = new SetExpressCheckoutRequestType(setExpressCheckoutRequestDetails)
			};

			try {
				var response = this.GetPayPalService().SetExpressCheckout(setExpressCheckout);
				if (response.Ack != AckCodeType.SUCCESS && response.Ack != AckCodeType.SUCCESSWITHWARNING) {
					return new PaymentResult(Attempt<IPayment>.Fail(payment, this.CreateErrorResult(response.Errors)), invoice, false);
				}

				var redirectUrl = string.Format("https://www.{0}paypal.com/cgi-bin/webscr?cmd=_express-checkout&token={1}", (this._settings.LiveMode ? "" : "sandbox."), response.Token);
				payment.ExtendedData.SetValue("RedirectUrl", redirectUrl);
				return new PaymentResult(Attempt<IPayment>.Succeed(payment), invoice, true);

			} catch (Exception ex) {
				return new PaymentResult(Attempt<IPayment>.Fail(payment, ex), invoice, true);
			}

		}

		public IPaymentResult AuthorizePayment(IInvoice invoice, IPayment payment, string token, string payerId)
		{
			var service = this.GetPayPalService();

			var getExpressCheckoutReq = new GetExpressCheckoutDetailsReq() { GetExpressCheckoutDetailsRequest = new GetExpressCheckoutDetailsRequestType(token) };
			
			GetExpressCheckoutDetailsResponseType expressCheckoutDetailsResponse;
			try {
				expressCheckoutDetailsResponse = service.GetExpressCheckoutDetails(getExpressCheckoutReq);
				if (expressCheckoutDetailsResponse.Ack != AckCodeType.SUCCESS && expressCheckoutDetailsResponse.Ack != AckCodeType.SUCCESSWITHWARNING) {
					return new PaymentResult(Attempt<IPayment>.Fail(payment, this.CreateErrorResult(expressCheckoutDetailsResponse.Errors)), invoice, false);
				}
			} catch (Exception ex) {
				return new PaymentResult(Attempt<IPayment>.Fail(payment, ex), invoice, false);
			}
			
			// check if already do
			if (payment.ExtendedData.GetValue(Constants.PayPal.ExtendedDataKeys.PaymentAuthorized) != "true") {
				
				// do express checkout
				var doExpressCheckoutPaymentRequest = new DoExpressCheckoutPaymentRequestType(new DoExpressCheckoutPaymentRequestDetailsType
					{
						Token = token,
						PayerID = payerId,
						PaymentDetails = new List<PaymentDetailsType> { this.CreatePayPalPaymentDetails(invoice) }
					});
				var doExpressCheckoutPayment = new DoExpressCheckoutPaymentReq() { DoExpressCheckoutPaymentRequest = doExpressCheckoutPaymentRequest };

				DoExpressCheckoutPaymentResponseType doExpressCheckoutPaymentResponse;
				try {
					doExpressCheckoutPaymentResponse = service.DoExpressCheckoutPayment(doExpressCheckoutPayment);
					if (doExpressCheckoutPaymentResponse.Ack != AckCodeType.SUCCESS && doExpressCheckoutPaymentResponse.Ack != AckCodeType.SUCCESSWITHWARNING) {
						return new PaymentResult(Attempt<IPayment>.Fail(payment, this.CreateErrorResult(doExpressCheckoutPaymentResponse.Errors)), invoice, false);
					}
				} catch (Exception ex) {
					return new PaymentResult(Attempt<IPayment>.Fail(payment, ex), invoice, false);
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
				if (doAuthorizationResponse.Ack != AckCodeType.SUCCESS && doAuthorizationResponse.Ack != AckCodeType.SUCCESSWITHWARNING)
				{
					return new PaymentResult(Attempt<IPayment>.Fail(payment, this.CreateErrorResult(doAuthorizationResponse.Errors)), invoice, false);
				}
			
				payment.ExtendedData.SetValue(Constants.PayPal.ExtendedDataKeys.AuthorizationId, doAuthorizationResponse.TransactionID);
				payment.ExtendedData.SetValue(Constants.PayPal.ExtendedDataKeys.AmountCurrencyId, currency.ToString());
				payment.ExtendedData.SetValue(Constants.PayPal.ExtendedDataKeys.PaymentAuthorized, "true");
			}

			payment.Authorized = true;

			return new PaymentResult(Attempt<IPayment>.Succeed(payment), invoice, true);
		}
		
		public IPaymentResult CapturePayment(IInvoice invoice, IPayment payment, decimal amount, bool isPartialPayment)
		{
			var service = this.GetPayPalService();
			var authorizationId = payment.ExtendedData.GetValue(Constants.PayPal.ExtendedDataKeys.AuthorizationId);
			var currency = PayPalCurrency(invoice.CurrencyCode());
			var currencyDecimals = CurrencyDecimals(currency);
			
			// do express checkout
			var doCaptureRequest = new DoCaptureRequestType() 
				{
					AuthorizationID = authorizationId,
					Amount = new BasicAmountType(currency, PriceToString(amount, currencyDecimals)),
					CompleteType = (isPartialPayment ? CompleteCodeType.NOTCOMPLETE : CompleteCodeType.COMPLETE)
				};
			var doCaptureReq = new DoCaptureReq() { DoCaptureRequest = doCaptureRequest };

			//if (payment.ExtendedData.GetValue(Constants.ExtendedDataKeys.PaymentCaptured) != "true") {
				DoCaptureResponseType doCaptureResponse;
				try {
					doCaptureResponse = service.DoCapture(doCaptureReq);
					if (doCaptureResponse.Ack != AckCodeType.SUCCESS && doCaptureResponse.Ack != AckCodeType.SUCCESSWITHWARNING) {
						return new PaymentResult(Attempt<IPayment>.Fail(payment, this.CreateErrorResult(doCaptureResponse.Errors)), invoice, false);
					}
				} catch (Exception ex) {
					return new PaymentResult(Attempt<IPayment>.Fail(payment, ex), invoice, false);
				}
			
				payment.ExtendedData.SetValue(Constants.PayPal.ExtendedDataKeys.TransactionId, doCaptureResponse.DoCaptureResponseDetails.PaymentInfo.TransactionID);
				payment.ExtendedData.SetValue(Constants.PayPal.ExtendedDataKeys.PaymentCaptured, "true");	
			//}
			
			payment.Authorized = true;
			payment.Collected = true;
			return new PaymentResult(Attempt<IPayment>.Succeed(payment), invoice, true);
			
		}
		
        public IPaymentResult RefundPayment(IInvoice invoice, IPayment payment)
        {
            var transactionId = payment.ExtendedData.GetValue(Constants.PayPal.ExtendedDataKeys.TransactionId);

            var wrapper = new RefundTransactionReq
            {
                RefundTransactionRequest =
                    {
                        TransactionID = transactionId,
                        RefundType = RefundType.FULL
                    }
            };
            RefundTransactionResponseType refundTransactionResponse = this.GetPayPalService().RefundTransaction(wrapper);

            if (refundTransactionResponse.Ack != AckCodeType.SUCCESS && refundTransactionResponse.Ack != AckCodeType.SUCCESSWITHWARNING)
            {
                return new PaymentResult(Attempt<IPayment>.Fail(payment), invoice, false);
            }

            return new PaymentResult(Attempt<IPayment>.Succeed(payment), invoice, true);
        }

		/// <summary>
		/// The PayPal API version
		/// </summary>
		public static string ApiVersion
		{
			get { return "1.0.4"; }
		}
	}
}
