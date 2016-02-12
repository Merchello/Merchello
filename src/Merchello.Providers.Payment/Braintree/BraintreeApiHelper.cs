namespace Merchello.Providers.Payment.Braintree
{
    using System;
    using System.Web.Configuration;

    using global::Braintree;

    using Merchello.Core;
    using Merchello.Core.Models;

    using Merchello.Providers.Payment.Braintree.Models;



    /// <summary>
    /// Utility class that assists in Braintree API calls
    /// </summary>
    public class BraintreeApiHelper
    {
        /// <summary>
        /// Gets the <see cref="BraintreeProviderSettings"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="BraintreeProviderSettings"/>.
        /// </returns>
        /// <remarks>
        /// TODO : Braintree "Environment" has a DEVELOPMENT setting as well
        /// </remarks>
        public static BraintreeProviderSettings GetBraintreeProviderSettings()
        {
            return new BraintreeProviderSettings()
            {
                Environment = WebConfigurationManager.AppSettings["braintreeEnvironment"].Equals("SANDBOX") ? EnvironmentType.Sandbox : EnvironmentType.Production,
                PublicKey = WebConfigurationManager.AppSettings["publicKey"],
                PrivateKey = WebConfigurationManager.AppSettings["privateKey"],
                MerchantId = WebConfigurationManager.AppSettings["merchantId"],
                MerchantDescriptor = new MerchantDescriptor()
                {
                    Name = WebConfigurationManager.AppSettings["merchantName"],
                    Url = WebConfigurationManager.AppSettings["merchantUrl"],
                    Phone = WebConfigurationManager.AppSettings["merchantPhone"]
                },
                DefaultTransactionOption = (TransactionOption)Enum.Parse(typeof(TransactionOption), WebConfigurationManager.AppSettings["defaultTransactionOption"])
            };
        }




        /// <summary>
        /// Gets the Merchello <see cref="ICustomer"/> for a given Braintree <see cref="Transaction"/>.
        /// </summary>
        /// <param name="transaction">
        /// The transaction.
        /// </param>
        /// <returns>
        /// The <see cref="ICustomer"/>.
        /// </returns>
        public static ICustomer GetCustomerForTransaction(Transaction transaction)
        {
            return GetCustomerForTransaction(MerchelloContext.Current, transaction);
        }

        /// <summary>
        /// Gets the Merchello <see cref="ICustomer"/> for a given Braintree <see cref="Transaction"/>.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="transaction">
        /// The transaction.
        /// </param>
        /// <returns>
        /// The <see cref="ICustomer"/>.
        /// </returns>
        public static ICustomer GetCustomerForTransaction(IMerchelloContext merchelloContext, Transaction transaction)
        {
            var customerService = merchelloContext.Services.CustomerService;

            return customerService.GetByKey(new Guid(transaction.Customer.Id));
        }
    }
}