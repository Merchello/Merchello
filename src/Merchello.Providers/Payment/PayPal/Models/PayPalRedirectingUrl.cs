namespace Merchello.Providers.Payment.PayPal.Models
{
    using Umbraco.Core;

    /// <summary>
    /// Allows for overriding default redirections for Success and Cancel responses for PayPal API returns.
    /// </summary>
    public class PayPalRedirectingUrl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PayPalRedirectingUrl"/> class.
        /// </summary>
        /// <param name="responseFor">
        /// The response for.
        /// </param>
        public PayPalRedirectingUrl(string responseFor)
        {
            Mandate.ParameterNotNullOrEmpty(responseFor, "responseFor");
            this.ResponseFor = responseFor;
            this.RedirectingToUrl = "/";
        }

        /// <summary>
        /// Gets or sets the redirecting to url.
        /// </summary>
        public string RedirectingToUrl { get; set; }

        /// <summary>
        /// Gets the response for.
        /// </summary>
        public string ResponseFor { get; private set; }
    }
}