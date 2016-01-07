namespace Merchello.Core.Checkout.Data
{
    using Merchello.Core.Models;

    using Newtonsoft.Json;

    using Umbraco.Core;

    /// <summary>
    /// Represents a data repository that saves data with the customer record in the customer <see cref="ExtendedDataCollection"/>
    /// </summary>
    public class CheckoutCustomerExtendedDataRepository 
    {
        /// <summary>
        /// The <see cref="ICustomerBase"/>.
        /// </summary>
        private readonly ICustomerBase _customer;

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutCustomerExtendedDataRepository"/> class.
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        public CheckoutCustomerExtendedDataRepository(ICustomerBase customer)
        {
            Mandate.ParameterNotNull(customer, "customer");

            _customer = customer;
        }

        public void Save(string alias, object value, bool raiseEvents = true)
        {
            var json = JsonConvert.SerializeObject(value);

        }

        public string Get(string alias)
        {
            throw new System.NotImplementedException();
        }

        public T Get<T>(string alias) where T : class, new()
        {
            throw new System.NotImplementedException();
        }
    }
}