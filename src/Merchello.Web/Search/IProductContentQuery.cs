namespace Merchello.Web.Search
{
    using Merchello.Web.Models.VirtualContent;

    /// <summary>
    /// Represents a Product Content Query.
    /// </summary>
    public interface IProductContentQuery : ICmsContentQuery<IProductContent>
    {
        /// <summary>
        /// Gets the minimum price.
        /// </summary>
        decimal MinPrice { get; }

        /// <summary>
        /// Gets the maximum price.
        /// </summary>
        decimal MaxPrice { get; }

        /// <summary>
        /// Gets a value indicating whether the query is constrained by a price range.
        /// </summary>
        bool HasPriceRange { get; }

        /// <summary>
        /// Sets the price range constraints.
        /// </summary>
        /// <param name="min">
        /// The min.
        /// </param>
        /// <param name="max">
        /// The max.
        /// </param>
        void SetPriceRange(decimal min, decimal max);

        /// <summary>
        /// Clears the price range constraints.
        /// </summary>
        void ClearPriceRange();
    }
}