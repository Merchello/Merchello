namespace Merchello.Web.Models.ContentEditing
{
    using System.Diagnostics.CodeAnalysis;

    using Merchello.Core.Models;

    /// <summary>
    /// The currency display.
    /// </summary>
    public class CurrencyDisplay
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the currency code.
        /// </summary>
        public string CurrencyCode { get; set; }

        /// <summary>
        /// Gets or sets the currency symbol.
        /// </summary>
        public string Symbol { get; set; }
    }

    /// <summary>
    /// The currency display extensions.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    internal static class CurrencyDisplayExtensions
    {
        /// <summary>
        /// Maps <see cref="ICurrency"/> to <see cref="CurrencyDisplay"/>.
        /// </summary>
        /// <param name="currency">
        /// The currency.
        /// </param>
        /// <returns>
        /// The <see cref="CurrencyDisplay"/>.
        /// </returns>
        public static CurrencyDisplay ToCurrencyDisplay(this ICurrency currency)
        {
            return AutoMapper.Mapper.Map<CurrencyDisplay>(currency);
        }
    }
}