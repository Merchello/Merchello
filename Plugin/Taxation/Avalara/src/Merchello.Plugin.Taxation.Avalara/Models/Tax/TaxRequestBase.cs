namespace Merchello.Plugin.Taxation.Avalara.Models.Tax
{
    using System.Collections;

    /// <summary>
    /// A base class for a tax requests.
    /// </summary>
    public abstract class TaxRequestBase
    {
        /// <summary>
        /// Gets or sets the doc type.  We use statement type to avoid confusion with the notion of an Umbraco Doc Type
        /// </summary>
        public StatementType DocType { get; set; }

        public string CompanyCode { get; set; }

        public string DocCode { get; set; } 
    }
}