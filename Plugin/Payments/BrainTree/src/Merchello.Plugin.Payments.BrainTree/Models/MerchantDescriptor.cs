namespace Merchello.Plugin.Payments.Braintree.Models
{
    /// <summary>
    /// The merchant descriptor.
    /// </summary>
    public class MerchantDescriptor
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the url.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the phone.
        /// </summary>
        public string Phone { get; set; }
    }
}