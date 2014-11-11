namespace Merchello.Plugin.Payments.Chase.Models
{
    public class ChaseProcessorSettings
    {
        public bool UseSandbox { get; set; }

        public ChaseProcessorSettings()
        {
            // set the defaults
            DelimitedData = true;
            DelimitedChar = "|";
            Method = "CC";
        }

        public string MerchantId { get; set; }
        public string Bin { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }                                
        public string Method { get; set; }
        public bool DelimitedData { get; set; }
        public string DelimitedChar { get; set; }
        public string EncapChar { get; set; }
        public bool RelayResponse { get; set; }

        public string ApiVersion
        {
            get { return ChasePaymentProcessor.ApiVersion; }
        }
    }
}