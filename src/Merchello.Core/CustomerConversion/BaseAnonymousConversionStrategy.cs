using Merchello.Core.Events;
using Merchello.Core.Models;
using Merchello.Core.Services;

namespace Merchello.Core.CustomerConversion
{
    public abstract class BaseAnonymousConversionStrategy : IAnonymousCustomerConversionStrategy
    {
        protected readonly ICustomerService CustomerService;

        protected BaseAnonymousConversionStrategy(ICustomerService customerService)
        {
            Mandate.ParameterNotNull(customerService, "customerService");

            CustomerService = customerService;
        }

        /// <summary>
        /// Converts an anonymous customer into a customer.
        /// </summary>
        public abstract ICustomer ConvertToCustomer();

        
    }
}
