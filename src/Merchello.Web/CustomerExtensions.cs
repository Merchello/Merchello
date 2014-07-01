namespace Merchello.Web
{
    using Merchello.Core.Models;
    using Merchello.Web.Workflow;

    /// <summary>
    /// Extension methods for customer classes.
    /// </summary>
    public static class CustomerExtensions
    {
        /// <summary>
        /// Returns the <see cref="IBasket"/> for the customer
        /// </summary>
        /// <param name="customer">The <see cref="ICustomerBase"/></param>
        /// <returns>The <see cref="IBasket"/></returns>
        public static IBasket Basket(this ICustomerBase customer)
        {
            return Workflow.Basket.GetBasket(customer);
        }
    }
}