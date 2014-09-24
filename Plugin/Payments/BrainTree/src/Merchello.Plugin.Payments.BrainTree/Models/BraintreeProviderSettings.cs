namespace Merchello.Plugin.Payments.Braintree.Models
{
    using global::Braintree;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// The BrainTree provider settings used to access the BrainTree Payment API.
    /// </summary>
    public class BraintreeProviderSettings
    {
        /// <summary>
        /// Gets or sets the environment.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public Environment Environment { get; set; }

        /// <summary>
        /// Gets or sets the public key.
        /// </summary>
        public string PublicKey { get; set; }

        /// <summary>
        /// Gets or sets the private key.
        /// </summary>
        public string PrivateKey { get; set; }

        /// <summary>
        /// Gets or sets the merchant id.
        /// </summary>
        public string MerchantId { get; set; }

        /// <summary>
        /// Gets or sets the merchant descriptor.
        /// </summary>
        public MerchantDescriptor MerchantDescriptor { get; set; }


        /// <summary>
        /// Gets or sets the default transaction option.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public TransactionOption DefaultTransactionOption { get; set; }        
    }
}