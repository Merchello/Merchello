namespace Merchello.Plugin.Payments.Epay
{
    using System.Runtime.Serialization;

    using EPay.API;

    using Merchello.Plugin.Payments.Epay.Models;

    /// <summary>
    /// Mapping extensions.
    /// </summary>
    internal static class MappingExtensions
    {
        public static authentication ToAuthentication(this EpayProcessorSettings settings)
        {
            return new authentication()
                       {
                           merchantnumber = settings.MerchantNumber,
                           password = settings.Password,
                       };
        }

        public static authentication ToAuthentication(this EpayProcessorSettings settings, ExtensionDataObject extensionData)
        {
            var authentication = settings.ToAuthentication();
            authentication.ExtensionData = extensionData;

            return authentication;
        }
    }
}