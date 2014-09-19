namespace Merchello.Plugin.Payments.Braintree.Services
{
    using System;

    using global::Braintree;

    using Merchello.Core.Models;

    public interface IBraintreeCustomerService
    {
        Customer GetBraintreeCustomer(Guid customerKey);

        Customer GetBraintreeCustomer(ICustomer customer);

        string GenerateClientRequestToken();

        string GenerateClientRequestToken(Guid customerKey);


    }

   
}