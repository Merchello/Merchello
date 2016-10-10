namespace Merchello.Web.Models
{
    /// <summary>
    /// Represents a product filter that includes pre queried counts.
    /// </summary>
    public interface IPrimedProductFilter : IProductFilter
    {
        /// <summary>
        /// Gets or sets the count of items in the filter would matches in the current context.
        /// </summary>
        int Count { get; set; }
    }
}