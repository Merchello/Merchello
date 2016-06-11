namespace Merchello.Providers.Payment.PayPal.Models
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
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

    /// <summary>
    /// Extension methods for <see cref="ExpressCheckoutResponse"/>.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1204:StaticElementsMustAppearBeforeInstanceElements", Justification = "Reviewed. Suppression is OK here.")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public static class ExpressCheckoutResponseExtensions
    {
        /// <summary>
        /// Shortcut check of success.
        /// </summary>
        /// <param name="response">
        /// The response.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool Success(this ExpressCheckoutResponse response)
        {
            return response.Ack != null && (response.Ack == AckCodeType.SUCCESS || response.Ack == AckCodeType.SUCCESSWITHWARNING);
        }
    }
}