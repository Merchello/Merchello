namespace Merchello.Plugin.Taxation.Avalara.Models.Tax
{
    /// <summary>
    /// Defines a reason to override AvaTax computed tax.
    /// </summary>
    /// <remarks>
    /// This is generally used for partial returns
    /// </remarks>
    public class TaxOverride
    {
        /// <summary>
        /// Gets or sets the tax override type.
        /// </summary>
        public string TaxOverrideType { get; set; }

        /// <summary>
        /// Gets or sets the tax amount.
        /// </summary>
        public string TaxAmount { get; set; }

        /// <summary>
        /// Gets or sets the tax date.
        /// </summary>
        public string TaxDate { get; set; }

        /// <summary>
        /// Gets or sets the reason.
        /// </summary>
        public string Reason { get; set; } 
    }
}