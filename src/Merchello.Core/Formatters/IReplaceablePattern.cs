namespace Merchello.Core.Formatters
{
    public interface IReplaceablePattern
    {
        /// <summary>
        /// The unique alias
        /// </summary>
        string Alias { get; }

        /// <summary>
        /// The pattern to be searched
        /// </summary>
        string Pattern { get; set; }

        /// <summary>
        /// The replacement for the pattern
        /// </summary>
        string Replacement { get; set; }
    }
}