namespace Merchello.Plugin.Taxation.Avalara.Services
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;

    using Merchello.Plugin.Taxation.Avalara.Models;
    using Merchello.Plugin.Taxation.Avalara.Models.Address;

    using Newtonsoft.Json;

    /// <summary>
    /// Represents the AvaTaxService.
    /// </summary>
    internal class AvaTaxService : IAvaTaxService
    {
        #region Fields

        /// <summary>
        /// The API service url.
        /// </summary>
        private readonly string _serviceUrl;

        /// <summary>
        /// The API version.
        /// </summary>
        private readonly string _apiVersion;

        /// <summary>
        /// The account number used in API authentication.
        /// </summary>
        private readonly string _accountNumber;

        /// <summary>
        /// The license key used in API authentication.
        /// </summary>
        private readonly string _licenseKey;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="AvaTaxService"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public AvaTaxService(AvaTaxProviderSettings settings)
        {
            _serviceUrl = settings.ServiceUrl.EndsWith("/") ? settings.ServiceUrl : string.Format("{0}/", settings.ServiceUrl);
            _accountNumber = settings.AccountNumber;
            _apiVersion = settings.ApiVersion;
            _licenseKey = settings.LicenseKey;
        }

        public AddressValidationResult ValidateTaxAddress(IValidatableAddress address)
        {
            var requestUrl = string.Format("{0}?{1}", GetApiUrl("address", "validate"), address.AsApiQueryString());

            var json = GetResponse(requestUrl);

            return JsonConvert.DeserializeObject<AddressValidationResult>(json);
        }

        /// <summary>
        /// Responsible for constructing a valid API request url.
        /// </summary>
        /// <param name="operation">Tax or Address</param>
        /// <param name="methodName">
        /// The method name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        internal string GetApiUrl(string operation, string methodName)
        {
            return string.Format("{0}{1}/{2}/{3}", _serviceUrl, _apiVersion, operation.ToLower(), methodName);
        }

        /// <summary>
        /// Gets a <see cref="HttpWebResponse"/> from the API.
        /// </summary>
        /// <param name="requestUrl">
        /// The request url.
        /// </param>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="requestMethod">The request method</param>
        /// <returns>
        /// The <see cref="HttpWebResponse"/>.
        /// </returns>
        internal string GetResponse(string requestUrl, string data = "", RequestMethod requestMethod = RequestMethod.HttpGet)
        {
            var address = new Uri(requestUrl);

            var request = WebRequest.Create(address) as HttpWebRequest;

            //// Add Authorization header
            var pair = Encoding.ASCII.GetBytes(string.Format("{0}:{1}", _accountNumber, _licenseKey));
            var basic = string.Format("Basic {0}", Convert.ToBase64String(pair));
            request.Headers.Add(HttpRequestHeader.Authorization, basic);

            if (requestMethod == RequestMethod.HttpPost)
            {
                request.Method = "POST";
                request.ContentType = "application/json";
                request.ContentLength = data.Length;
                var s = request.GetRequestStream();
                s.Write(Encoding.ASCII.GetBytes(data), 0, data.Length);
            }
            else
            {
                request.Method = "GET";
            }

            var response = request.GetResponse();

            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                return sr.ReadToEnd();
            }
        }

    }
}