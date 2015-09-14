namespace Merchello.Core.Exceptions
{
    using System;

    /// <summary>
    /// An exception for invalid SKUs.
    /// </summary>
    public class InvalidSkuException : Exception 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidSkuException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public InvalidSkuException(string message)
            : base(message)
        {            
        }
    }
}