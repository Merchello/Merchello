namespace Merchello.Plugin.Taxation.Taxjar.Models
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization;

    using Merchello.Core.Models;

    /// <summary>
    /// The TaxJar provider settings.
    /// Stores API authentication credentials and implementation settings
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public class TaxJarProviderSettings
    {
        /// <summary>
        /// Gets the extended data key.
        /// </summary>
        public static string ExtendedDataKey
        {
            get
            {
                return "merchTaxJarProviderSettings";
            }
        }

        /// <summary>
        /// Gets or sets the TaxJar API key.
        /// </summary>
        /// <remarks>
        /// Get a TaxJar API token by signing up at http://www.taxjar.com/api_sign_up
        /// </remarks>
        [DataMember]
        public string ApiToken { get; set; }


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