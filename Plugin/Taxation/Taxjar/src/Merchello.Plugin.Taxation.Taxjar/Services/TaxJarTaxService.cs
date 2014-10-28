namespace Merchello.Plugin.Taxation.Taxjar.Services
{
    using System.IO;
    using System.Net;
    using Models;
    using Newtonsoft.Json.Linq;

    internal class TaxJarTaxService : ITaxJarTaxService
    {
        #region Fields

        /// <summary>
        /// The API version.
        /// </summary>
        private readonly string _apiVersion;

        /// <summary>
        /// The api token used in API authentication.
        /// </summary>
        private readonly string _apiToken;

        #endregion
        public TaxJarTaxService(TaxJarProviderSettings settings)
        {
            _apiToken = settings.ApiToken;
            _apiVersion = settings.ApiVersion;
        }

        public TaxResult GetTax(TaxRequest request)
        {
            string url = "https://api.taxjar.com/sales_tax?amount=" + request.Amount
                + "&shipping=" + request.Shipping
                + "&to_country=" + request.ToCountry
                + "&to_state=" + request.ToState
                + "&to_city=" + request.ToCity
                + "&to_zip=" + request.ToZip;
            JObject response = MakeTaxJarApiRequest(url);

            var result = new TaxResult();
            try
            {
                result.TotalTax = (decimal) response["amount_to_collect"];
                result.Rate = (decimal) response["rate"];
                result.HasNexus = (bool) response["has_nexus"];
                if (response["freight_taxable"].HasValues)
                    result.FreightTaxable = (bool) response["freight_taxable"];
                if (response["tax_source"].HasValues)
                    result.TaxSource = (string) response["tax_source"];
                result.Success = true;
            }
            catch
            {
                result.Success = false;
            }

            return result;
        }

        private JObject MakeTaxJarApiRequest(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Headers.Add("Authorization", "Token token=\"" + this._apiToken + "\"");
            request.UserAgent = "Merchello (https://github.com/Merchello/Merchello)";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string apiResponse = null;
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                apiResponse = reader.ReadToEnd();
            }
            return JObject.Parse(apiResponse);
        }
    }
}
