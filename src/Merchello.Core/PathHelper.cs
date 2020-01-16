namespace Merchello.Core
{
    using System;
    using System.IO;
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
            var url = string.Join(
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

            // Bit hacky, but is pretty quick
            return url.Replace("---", "-").Replace("--", "-");
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

            // Add PathHelper ones, i.e. charactors to remove
            foreach (var c in InvalidFileNameChars)
            {
                value = RemoveCharFromString(value, c);
            }

            // Add Umbraco ones
            // TODO - Need to remove this dependency to Umbraco for v4
            foreach (var n in UmbracoConfig.For.UmbracoSettings().RequestHandler.CharCollection)
            {
                if (string.IsNullOrEmpty(n.Char) == false)
                {
                    value = value.Replace(n.Char, n.Replacement);
                }
            }

            // Removed RemoveSpecialCharacters() as we are using the above Umbraco method
            // Removed SafeEncodeUrlSegments() too as sometimes creates weird urls
            return value.ToLowerInvariant().EnsureNotStartsOrEndsWith('/')
                                // Bit hacky, but is pretty quick
                                .Replace("----", "-")
                                .Replace("---", "-")
                                .Replace("--", "-")
                                .TrimStart('-')
                                .TrimEnd('-');
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

        /// <summary>
        /// Remove charactor from a string
        /// </summary>
        /// <param name="input"></param>
        /// <param name="charItem"></param>
        /// <returns></returns>
        internal static string RemoveCharFromString(string input, char charItem)
        {
            int indexOfChar = input.IndexOf(charItem);
            if (indexOfChar < 0)
            {
                return input;
            }
            return RemoveCharFromString(input.Remove(indexOfChar, 1), charItem);
        }

        // ok to be static here because it's not configureable in any way
        private static readonly char[] InvalidFileNameChars =
            Path.GetInvalidFileNameChars()
                .Union("!*'();:@&=$,£€/?%#₱₡¥[]~{}\"<>\\^`|".ToCharArray())
                .Distinct()
                .ToArray();

        internal static bool IsValidFileNameChar(char c)
        {
            return InvalidFileNameChars.Contains(c) == false;
        }
    }
}