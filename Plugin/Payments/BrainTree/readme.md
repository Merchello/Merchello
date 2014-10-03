# Merchello.Plugin.Payments.Braintree

     // Author: Rusty Swayne
     // Twitter: @rustyswayne
     // Compatible: Merchello 1.4.1 and Umbraco 7.1.7

## About this package

This Merchello plugin wraps the Braintree .Net SDK library allowing the developer to interact with the Braintree API using Merchello interfaces such as ICustomer and IInvoice.  Some responses are likewise returned as Merchello classes such as IPaymentResult where it made sense to do so.

[Braintree .Net SDK documentation](https://developers.braintreepayments.com/javascript+dotnet/sdk/server)

     PM> Install-Package Braintree 


## Primary Classes

### BraintreeProviderSettings

     // Namespace : Merchello.Plugin.Payments.Braintree.Models

The BraintreeProviderSettings class contains the properties and configurations needed by the Braintree .Net SDK to perform requests against the Braintree API.  These configurations are set in the Merchello Back Office when you "Active" the gateway provider.  Alternatively, you can create a few App_Settings (or whatever) so that you can instantiate the BraintreeApiService in instances where you do not wish to use Braintree as a payment processor but would like to manage subscriptions.

**Example**
     
     internal class BraintreeHelper
     {
	
	     public static BraintreeProviderSettings GetBraintreeProviderSettings()
	        {
	            return new BraintreeProviderSettings()
	                            {
	                                Environment = Environment.SANDBOX,
	                                PublicKey = ConfigurationManager.AppSettings["publicKey"],
	                                PrivateKey = ConfigurationManager.AppSettings["privateKey"],
	                                MerchantId = ConfigurationManager.AppSettings["merchantId"],
	                                MerchantDescriptor = new MerchantDescriptor()
	                                    {
	                                        Name = ConfigurationManager.AppSettings["merchantName"],
	                                        Url = ConfigurationManager.AppSettings["merchantUrl"],
	                                        Phone = ConfigurationManager.AppSettings["merchantPhone"]
	                                    },
	                                DefaultTransactionOption = (TransactionOption)Enum.Parse(typeof(TransactionOption), ConfigurationManager.AppSettings["defaultTransactionOption"])
	                            };
	        }
     }

### BraintreePaymentGatewayProvider

     // Namespace :  Merchello.Plugin.Payments.Braintree.Provider

This is the provider class Merchello uses to process direct payment transactions in the typical checkout work flow.  



### BraintreeApiService   

     // Namespace: Merchello.Plugin.Payments.Braintree.Services

The BraintreeApiService is internally used by the BraintreePaymentGatewayProvider methods but it can be used as a standalone object to perform operations against the Braintree API.


To instantiate the BrainTreeApiService

    var braintreeApiService = new BraintreeApiService(BraintreeHelper.GetBraintreeProviderSettings());

This service lazy loads the four API Service wrappers:

[Customer Management](https://developers.braintreepayments.com/javascript+dotnet/sdk/server/customer-management/create)

    // Customer Management
    var braintreeCustomerApiService = braintreeApiService.Customer;


[Payment Method Management](https://developers.braintreepayments.com/javascript+dotnet/sdk/server/payment-method-management/create)

    // Payment Method Management
    var paymentMethodApiService = braintreeApiService.PaymentMethod;

[Recurring Billing](https://developers.braintreepayments.com/javascript+dotnet/sdk/server/recurring-billing/overview)

    // Recurring Billing
    var subscriptionApiService = braintreeApiService.Subscription;

[TransactionProcessing](https://developers.braintreepayments.com/javascript+dotnet/sdk/server/transaction-processing/overview)

    // Transaction Processing
    var transactionApiService = braintreeApiService.Transaction;

#### Why use the BraintreeApiService and not simply use the Braintree .Net SDK directly?

* The service allows the developer to directly use some Merchello classes such as **ICustomer, IInvoice and IPaymentResult**
* Provides some fault tolerance on API calls, usually returning Umbraco **Attempt<T>**
* Logs errors to the Umbraco Log /App_Data/Logs using Umbraco's **LogHelper**
* Internally manages HttpRuntimeCache using using the **ApplicationContext.Current.ApplicationCache.RuntimeCache**