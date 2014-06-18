namespace Merchello.Core.Formatters
{
    /// <summary>
    /// Represents the default formatter
    /// </summary>
    public class DefaultFormatter : IFormatter
    {
        /// <summary>
        /// Performs the formatting operation on the value 
        /// </summary>
        /// <param name="value">
        /// The string to be formatted
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string Format(string value)
        {
            return value;
        }
    }
}