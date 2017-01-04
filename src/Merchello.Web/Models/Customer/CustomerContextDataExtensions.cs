namespace Merchello.Web.Models.Customer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Core;

    using Merchello.Core.Logging;

    using Newtonsoft.Json;

    using Umbraco.Core;
    using Umbraco.Core.Logging;

    /// <summary>
    /// The customer context data extensions.
    /// </summary>
    internal static class CustomerContextDataExtensions
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
            return EncryptionHelper.Encrypt(JsonConvert.SerializeObject(contextData));
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
                LogHelper.Debug(typeof(CustomerContextDataExtensions), "The CustomerContext cookie was null");
                return null;
            }

            try
            {
                return JsonConvert.DeserializeObject<CustomerContextData>(EncryptionHelper.Decrypt(contextCookie.Value));
            }
            catch (Exception ex)
            {
                MultiLogHelper.WarnWithException(typeof(CustomerContextDataExtensions), "Failed to decrypt custom context data", ex);
                return new CustomerContextData();
            }
        }

        /// <summary>
        /// Gets a value from the CustomerContextData.
        /// </summary>
        /// <param name="contextData">
        /// The context data.
        /// </param>
        /// <param name="alias">
        /// The alias.
        /// </param>
        /// <returns>
        /// The value as a string.
        /// </returns>
        /// <seealso cref="http://issues.merchello.com/youtrack/issue/M-1264"/>
        [Obsolete]
        public static string GetValue(this CustomerContextData contextData, string alias)
        {
            return contextData.Values.FirstOrDefault(x => x.Key == alias).Value;
        }

        /// <summary>
        /// Adds a value to the Context Data
        /// </summary>
        /// <param name="contextData">
        /// The context data.
        /// </param>
        /// <param name="alias">
        /// The alias.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <see cref="http://issues.merchello.com/youtrack/issue/M-1264"/>
        [Obsolete]
        public static void SetValue(this CustomerContextData contextData, string alias, string value)
        {
            Ensure.ParameterNotNullOrEmpty(alias, "alias");
            Ensure.ParameterNotNullOrEmpty(value, "value");

            contextData.Values.Remove(contextData.Values.FirstOrDefault(x => x.Key == alias));
            contextData.Values.Add(new KeyValuePair<string, string>(alias, value));
        }
    }
}