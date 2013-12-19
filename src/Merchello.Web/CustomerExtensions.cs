using Merchello.Core.Models;
using Merchello.Web.Models;

namespace Merchello.Web
{
    public static class CustomerExtensions
    {
        /// <summary>
        /// Returns the <see cref="IBasket"/> for the customer
        /// </summary>
        /// <param name="customer"><see cref="ICustomerBase"/></param>
        /// <returns><see cref="IBasket"/></returns>
        public static IBasket Basket(this ICustomerBase customer)
        {
            return Web.Basket.GetBasket(customer);
        }
         
    }
}