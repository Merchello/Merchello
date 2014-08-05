namespace Merchello.Plugin.Taxation.Avalara.Models.Tax
{
    /// <summary>
    /// Represents the details of a tax computation.
    /// </summary>
    public class TaxDetail
    {
        /// <summary>
        /// Gets or sets the tax rate.
        /// </summary>
        public decimal Rate { get; set; }

        /// <summary>
        /// Gets or sets the total tax amount.
        /// </summary>
        public decimal Tax { get; set; }

        /// <summary>
        /// Gets or sets the taxable amount.
        /// </summary>
        public decimal Taxable { get; set; }

        /// <summary>
        /// Gets or sets the country associated with the tax.
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets the region associate with the tax.
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// Gets or sets the jurisdiction type associate with the tax.
        /// </summary>
        public string JurisType { get; set; }

        /// <summary>
        /// Gets or sets the jurisdiction name.
        /// </summary>
        public string JurisName { get; set; }

        /// <summary>
        /// Gets or sets the tax name.
        /// </summary>
        public string TaxName { get; set; } 
    }
}