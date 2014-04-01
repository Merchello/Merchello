namespace Merchello.Plugin.Payments.AuthorizeNet.Models
{
    public class AuthorizeNetConfiguration
    {
        public bool UseSandbox { get; set; } 
        public int TransactionModeId { get; set; }
        public string TransactionKey { get; set; }
        public string LoginId { get; set; }
        
    }
}