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
        /// <param name="model">
        /// The model.
        /// </param>
        public static void Notify(this IPaymentResult result, string alias, object model)
        {
            result.Notify(alias, model, new string[] { });
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
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="contacts">
        /// The contacts.
        /// </param>
        public static void Notify(this IPaymentResult result, string alias, object model, IEnumerable<string> contacts)
        {
            Notification.Trigger(alias, result, contacts);
        }       
    }
}