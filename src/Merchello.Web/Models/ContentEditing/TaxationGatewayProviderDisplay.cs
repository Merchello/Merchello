namespace Merchello.Web.Models.ContentEditing
{
    using System.Runtime.InteropServices;

    using Merchello.Core.Gateways;
    using Merchello.Core.Gateways.Taxation;

    /// <summary>
    /// The taxation gateway provider display.
    /// </summary>
    public class TaxationGatewayProviderDisplay : GatewayProviderDisplay
    {
        /// <summary>
        /// Gets or sets a value indicating whether taxation by product provider.
        /// </summary>
        public bool TaxationByProductProvider { get; set; }
    }

    /// <summary>
    /// The taxation gateway provider display extensions.
    /// </summary>
    internal static class TaxationGatewayProviderDisplayExtensions
    {
        /// <summary>
        /// The to taxation gateway provider display.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <returns>
        /// The <see cref="TaxationGatewayProviderDisplay"/>.
        /// </returns>
        internal static TaxationGatewayProviderDisplay ToTaxationGatewayProviderDisplay(this GatewayProviderBase provider)
        {
            var display = AutoMapper.Mapper.Map<TaxationGatewayProviderDisplay>(provider.GatewayProviderSettings);
            display.TaxationByProductProvider = provider is ITaxationByProductProvider;
            return display;
        }
    }
}