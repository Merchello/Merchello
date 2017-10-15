namespace Merchello.Core
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Web;

    using Umbraco.Core;
    using Umbraco.Core.Configuration;
     
    /// <summary>
    /// The path helper.
    /// </summary>
    internal static class PathHelper
    {
        /// <summary>
        /// Encodes url segments.
        /// </summary>
        /// <param name="urlPath">
        /// The url path.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        /// <seealso cref="https://github.com/Shandem/Articulate/blob/master/Articulate/StringExtensions.cs"/>
        public static string SafeEncodeUrlSegments(this string urlPath)
        {
            return string.Join(
                "/",
                urlPath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x =>
                        {
                            var urlEncode = HttpUtility.UrlEncode(x);
                            return urlEncode != null ? urlEncode.Replace("+", "-") : null;
                        })
                    .WhereNotNull()

                    //// we are not supporting dots in our URLs it's just too difficult to
                    //// support across the board with all the different config options
                    .Select(x => x.Replace('.', '-')));
        }

        /// <summary>
        /// The convert to slug.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        internal static string ConvertToSlug(string value)
        {
            foreach (var n in UmbracoConfig.For.UmbracoSettings().RequestHandler.CharCollection)
            {
                if (n.Char != "")
                    value = value.Replace(n.Char, n.Replacement);
            }
            return RemoveSpecialCharacters(value).SafeEncodeUrlSegments().ToLowerInvariant().EnsureNotStartsOrEndsWith('/');
        }

        /// <summary>
        /// The get searchable url.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        internal static string GetSearchableUrl(string value)
        {
            return string.IsNullOrEmpty(value) ?
                string.Empty :
                value.EnsureForwardSlashes().EnsureEndsWith('/').SafeEncodeUrlSegments();
        }


        /// <summary>
        /// The remove special characters.
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        internal static string RemoveSpecialCharacters(string input)
        {
            var regex = new Regex("(?:[^a-z0-9 ]|(?<=['\"])s)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
            return regex.Replace(input, string.Empty);
        }
    }
}
