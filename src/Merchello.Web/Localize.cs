namespace Merchello.Web
{
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
            return umbraco.ui.Text(area, key);
        }
    }
}