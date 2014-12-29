namespace Merchello.Plugin.Payments.Chase.Models
{
    public class CreditCardFormData
    {        
        /// <summary>
        /// The type of the credit card.  
        /// </summary>
        public string CreditCardType { get; set; }

        /// <summary>
        /// The card holders name
        /// </summary>
        public string CardholderName { get; set; }

        /// <summary>
        /// The credit card number
        /// </summary>
        public string CardNumber { get; set; }

        /// <summary>
        /// The expiration month - format MM
        /// </summary>
        public string ExpireMonth { get; set; }

        /// <summary>
        /// The expiration year = format yy
        /// </summary>
        public string ExpireYear { get; set; }

        /// <summary>
        /// The credit card code or CVV
        /// </summary>
        public string CardCode { get; set; }

        /// <summary>
        /// The customer's IP Address
        /// </summary>
        public string CustomerIp { get; set; }

        /// <summary>
        /// The CAVV or AAV values used for authentication verification in Visa or Master Card
        /// </summary>
        public string AuthenticationVerification { get; set; }

        /// <summary>
        /// The AuthenticationECIInd for authentication verification in Visa or Master Card
        /// </summary>
        public string AuthenticationVerificationEci { get; set; }
    }
}