namespace Merchello.Plugin.Taxation.Avalara.Models.Tax
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// A base class for a tax requests.
    /// </summary>
    public abstract class TaxRequestBase
    {
        /// <summary>
        /// Gets or sets the doc type.  We use statement type to avoid confusion with the notion of an Umbraco Doc Type
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public StatementType DocType { get; set; }

        /// <summary>
        /// Gets or sets the company code.
        /// </summary>
        public string CompanyCode { get; set; }

        /// <summary>
        /// Gets or sets the doc code.  
        /// </summary>
        public string DocCode { get; set; } 
    }
}