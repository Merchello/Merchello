namespace Merchello.Plugin.Payments.AuthorizeNet.Models
{
    public class AuthorizeNetProcessorSettings
    {
        public bool UseSandbox { get; set; }

        public AuthorizeNetProcessorSettings()
        {
            // set the defaults
            DelimitedData = true;
            DelimitedChar = "|";
            Method = "CC";
        }

        public string LoginId { get; set; }
        public string TransactionKey { get; set; }                                
        public string Method { get; set; }
        public bool DelimitedData { get; set; }
        public string DelimitedChar { get; set; }
        public string EncapChar { get; set; }
        public bool RelayResponse { get; set; }

        public string ApiVersion
        {
            get { return AuthorizeNetPaymentProcessor.ApiVersion; }
        }
    }
}