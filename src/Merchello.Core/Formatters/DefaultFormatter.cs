namespace Merchello.Core.Formatters
{
    /// <summary>
    /// Represents the default formatter
    /// </summary>
    public class DefaultFormatter : IFormatter
    {
        public string Format(string value)
        {
            return value;
        }
    }
}