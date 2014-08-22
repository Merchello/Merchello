namespace Merchello.Web
{
    using System;
    using System.Linq;
    using System.Xml;
    using System.Xml.Linq;

    using Merchello.Core;
    using Merchello.Core.Cache;
    using Merchello.Core.Configuration;

    using umbraco;

    /// <summary>
    /// The localization helper.
    /// </summary>
    internal class Localize
    {
        /// <summary>
        /// The text.
        /// </summary>
        /// <param name="area">
        /// The area.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string Text(string area, string key)
        {
            return Text(MerchelloContext.Current, area, key);
        }

        /// <summary>
        /// The text.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="area">
        /// The area.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        internal static string Text(IMerchelloContext merchelloContext, string area, string key)
        {
            Mandate.ParameterNotNull(merchelloContext, "MerchelloContext");

            if (string.IsNullOrEmpty(area) || string.IsNullOrEmpty(key)) return string.Empty;

            var lang = "en";
            try
            {
                lang = MerchelloConfiguration.Current.Section.LogLocalization;
            }
            catch (Exception)
            {
                lang = "en";
            }

            var cacheKey = CacheKeys.GetLocalizationCacheKey(lang);

            var xdoc = XDocument.Parse((string)merchelloContext.Cache.RuntimeCache.GetCacheItem(cacheKey, () => ui.getLanguageFile(lang).InnerXml), LoadOptions.None);

            var xArea = xdoc.Descendants("area").FirstOrDefault(x => x.Attribute("alias").Value == area);

            if (xArea == null) return string.Empty;

            var xKey = xArea.Descendants("key").FirstOrDefault(x => x.Attribute("alias").Value == key);

            return xKey == null ? string.Empty : xKey.Value;
        }
    }
}