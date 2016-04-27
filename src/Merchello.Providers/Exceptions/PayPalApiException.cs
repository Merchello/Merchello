namespace Merchello.Providers.Exceptions
{
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Exceptions;

    using PayPal.PayPalAPIInterfaceService.Model;

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
            ErrorTypes = Enumerable.Empty<ErrorType>();
        }

        /// <summary>
        /// Gets or sets the error types.
        /// </summary>
        public IEnumerable<ErrorType> ErrorTypes { get; set; } 
    }
}