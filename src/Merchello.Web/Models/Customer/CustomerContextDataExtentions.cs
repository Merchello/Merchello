namespace Merchello.Web.Models.Customer
{
    using System.Web;
    using Newtonsoft.Json;
    using Umbraco.Core.Logging;

    /// <summary>
    /// The customer context data extentions.
    /// </summary>
    internal static class CustomerContextDataExtentions
    {
        /// <summary>
        /// Serializes the <see cref="CustomerContextData"/> to JSON
        /// </summary>
        /// <param name="contextData">
        /// The context data.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string ToJson(this CustomerContextData contextData)
        {
            return JsonConvert.SerializeObject(contextData);
        }

        /// <summary>
        /// Deserializes the <see cref="CustomerContextData"/> from JSON
        /// </summary>
        /// <param name="contextCookie">
        /// The context cookie.
        /// </param>
        /// <returns>
        /// The <see cref="CustomerContextData"/>.
        /// </returns>
        public static CustomerContextData ToCustomerContextData(this HttpCookie contextCookie)
        {
            if (contextCookie == null || string.IsNullOrEmpty(contextCookie.Value))
            {
                LogHelper.Debug<CustomerContext>("The CustomerContext cookie was null");
                return null;
            }

            return JsonConvert.DeserializeObject<CustomerContextData>(contextCookie.Value);
        }

    }
}