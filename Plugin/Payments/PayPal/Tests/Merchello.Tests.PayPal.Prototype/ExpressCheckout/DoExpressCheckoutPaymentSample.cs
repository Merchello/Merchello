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

	// # Sample for  DoExpressCheckoutPayment API   
	// Authorize a payment.
	// This sample code uses Merchant .NET SDK to make API call. You can
	// download the SDKs [here](https://github.com/paypal/sdk-packages/tree/gh-pages/merchant-sdk/dotnet)
	public class DoExpressCheckoutPaymentSample
	{
		// # Static constructor for configuration setting
		static DoExpressCheckoutPaymentSample()
		{
		}

		// # DoExpressCheckoutPayment API Operation
		// The DoExpressCheckoutPayment API operation completes an Express Checkout transaction. 
		// If you set up a billing agreement in your SetExpressCheckout API call, 
		// the billing agreement is created when you call the DoExpressCheckoutPayment API operation. 
		public DoExpressCheckoutPaymentResponseType DoExpressCheckoutPaymentAPIOperation()
		{
			// Create the DoExpressCheckoutPaymentResponseType object
			DoExpressCheckoutPaymentResponseType responseDoExpressCheckoutPaymentResponseType =
				new DoExpressCheckoutPaymentResponseType();

			try
			{
				// Create the DoExpressCheckoutPaymentReq object
				DoExpressCheckoutPaymentReq doExpressCheckoutPayment = new DoExpressCheckoutPaymentReq();

				DoExpressCheckoutPaymentRequestDetailsType doExpressCheckoutPaymentRequestDetails =
					new DoExpressCheckoutPaymentRequestDetailsType();

				// The timestamped token value that was returned in the
				// `SetExpressCheckout` response and passed in the
				// `GetExpressCheckoutDetails` request.
				doExpressCheckoutPaymentRequestDetails.Token = "EC-3PG29673CT337061M";

				// Unique paypal buyer account identification number as returned in
				// `GetExpressCheckoutDetails` Response
				doExpressCheckoutPaymentRequestDetails.PayerID = "WJ3Q38FZ9FDYS";

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
				// Note:
				// You cannot set this field to Sale in SetExpressCheckout request and
				// then change the value to Authorization or Order in the
				// DoExpressCheckoutPayment request. If you set the field to
				// Authorization or Order in SetExpressCheckout, you may set the field
				// to Sale.
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
				paymentDetailsList.Add(paymentDetails1);
				paymentDetailsList.Add(paymentDetails2);
				doExpressCheckoutPaymentRequestDetails.PaymentDetails = paymentDetailsList;

				DoExpressCheckoutPaymentRequestType doExpressCheckoutPaymentRequest =
					new DoExpressCheckoutPaymentRequestType(doExpressCheckoutPaymentRequestDetails);
				doExpressCheckoutPayment.DoExpressCheckoutPaymentRequest = doExpressCheckoutPaymentRequest;

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
				// Invoke the DoExpressCheckoutPayment method in service wrapper object
				responseDoExpressCheckoutPaymentResponseType = service.DoExpressCheckoutPayment(doExpressCheckoutPayment);

				if (responseDoExpressCheckoutPaymentResponseType != null)
				{
					// Response envelope acknowledgement
					string acknowledgement = "DoExpressCheckoutPayment API Operation - ";
					acknowledgement += responseDoExpressCheckoutPaymentResponseType.Ack.ToString();
					Console.WriteLine(acknowledgement + "\n");

					// # Success values
					if (responseDoExpressCheckoutPaymentResponseType.Ack.ToString().Trim().ToUpper().Equals("SUCCESS"))
					{
						// Transaction identification number of the transaction that was
						// created.
						// This field is only returned after a successful transaction
						// for DoExpressCheckout has occurred.
						if (responseDoExpressCheckoutPaymentResponseType.DoExpressCheckoutPaymentResponseDetails.PaymentInfo != null)
						{
							IEnumerator<PaymentInfoType> paymentInfoIterator =
								responseDoExpressCheckoutPaymentResponseType.DoExpressCheckoutPaymentResponseDetails.PaymentInfo.GetEnumerator();
							while (paymentInfoIterator.MoveNext())
							{
								PaymentInfoType paymentInfo = paymentInfoIterator.Current;
								Console.WriteLine("Transaction ID : " + paymentInfo.TransactionID + "\n");
							}
						}
					}
						// # Error Values
					else
					{
						List<ErrorType> errorMessages = responseDoExpressCheckoutPaymentResponseType.Errors;
						foreach (ErrorType error in errorMessages)
						{
							Console.WriteLine("API Error Message : " + error.LongMessage + "\n");
						}
					}
				}

				return responseDoExpressCheckoutPaymentResponseType;
			}
				// # Exception log    
			catch (System.Exception ex)
			{
				// Log the exception message       
				Console.WriteLine("Error Message : " + ex.Message);
			}
			return responseDoExpressCheckoutPaymentResponseType;
		}

	}

}