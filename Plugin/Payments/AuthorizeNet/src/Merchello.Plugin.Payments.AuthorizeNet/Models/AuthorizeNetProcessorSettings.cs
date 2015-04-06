namespace Merchello.Plugin.Payments.AuthorizeNet.Models
{
    /// <summary>
    /// The authorize net processor settings.
    /// </summary>
    public class AuthorizeNetProcessorSettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether use sandbox.
        /// </summary>
        public bool UseSandbox { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizeNetProcessorSettings"/> class.
        /// </summary>
        public AuthorizeNetProcessorSettings()
        {
            // set the defaults
            DelimitedData = true;
            DelimitedChar = "|";
            Method = "CC";
        }

        /// <summary>
        /// Gets or sets the login id.
        /// </summary>
        public string LoginId { get; set; }

        /// <summary>
        /// Gets or sets the transaction key.
        /// </summary>
        public string TransactionKey { get; set; }

        /// <summary>
        /// Gets or sets the method.
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether delimited data.
        /// </summary>
        public bool DelimitedData { get; set; }

        /// <summary>
        /// Gets or sets the delimited char.
        /// </summary>
        public string DelimitedChar { get; set; }

        /// <summary>
        /// Gets or sets the encap char.
        /// </summary>
        public string EncapChar { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether relay response.
        /// </summary>
        public bool RelayResponse { get; set; }

        /// <summary>
        /// Gets the api version.
        /// </summary>
        public string ApiVersion
        {
            get { return AuthorizeNetPaymentProcessor.ApiVersion; }
        }
    }
}