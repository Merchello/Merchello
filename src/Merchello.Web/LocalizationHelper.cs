namespace Merchello.Web
{
    using System.Globalization;
    using System.Threading;

    using umbraco;

    using Umbraco.Core;
    using Umbraco.Core.Models.Membership;

    /// <summary>
    /// The localization helper.
    /// </summary>
    public class LocalizationHelper
    {
        /// <summary>
        /// The umbraco default UI language.
        /// </summary>
        private static readonly string UmbracoDefaultUiLanguage = GlobalSettings.DefaultUILanguage;

        /// <summary>
        /// Gets the culture from user language.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="CultureInfo"/>.
        /// </returns>
        public static CultureInfo GetCultureFromUser(IUser user)
        {            
            return CultureInfo.GetCultureInfo(GetLanguage(user));
        }

        /// <summary>
        /// The get language.
        /// </summary>
        /// <param name="u">
        /// The <see cref="IUser"/>.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetLanguage(IUser u)
        {
            if (u != null)
            {
                return u.Language;
            }

            return GetLanguage(string.Empty);
        }

        /// <summary>
        /// The get language.
        /// </summary>
        /// <param name="userLanguage">
        /// The user language.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetLanguage(string userLanguage)
        {
            if (userLanguage.IsNullOrWhiteSpace() == false)
            {
                return userLanguage;
            }

            var language = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
            if (string.IsNullOrEmpty(language))
                language = UmbracoDefaultUiLanguage;
            return language;
        }
    }
}