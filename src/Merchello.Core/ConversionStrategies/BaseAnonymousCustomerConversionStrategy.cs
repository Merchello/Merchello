using Merchello.Core.Models;
using Merchello.Core.Services;

namespace Merchello.Core.ConversionStrategies
{
    public abstract class BaseAnonymousCustomerConversionStrategy
    {
        protected readonly ICustomerService CustomerService;

        protected BaseAnonymousCustomerConversionStrategy(ICustomerService customerService)
        {
            Mandate.ParameterNotNull(customerService, "customerService");

            CustomerService = customerService;
        }
    }
}
