namespace Merchello.Plugin.Shipping.USPS.Models
{
    public class UspsProcessorSettings
    {
        private readonly UspsProcessorSettings _settings;

        public bool UseSandbox { get; set; }

        public UspsProcessorSettings()
        {
            UspsAdditionalHandlingCharge = "0.00";
        }

        public string UspsUsername { get; set; }
        public string UspsPassword { get; set; }
        public string UspsAdditionalHandlingCharge { get; set; }
    }
}
