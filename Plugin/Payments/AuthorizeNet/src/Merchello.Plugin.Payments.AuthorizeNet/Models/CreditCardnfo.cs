using System;
using System.Linq;
using Merchello.Plugin.Payments.AuthorizeNet.Provider;

namespace Merchello.Plugin.Payments.AuthorizeNet.Models
{
    public class CreditCardnfo
    {
        private string _creditCardType;

        /// <summary>
        /// The type of the credit card.  
        /// </summary>
        public string CreditCardType
        {
            get
            {
                return _creditCardType;
            }
            set
            {
                _creditCardType = AuthorizeNetPaymentGatewayProvider.AvailableResources.Any(x => x.ServiceCode == value)
                    ? value
                    : string.Empty;
            }
        } 

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
    }
}