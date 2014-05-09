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

	// # Sample for GetExpressCheckoutDetails API   
	// The GetExpressCheckoutDetails API operation obtains information about
	// an Express Checkout transaction.
	// This sample code uses Merchant .NET SDK to make API call. You can
	// download the SDKs [here](https://github.com/paypal/sdk-packages/tree/gh-pages/merchant-sdk/dotnet)
	public class GetExpressCheckoutDetailsSample
	{
		// # Static constructor for configuration setting
		static GetExpressCheckoutDetailsSample()
		{
		}

		// # GetExpressCheckout API Operation
		// The GetExpressCheckoutDetails API operation obtains information about an Express Checkout transaction
		public GetExpressCheckoutDetailsResponseType GetExpressCheckoutDetailsAPIOperation()
		{
			// Create the GetExpressCheckoutDetailsResponseType object
			GetExpressCheckoutDetailsResponseType responseGetExpressCheckoutDetailsResponseType =
				new GetExpressCheckoutDetailsResponseType();

			try
			{
				// Create the GetExpressCheckoutDetailsReq object
				GetExpressCheckoutDetailsReq getExpressCheckoutDetails = new GetExpressCheckoutDetailsReq();

				// A timestamped token, the value of which was returned by `SetExpressCheckout` response
				GetExpressCheckoutDetailsRequestType getExpressCheckoutDetailsRequest =
					new GetExpressCheckoutDetailsRequestType("EC-3PG29673CT337061M");
				getExpressCheckoutDetails.GetExpressCheckoutDetailsRequest = getExpressCheckoutDetailsRequest;

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
				// Invoke the GetExpressCheckoutDetails method in service wrapper object
				responseGetExpressCheckoutDetailsResponseType = service.GetExpressCheckoutDetails(getExpressCheckoutDetails);

				if (responseGetExpressCheckoutDetailsResponseType != null)
				{
					// Response envelope acknowledgement
					string acknowledgement = "GetExpressCheckoutDetails API Operation - ";
					acknowledgement += responseGetExpressCheckoutDetailsResponseType.Ack.ToString();
					Console.WriteLine(acknowledgement + "\n");

					// # Success values
					if (responseGetExpressCheckoutDetailsResponseType.Ack.ToString().Trim().ToUpper().Equals("SUCCESS"))
					{
						// Unique PayPal Customer Account identification number. This
						// value will be null unless you authorize the payment by
						// redirecting to PayPal after `SetExpressCheckout` call.
						Console.WriteLine("Payer ID : " +
						                  responseGetExpressCheckoutDetailsResponseType.GetExpressCheckoutDetailsResponseDetails.PayerInfo
						                                                               .PayerID + "\n");

					}
						// # Error Values
					else
					{
						List<ErrorType> errorMessages = responseGetExpressCheckoutDetailsResponseType.Errors;
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
			return responseGetExpressCheckoutDetailsResponseType;
		}


	}

}