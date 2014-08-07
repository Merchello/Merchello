namespace Merchello.Plugin.Taxation.Avalara.Models
{
    using System;

    /// <summary>
    /// The ava tax api exception.
    /// </summary>
    public class AvaTaxApiException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AvaTaxApiException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public AvaTaxApiException(string message)
            : base(message)
        {            
        }
    }
}