namespace Merchello.Providers.Exceptions
{
    using Merchello.Core.Exceptions;

    /// <summary>
    /// An exception for PayPal errors.
    /// </summary>
    public class PayPalApiException : MerchelloApiException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PayPalApiException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public PayPalApiException(string message)
            : base(message)
        {
        }
    }
}