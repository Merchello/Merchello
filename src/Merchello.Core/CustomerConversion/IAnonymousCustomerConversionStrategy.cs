using Merchello.Core.Models;

namespace Merchello.Core.CustomerConversion
{
    /// <summary>
    /// Defines the anonymous customer conversion strategy
    /// </summary>
    public interface IAnonymousCustomerConversionStrategy
    {
        /// <summary>
        /// Converts an anonymous customer into a customer.
        /// </summary>
        ICustomer ConvertToCustomer();

    }
}
