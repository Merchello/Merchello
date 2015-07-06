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

    internal static class TaxationGatewayProviderDisplayExtensions
    {
        internal static TaxationGatewayProviderDisplay ToTaxationGatewayProviderDisplay(this GatewayProviderBase provider)
        {
            var display = AutoMapper.Mapper.Map<TaxationGatewayProviderDisplay>(provider.GatewayProviderSettings);
            display.TaxationByProductProvider = provider is ITaxationByProductProvider;
            return display;
        }
    }
}