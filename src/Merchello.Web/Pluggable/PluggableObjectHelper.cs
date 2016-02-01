namespace Merchello.Web.Pluggable
{
    using System;

    using Merchello.Core;
    using Merchello.Core.Configuration;

    using Umbraco.Core.Logging;

    /// <summary>
    /// The pluggable object helper.
    /// </summary>
    public class PluggableObjectHelper
    {
        /// <summary>
        /// Attempts to instantiate an instance of a configured "pluggable" object
        /// </summary>
        /// <param name="configurationAlias">
        /// The configuration alias.
        /// </param>
        /// <param name="param1">
        /// The first constructor parameter value.
        /// </param>
        /// <typeparam name="T">
        /// The type to return
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public static T GetInstance<T>(string configurationAlias, object param1) where T : class
        {
            return GetInstance<T>(configurationAlias, new[] { param1 });
        }

        /// <summary>
        /// Attempts to instantiate an instance of a configured "pluggable" object
        /// </summary>
        /// <param name="configurationAlias">
        /// The configuration alias.
        /// </param>
        /// <param name="param1">
        /// The first constructor parameter value.
        /// </param>
        /// <param name="param2">
        /// The second constructor parameter value.
        /// </param>
        /// <typeparam name="T">
        /// The type to return
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public static T GetInstance<T>(string configurationAlias, object param1, object param2) where T : class
        {
            return GetInstance<T>(configurationAlias, new[] { param1, param2 });
        }

        /// <summary>
        /// Attempts to instantiate an instance of a configured "pluggable" object
        /// </summary>
        /// <param name="configurationAlias">
        /// The configuration alias.
        /// </param>
        /// <param name="ctrArgValues">
        /// The collection of constructor argument values
        /// </param>
        /// <typeparam name="T">
        /// The type to return
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public static T GetInstance<T>(string configurationAlias, object[] ctrArgValues) where T : class
        {
            var contextType = MerchelloConfiguration.Current.GetPluggableObjectElement(configurationAlias).Type;

            // We have to have this key to instantiate the customer context
            if (string.IsNullOrEmpty(contextType))
            {
                var nullReference = new NullReferenceException("Reference to the pluggable/object with key '" + configurationAlias + "' was not found in the merchello.config file.");
                LogHelper.Error(typeof(PluggableObjectHelper), "Configuration missing", nullReference);
                throw nullReference;
            }
            var attempt = ActivatorHelper.CreateInstance<T>(contextType, ctrArgValues);

            if (attempt.Success)
            {
                return attempt.Result;
            }

            LogHelper.Error(typeof(PluggableObjectHelper), "Failed to instantiate configured type", attempt.Exception);
            throw attempt.Exception;
        }
    }
}