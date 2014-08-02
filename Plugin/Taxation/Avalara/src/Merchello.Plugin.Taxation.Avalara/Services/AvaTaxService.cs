namespace Merchello.Plugin.Taxation.Avalara.Services
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;
    using Models;
    using Models.Address;
    using Models.Tax;
    using Newtonsoft.Json;
    using Umbraco.Core.Logging;

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

        /// <summary>
        /// Performs address validation on an address
        /// </summary>
        /// <param name="address">
        /// The address.
        /// </param>
        /// <returns>
        /// The <see cref="AddressValidationResult"/>.
        /// </returns>
        public AddressValidationResult ValidateTaxAddress(IValidatableAddress address)
        {
            var requestUrl = string.Format("{0}?{1}", GetApiUrl("address", "validate"), address.AsApiQueryString());

            var json = GetResponse(requestUrl);

            return JsonConvert.DeserializeObject<AddressValidationResult>(json);
        }


        /// <summary>
        /// Gets the <see cref="TaxResult"/> from the AvaTax API based on request parameters.
        /// </summary>
        /// <param name="request">
        /// The <see cref="TaxRequest"/>.
        /// </param>
        /// <returns>
        /// The <see cref="TaxResult"/>.
        /// </returns>
        public TaxResult GetTax(TaxRequest request)
        {
            var requestUrl = this.GetApiUrl("tax", "get");

            request.CustomerCode = _accountNumber;

            var requestData = JsonConvert.SerializeObject(
                request, 
                Formatting.None, 
                new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore
                });

            var json = this.GetResponse(requestUrl, requestData, RequestMethod.HttpPost);

            return JsonConvert.DeserializeObject<TaxResult>(json);
        }

        /// <summary>
        /// Estimates the tax on a sales amount based on geo data
        /// </summary>
        /// <param name="latitude">
        /// The latitude.
        /// </param>
        /// <param name="longitude">
        /// The longitude.
        /// </param>
        /// <param name="saleAmount">
        /// The sale amount.
        /// </param>
        /// <returns>
        /// The <see cref="GeoTaxResult"/>.
        /// </returns>
        public GeoTaxResult EstimateTax(decimal latitude, decimal longitude, decimal saleAmount)
        {            
            var methodName = string.Format("{0},{1}/get?saleamount={2}", latitude, longitude, saleAmount);

            var requestUrl = GetApiUrl("tax", methodName);

            var json = GetResponse(requestUrl);

            return JsonConvert.DeserializeObject<GeoTaxResult>(json);
        }

        /// <summary>
        /// Queries the API to solicit a response based on a predefined and know latitude and longitude.
        /// A success result indicates a valid "ping"
        /// </summary>
        /// <returns>
        /// The <see cref="GeoTaxResult"/>.
        /// </returns>
        public GeoTaxResult Ping()
        {
            return this.EstimateTax(41.220691M, -111.972612M, 911);
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
            try
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
            catch (Exception ex)
            {
                LogHelper.Error<AvaTaxService>("AvaTax API Failure", ex);
                var result = new TaxResult() 
                { 
                    ResultCode = SeverityLevel.Exception, 
                    Messages = new[] 
                    { 
                        new ApiResponseMessage() { Details = ex.Message, Source = typeof(AvaTaxService).Name, RefersTo = "GetTax", Severity = SeverityLevel.Exception } 
                    }
                };

                return JsonConvert.SerializeObject(result);
            }
        }        
    }
}