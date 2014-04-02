using AuthorizeNet.APICore;
using Merchello.Core.Gateways.Payment;
using Merchello.Plugin.Payments.AuthorizeNet.Models;

namespace Merchello.Plugin.Payments.AuthorizeNet
{
    /// <summary>
    /// The Authorize.Net payment processor
    /// </summary>
    public class AuthorizeNetPaymentProcessor
    {
        private readonly AuthorizeNetProcessorSettings _settings;

        public AuthorizeNetPaymentProcessor(AuthorizeNetProcessorSettings settings)
        {
            _settings = settings;
        }



  
        /// <summary>
        /// Gets the Authorize.Net Url
        /// </summary>
        private string GetAuthorizeNetUrl()
        {
            return _settings.UseSandbox
                ? "https://test.authorize.net/gateway/transact.dll"
                : "https://secure.authorize.net/gateway/transact.dll";
        }



        /// <summary>
        /// The Authorize.Net API version
        /// </summary>
        public static string ApiVersion
        {
            get { return "3.1"; }
        }
    }
}