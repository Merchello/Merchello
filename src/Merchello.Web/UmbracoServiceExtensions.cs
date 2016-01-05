namespace Merchello.Web
{
    using System.Globalization;

    using Umbraco.Core.Services;

    /// <summary>
    /// Extension methods for Umbraco services.
    /// </summary>
    public static class UmbracoServiceExtensions
    {
        #region ILocalizedTextService

        /// <summary>
        /// Returns a localized month name from a month number.
        /// </summary>
        /// <param name="textService">
        /// The text service.
        /// </param>
        /// <param name="culture">
        /// The culture.
        /// </param>
        /// <param name="month">
        /// The month.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetLocalizedMonthName(this ILocalizedTextService textService, CultureInfo culture,  int month)
        {
            string shortName;

            switch (month)
            {
                case 1:
                    shortName = "jan";
                    break;
                case 2:
                    shortName = "feb";
                    break;
                case 3:
                    shortName = "mar";
                    break;
                case 4:
                    shortName = "apr";
                    break;
                case 5:
                    shortName = "may";
                    break;
                case 6:
                    shortName = "jun";
                    break;
                case 7:
                    shortName = "jul";
                    break;
                case 8:
                    shortName = "aug";
                    break;
                case 9:
                    shortName = "sep";
                    break;
                case 10:
                    shortName = "oct";
                    break;
                case 11:
                    shortName = "nov";
                    break;
                case 12:
                    shortName = "dec";
                    break;
                default:
                    shortName = "jan";
                    break;

            }

            return textService.Localize(string.Format("merchelloGeneral/{0}", shortName), culture);
        }

        #endregion
    }
}