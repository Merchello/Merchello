namespace Merchello.Core.Exceptions
{
    using System;

    /// <summary>
    /// A custom Merchello specific exception intended to be used to expose more information
    /// about API exceptions.
    /// </summary>
    public class MerchelloApiException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MerchelloApiException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public MerchelloApiException(string message)
            : base(message)
        {
        }
    }
}