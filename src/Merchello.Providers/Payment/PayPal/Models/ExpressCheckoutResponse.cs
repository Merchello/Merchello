namespace Merchello.Providers.Payment.PayPal.Models
{
    using System.Collections.Generic;
    using System.Linq;

    using global::PayPal.PayPalAPIInterfaceService.Model;

    /// <summary>
    /// The express checkout response.
    /// </summary>
    public class ExpressCheckoutResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressCheckoutResponse"/> class.
        /// </summary>
        public ExpressCheckoutResponse()
        {
            ErrorTypes = Enumerable.Empty<ErrorType>();
        }

        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets the success value.
        /// </summary>
        public AckCodeType? Ack { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the build number.
        /// </summary>
        public string Build { get; set; }

        /// <summary>
        /// Gets or sets the errors.
        /// </summary>
        public IEnumerable<ErrorType> ErrorTypes { get; set; } 

        /// <summary>
        /// Gets or sets the redirect url.
        /// </summary>
        public string RedirectUrl { get; set; }
    }
}