namespace Merchello.Examine.Models
{
    /// <summary>
    /// Represents a search term.
    /// </summary>
    internal class SearchTerm
    {
        /// <summary>
        /// Gets or sets the term.
        /// </summary>
        public string Term { get; set; }

        /// <summary>
        /// Gets or sets the search term type.
        /// </summary>
        public SearchTermType SearchTermType { get; set; }
    }
}