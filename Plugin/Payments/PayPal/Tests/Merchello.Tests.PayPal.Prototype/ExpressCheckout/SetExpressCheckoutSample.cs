// # Namespaces  
using System;
using System.Collections.Generic;
// # NuGet Install          
// Visual Studio 2012 and 2010 Command:  
// Install-Package PayPalMerchantSDK        
// Visual Studio 2005 and 2008 (NuGet.exe) Command:   
// install PayPalMerchantSDK      
using PayPal.PayPalAPIInterfaceService;
using PayPal.PayPalAPIInterfaceService.Model;

namespace Merchello.Tests.PayPal.Prototype.ExpressCheckout
{
	// # Sample for SetExpressCheckout API  
	// The SetExpressCheckout API operation initiates an Express Checkout
	// transaction.
	// This sample code uses Merchant .NET SDK to make API call. You can
	// download the SDKs [here](https://github.com/paypal/sdk-packages/tree/gh-pages/merchant-sdk/dotnet)
	public class SetExpressCheckoutSample
	{
		// # Static constructor for configuration setting
		static SetExpressCheckoutSample()
		{
		}

		// # SetExpressCheckout API Operation
		// The SetExpressCheckout API operation initiates an Express Checkout transaction. 
		public SetExpressCheckoutResponseType SetExpressCheckoutAPIOperation()
		{
			// Create the SetExpressCheckoutResponseType object
			SetExpressCheckoutResponseType responseSetExpressCheckoutResponseType = new SetExpressCheckoutResponseType();

			try
			{
				// # SetExpressCheckoutReq
				SetExpressCheckoutRequestDetailsType setExpressCheckoutRequestDetails = new SetExpressCheckoutRequestDetailsType();

				// URL to which the buyer's browser is returned after choosing to pay
				// with PayPal. For digital goods, you must add JavaScript to this page
				// to close the in-context experience.
				// `Note:
				// PayPal recommends that the value be the final review page on which
				// the buyer confirms the order and payment or billing agreement.`
				setExpressCheckoutRequestDetails.ReturnURL = "http://localhost/return";

				// URL to which the buyer is returned if the buyer does not approve the
				// use of PayPal to pay you. For digital goods, you must add JavaScript
				// to this page to close the in-context experience.
				// `Note:
				// PayPal recommends that the value be the original page on which the
				// buyer chose to pay with PayPal or establish a billing agreement.`
				setExpressCheckoutRequestDetails.CancelURL = "http://localhost/cancel";

				// # Payment Information
				// list of information about the payment
				List<PaymentDetailsType> paymentDetailsList = new List<PaymentDetailsType>();

				// information about the first payment
				PaymentDetailsType paymentDetails1 = new PaymentDetailsType();

				// Total cost of the transaction to the buyer. If shipping cost and tax
				// charges are known, include them in this value. If not, this value
				// should be the current sub-total of the order.
				//
				// If the transaction includes one or more one-time purchases, this field must be equal to
				// the sum of the purchases. Set this field to 0 if the transaction does
				// not include a one-time purchase such as when you set up a billing
				// agreement for a recurring payment that is not immediately charged.
				// When the field is set to 0, purchase-specific fields are ignored.
				//
				// * `Currency Code` - You must set the currencyID attribute to one of the
				// 3-character currency codes for any of the supported PayPal
				// currencies.
				// * `Amount`
				BasicAmountType orderTotal1 = new BasicAmountType(CurrencyCodeType.USD, "2.00");
				paymentDetails1.OrderTotal = orderTotal1;

				// How you want to obtain payment. When implementing parallel payments,
				// this field is required and must be set to `Order`. When implementing
				// digital goods, this field is required and must be set to `Sale`. If the
				// transaction does not include a one-time purchase, this field is
				// ignored. It is one of the following values:
				//
				// * `Sale` - This is a final sale for which you are requesting payment
				// (default).
				// * `Authorization` - This payment is a basic authorization subject to
				// settlement with PayPal Authorization and Capture.
				// * `Order` - This payment is an order authorization subject to
				// settlement with PayPal Authorization and Capture.
				// `Note:
				// You cannot set this field to Sale in SetExpressCheckout request and
				// then change the value to Authorization or Order in the
				// DoExpressCheckoutPayment request. If you set the field to
				// Authorization or Order in SetExpressCheckout, you may set the field
				// to Sale.`
				paymentDetails1.PaymentAction = PaymentActionCodeType.ORDER;

				// Unique identifier for the merchant. For parallel payments, this field
				// is required and must contain the Payer Id or the email address of the
				// merchant.
				SellerDetailsType sellerDetails1 = new SellerDetailsType();
				sellerDetails1.PayPalAccountID = "konstantin_merchant@scandiaconsulting.com";
				paymentDetails1.SellerDetails = sellerDetails1;

				// A unique identifier of the specific payment request, which is
				// required for parallel payments.
				paymentDetails1.PaymentRequestID = "PaymentRequest1";

				// `Address` to which the order is shipped, which takes mandatory params:
				//
				// * `Street Name`
				// * `City`
				// * `State`
				// * `Country`
				// * `Postal Code`
				AddressType shipToAddress1 = new AddressType();
				shipToAddress1.Street1 = "Ape Way";
				shipToAddress1.CityName = "Austin";
				shipToAddress1.StateOrProvince = "TX";
				shipToAddress1.Country = CountryCodeType.US;
				shipToAddress1.PostalCode = "78750";

				paymentDetails1.ShipToAddress = shipToAddress1;

				// IPN URL
				// * PayPal Instant Payment Notification is a call back system that is initiated when a transaction is completed        
				// * The transaction related IPN variables will be received on the call back URL specified in the request       
				// * The IPN variables have to be sent back to the PayPal system for validation, upon validation PayPal will send a response string "VERIFIED" or "INVALID"     
				// * PayPal would continuously resend IPN if a wrong IPN is sent        
				paymentDetails1.NotifyURL = "http://IPNhost";

				// information about the second payment
				PaymentDetailsType paymentDetails2 = new PaymentDetailsType();
				// Total cost of the transaction to the buyer. If shipping cost and tax
				// charges are known, include them in this value. If not, this value
				// should be the current sub-total of the order.
				//
				// If the transaction includes one or more one-time purchases, this field must be equal to
				// the sum of the purchases. Set this field to 0 if the transaction does
				// not include a one-time purchase such as when you set up a billing
				// agreement for a recurring payment that is not immediately charged.
				// When the field is set to 0, purchase-specific fields are ignored.
				//
				// * `Currency Code` - You must set the currencyID attribute to one of the
				// 3-character currency codes for any of the supported PayPal
				// currencies.
				// * `Amount`
				BasicAmountType orderTotal2 = new BasicAmountType(CurrencyCodeType.USD, "4.00");
				paymentDetails2.OrderTotal = orderTotal2;

				// How you want to obtain payment. When implementing parallel payments,
				// this field is required and must be set to `Order`. When implementing
				// digital goods, this field is required and must be set to `Sale`. If the
				// transaction does not include a one-time purchase, this field is
				// ignored. It is one of the following values:
				//
				// * `Sale` - This is a final sale for which you are requesting payment
				// (default).
				// * `Authorization` - This payment is a basic authorization subject to
				// settlement with PayPal Authorization and Capture.
				// * `Order` - This payment is an order authorization subject to
				// settlement with PayPal Authorization and Capture.
				// `Note:
				// You cannot set this field to Sale in SetExpressCheckout request and
				// then change the value to Authorization or Order in the
				// DoExpressCheckoutPayment request. If you set the field to
				// Authorization or Order in SetExpressCheckout, you may set the field
				// to Sale.`
				paymentDetails2.PaymentAction = PaymentActionCodeType.ORDER;

				// Unique identifier for the merchant. For parallel payments, this field
				// is required and must contain the Payer Id or the email address of the
				// merchant.
				SellerDetailsType sellerDetails2 = new SellerDetailsType();
				sellerDetails2.PayPalAccountID = "konstantin_merchant@scandiaconsulting.com";
				paymentDetails2.SellerDetails = sellerDetails2;

				// A unique identifier of the specific payment request, which is
				// required for parallel payments.
				paymentDetails2.PaymentRequestID = "PaymentRequest2";

				// IPN URL
				// * PayPal Instant Payment Notification is a call back system that is initiated when a transaction is completed        
				// * The transaction related IPN variables will be received on the call back URL specified in the request       
				// * The IPN variables have to be sent back to the PayPal system for validation, upon validation PayPal will send a response string "VERIFIED" or "INVALID"     
				// * PayPal would continuously resend IPN if a wrong IPN is sent        
				paymentDetails2.NotifyURL = "http://IPNhost";

				// `Address` to which the order is shipped, which takes mandatory params:
				//
				// * `Street Name`
				// * `City`
				// * `State`
				// * `Country`
				// * `Postal Code`
				AddressType shipToAddress2 = new AddressType();
				shipToAddress2.Street1 = "Ape Way";
				shipToAddress2.CityName = "Austin";
				shipToAddress2.StateOrProvince = "TX";
				shipToAddress2.Country = CountryCodeType.US;
				shipToAddress2.PostalCode = "78750";
				paymentDetails2.ShipToAddress = shipToAddress2;

				paymentDetailsList.Add(paymentDetails1);
				paymentDetailsList.Add(paymentDetails2);

				setExpressCheckoutRequestDetails.PaymentDetails = paymentDetailsList;

				SetExpressCheckoutReq setExpressCheckout = new SetExpressCheckoutReq();
				SetExpressCheckoutRequestType setExpressCheckoutRequest = new SetExpressCheckoutRequestType(setExpressCheckoutRequestDetails);

				setExpressCheckout.SetExpressCheckoutRequest = setExpressCheckoutRequest;

				var config = new Dictionary<string, string>
					{
						{"mode", "sandbox"},
						{"account1.apiUsername", "konstantin_merchant_api1.scandiaconsulting.com"},
						{"account1.apiPassword", "1398157263"},
						{"account1.apiSignature", "AFcWxV21C7fd0v3bYYYRCpSSRl31AlRjlcug7qV.VXWV14E1KtmQPsPL"}
					};

				// Create the service wrapper object to make the API call
				PayPalAPIInterfaceServiceService service = new PayPalAPIInterfaceServiceService(config);

				// # API call            
				// Invoke the SetExpressCheckout method in service wrapper object
				responseSetExpressCheckoutResponseType = service.SetExpressCheckout(setExpressCheckout);

				if (responseSetExpressCheckoutResponseType != null)
				{
					// Response envelope acknowledgement
					string acknowledgement = "SetExpressCheckout API Operation - ";
					acknowledgement += responseSetExpressCheckoutResponseType.Ack.ToString();
					Console.WriteLine(acknowledgement + "\n");

					// # Success values
					if (responseSetExpressCheckoutResponseType.Ack.ToString().Trim().ToUpper().Equals("SUCCESS"))
					{
						// # Redirecting to PayPal for authorization
						// Once you get the "Success" response, needs to authorise the
						// transaction by making buyer to login into PayPal. For that,
						// need to construct redirect url using EC token from response.
						// For example,
						// `redirectURL="https://www.sandbox.paypal.com/cgi-bin/webscr?cmd=_express-checkout&token="+setExpressCheckoutResponse.Token;`

						// Express Checkout Token
						Console.WriteLine("Express Checkout Token : " + responseSetExpressCheckoutResponseType.Token + "\n");
					}
					// # Error Values
					else
					{
						List<ErrorType> errorMessages = responseSetExpressCheckoutResponseType.Errors;
						foreach (ErrorType error in errorMessages)
						{
							Console.WriteLine("API Error Message : " + error.LongMessage + "\n");
						}
					}
				}

			}
			// # Exception log    
			catch (System.Exception ex)
			{
				// Log the exception message       
				Console.WriteLine("Error Message : " + ex.Message);
			}
			return responseSetExpressCheckoutResponseType;
		}

	}

}
