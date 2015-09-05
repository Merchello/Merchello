namespace Merchello.Core
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Web;

    using Umbraco.Core;

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
            return RemoveSpecialCharacters(value).SafeEncodeUrlSegments().ToLowerInvariant();
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
        /// Replaces \ with / in a path.
        /// </summary>
        /// <param name="value">
        /// The value to replace backslashes.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        internal static string EnsureForwardSlashes(this string value)
        {
            return value.Replace("\\", "/");
        }

        /// <summary>
        /// Ensures a string both starts and ends with a character.
        /// </summary>
        /// <param name="input">
        /// The input string.
        /// </param>
        /// <param name="value">
        /// The char value to assert
        /// </param>
        /// <returns>
        /// The asserted string.
        /// </returns>
        internal static string EnsureStartsAndEndsWith(this string input, char value)
        {
            return input.EnsureStartsWith(value).EnsureEndsWith(value);
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