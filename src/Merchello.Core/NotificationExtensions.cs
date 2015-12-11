namespace Merchello.Core
{
    using System.Collections.Generic;
    using Gateways.Payment;

    /// <summary>
    /// Notification related extension methods
    /// </summary>
    public static class NotificationExtensions
    {
        /// <summary>
        /// The notify extension method for <see cref="IPaymentResult"/>
        /// </summary>
        /// <param name="result">
        /// The <see cref="IPaymentResult"/>
        /// </param>
        /// <param name="alias">
        /// The alias.
        /// </param>
        /// <remarks>
        /// This extension is intended for internal emails only.  To use to notify a customer,
        /// use the overloaded version and pass an array of contact addresses.
        /// </remarks>
        public static void Notify(this IPaymentResult result, string alias)
        {
            result.Notify(alias, new string[] { });
        }

        /// <summary>
        /// The notify extension method for <see cref="IPaymentResult"/>
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <param name="alias">
        /// The alias.
        /// </param>
        /// <param name="contacts">
        /// The contacts.
        /// </param>
        public static void Notify(this IPaymentResult result, string alias, IEnumerable<string> contacts)
        {
            Notification.Trigger(alias, result, contacts);
        }       
    }
}