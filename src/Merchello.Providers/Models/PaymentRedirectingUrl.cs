namespace Merchello.Providers.Models
{
	using Merchello.Core;

	/// <summary>
    /// Allows for overriding default redirections for Success and Cancel responses for API returns.
    /// </summary>
    public class PaymentRedirectingUrl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentRedirectingUrl"/> class.
        /// </summary>
        /// <param name="responseFor">
        /// The response for.
        /// </param>
        public PaymentRedirectingUrl(string responseFor)
        {
            Ensure.ParameterNotNullOrEmpty(responseFor, "responseFor");
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