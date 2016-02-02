namespace Merchello.Web.Workflow
{
    /// <summary>
    /// The pair of baskets involved in a basket conversion.
    /// </summary>
    public class BasketConversionPair
    {
        /// <summary>
        /// Gets or sets the anonymous basket.
        /// </summary>
        public IBasket AnonymousBasket { get; set; }

        /// <summary>
        /// Gets or sets the customer basket.
        /// </summary>
        public IBasket CustomerBasket { get; set; }
    }
}
