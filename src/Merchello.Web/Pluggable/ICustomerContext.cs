namespace Merchello.Web.Pluggable
{
    using Merchello.Core.Models;

    /// <summary>
    /// Defines a Merchello Customer Context.
    /// </summary>
    public interface ICustomerContext
    {
        /// <summary>
        /// Gets the current customer.
        /// </summary>
        ICustomerBase CurrentCustomer { get;  }

        /// <summary>
        /// Sets a value in the encrypted Merchello cookie
        /// </summary>
        /// <param name="key">
        /// The key for the value
        /// </param>
        /// <param name="value">
        /// The actual value to be save.
        /// </param>
        /// <remarks>
        /// Keep in mind this is just a cookie which has limited size.  This is intended for 
        /// small bits of information.
        /// </remarks>
        void SetValue(string key, string value);

        /// <summary>
        /// Gets a value from the encrypted Merchello cookie
        /// </summary>
        /// <param name="key">
        /// The key of the value to retrieve
        /// </param>
        /// <returns>
        /// The value stored in the cookie as a string.
        /// </returns>
        string GetValue(string key);

        /// <summary>
        /// Reinitializes the customer context
        /// </summary>
        /// <param name="customer">
        /// The <see cref="CustomerBase"/>
        /// </param>
        /// <remarks>
        /// Sometimes useful to clear the various caches used internally in the customer context
        /// </remarks>
        void Reinitialize(ICustomerBase customer);
    }
}