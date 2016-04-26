namespace Merchello.Providers.Payment.PayPal.Models
{
    /// <summary>
    /// Extension methods for <see cref="PayPalProviderSettings"/>.
    /// </summary>
    public static class PayPalProviderSettingsExtensions
    {
        /// <summary>
        /// Gets the SDK configuration.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        /// <returns>
        /// The <see cref="SdkConfig"/>.
        /// </returns>
        public static SdkConfig GetSdkConfig(this PayPalProviderSettings settings)
        {
            return new SdkConfig
                       {
                            { "mode", settings.Mode.ToString().ToLowerInvariant() } 
                       };
        }
    }
}