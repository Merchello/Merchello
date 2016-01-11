namespace Merchello.Core.Checkout
{
    using System;

    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// The <see cref="ICheckoutContext"/> event args.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the second argument
    /// </typeparam>
    public class CheckoutEventArgs<T> : EventArgs 
        where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutEventArgs{T}"/> class.
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <param name="item">
        /// The item.
        /// </param>
        public CheckoutEventArgs(ICustomerBase customer, T item)
        {
            Mandate.ParameterNotNull(customer, "customer");
            Mandate.ParameterNotNull(item, "item");

            this.Customer = customer;
            this.Item = item;
        }

        /// <summary>
        /// Gets the customer.
        /// </summary>
        public ICustomerBase Customer { get; private set; }

        /// <summary>
        /// Gets the item.
        /// </summary>
        public T Item { get; private set; }
    }
}