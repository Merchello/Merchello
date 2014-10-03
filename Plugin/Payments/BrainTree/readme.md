# Merchello.Plugin.Payments.Braintree

     // Author: Rusty Swayne
     // Twitter: @rustyswayne
     // Compatible: Merchello 1.4.1 and Umbraco 7.1.7

## About this package

This Merchello plugin wraps the Braintree .Net SDK library allowing the developer to interact with the Braintree API using Merchello interfaces such as ICustomer and IInvoice.  Some responses are likewise returned as Merchello classes such as IPaymentResult where it made sense to do so.

[Braintree .Net SDK documentation](https://developers.braintreepayments.com/javascript+dotnet/sdk/server)

     PM> Install-Package Braintree 


## Primary Classes

### BraintreePaymentGatewayProvider

     // Namespace :  Merchello.Plugin.Payments.Braintree.Provider

This is the provider class Merchello uses to process direct payment transactions in the typical checkout work flow.  


### BraintreeApiService   
     