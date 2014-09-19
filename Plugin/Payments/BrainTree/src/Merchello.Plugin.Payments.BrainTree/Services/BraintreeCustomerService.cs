namespace Merchello.Plugin.Payments.Braintree.Services
{
    using System;
    using global::Braintree;
    using Core;
    using Core.Models;
    using Umbraco.Core.Cache;

    /// <summary>
    /// The braintree customer service.
    /// </summary>
    internal class BraintreeCustomerService : BraintreeServiceBase, IBraintreeCustomerService
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="BraintreeCustomerService"/> class.
        /// </summary>
        /// <param name="braintreeGateway">
        /// The braintree gateway.
        /// </param>
        public BraintreeCustomerService(BraintreeGateway braintreeGateway)
            : this(Core.MerchelloContext.Current, braintreeGateway)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BraintreeCustomerService"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="braintreeGateway">
        /// The braintree gateway.
        /// </param>
        internal BraintreeCustomerService(IMerchelloContext merchelloContext, BraintreeGateway braintreeGateway)
            : base(merchelloContext, braintreeGateway)
        {
        }

        /// <summary>
        /// Creates a Braintree <see cref="Customer"/> from a Merchello <see cref="ICustomer"/>
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <param name="includeCustomerAddresses">A value indicating whether or not to include the customer addresses in the creation</param>
        /// <returns>
        /// The <see cref="Customer"/>.
        /// </returns>
        public Customer Create(ICustomer customer, bool includeCustomerAddresses = true)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <param name="includeCustomerAddresses">A value indicating whether or not to include the customer addresses in the update</param>
        /// <returns>
        /// The <see cref="Customer"/>.
        /// </returns>
        public Customer Update(ICustomer customer, bool includeCustomerAddresses = true)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes the Braintree <see cref="Customer"/> corresponding with the Merchello <see cref="ICustomer"/>
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        public void Delete(ICustomer customer)
        {
            throw new NotImplementedException();
        }

        public Customer GetBraintreeCustomer(Guid customerKey)
        {
            throw new NotImplementedException();
        }

        public Customer GetBraintreeCustomer(ICustomer customer)
        {
            throw new NotImplementedException();
        }

        public string GenerateClientRequestToken()
        {
            throw new NotImplementedException();
        }

        public string GenerateClientRequestToken(ICustomer customer)
        {
            throw new NotImplementedException();
        }

        public bool Exists(ICustomer customer)
        {
            throw new NotImplementedException();
        }
    }
}