namespace Merchello.Core.Formatters
{
    public interface IReplaceablePattern
    {
        /// <summary>
        /// Gets the unique alias
        /// </summary>
        string Alias { get; }

        /// <summary>
        /// Gets or sets the pattern to be searched
        /// </summary>
        string Pattern { get; set; }

        /// <summary>
        /// Gets or sets the replacement for the pattern
        /// </summary>
        string Replacement { get; set; }
    }
}