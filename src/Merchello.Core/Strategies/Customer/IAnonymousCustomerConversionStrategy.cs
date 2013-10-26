using Merchello.Core.Models;

namespace Merchello.Core.Strategies.Customer
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

        /// <summary>
        /// Converts an anonymous customer's basket to a customer basket
        /// </summary>
        /// <returns><see cref="IItemCache"/></returns>
        IItemCache ConvertBasket();

    }
}
