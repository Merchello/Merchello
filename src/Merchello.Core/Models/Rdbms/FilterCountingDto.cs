namespace Merchello.Core.Models.Rdbms
{
    /// <summary>
    /// A DTO for counting filter results.
    /// </summary>
    public class FilterCountingDto
    {
        /// <summary>
        /// Gets or sets the hash of the filter context (collection keys)
        /// </summary>
        public int Hash { get; set; }

        /// <summary>
        /// Gets or sets the count or products for the context.
        /// </summary>
        public int Count { get; set; }
    }
}