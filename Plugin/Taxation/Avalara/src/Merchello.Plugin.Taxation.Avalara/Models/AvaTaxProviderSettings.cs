using Merchello.Plugin.Taxation.Avalara.Models.Address;

namespace Merchello.Plugin.Taxation.Avalara.Models
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization;

    using Merchello.Core.Models;

    /// <summary>
    /// The Avalara AvaTax provider settings.
    /// Stores API authentication credentials and implementation settings
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public class AvaTaxProviderSettings
    {
        /// <summary>
        /// Gets the extended data key.
        /// </summary>
        public static string ExtendedDataKey
        {
            get
            {
                return "merchAvaTaxProviderSettings";
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to use a demo or sandbox mode.
        /// </summary>
        [DataMember]
        public bool UseSandBox { get; set; }

        /// <summary>
        /// Gets or sets the Avalara account number.
        /// </summary>
        /// <remarks>
        /// Available in you Avalara admin console
        /// </remarks>
        [DataMember]
        public string AccountNumber { get; set; }

        /// <summary>
        /// Gets or sets the Avalara license key.
        /// </summary>
        /// <remarks>
        /// This is sent to all admins email addresses when requested through
        /// the admin console
        /// </remarks>
        [DataMember]
        public string LicenseKey { get; set; }

        /// <summary>
        /// Gets the service url depending on use sandbox value.
        /// </summary>
        /// <remarks>
        /// Development : https://development.avalara.net/
        /// Production : https://avatax.avalara.net/ 
        /// </remarks>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here."),DataMember]
        public string ServiceUrl 
        {
            get
            {
                return UseSandBox ? "https://development.avalara.net/" : "https://avatax.avalara.net/";
            }
        }

        /// <summary>
        /// Gets or sets the company code.
        /// </summary>
        /// <remarks>
        /// Value can be found in the admin console
        /// </remarks>
        [DataMember]
        public string CompanyCode { get; set; }

        /// <summary>
        /// Gets or sets the default store address.
        /// </summary>
        [DataMember]
        public TaxAddress DefaultStoreAddress { get; set; }

        /// <summary>
        /// Gets the API version.
        /// </summary>
        [DataMember]
        public string ApiVersion 
        { 
            get { return "1.0"; } 
        }
    }
}