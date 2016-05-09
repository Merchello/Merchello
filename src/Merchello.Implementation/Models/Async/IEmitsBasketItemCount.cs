namespace Merchello.Implementation.Models.Async
{
    /// <summary>
    /// Indicates that basket item count could be required for client side events.
    /// </summary>
    public interface IEmitsBasketItemCount
    {
        /// <summary>
        /// Gets or sets the current basket item count.
        /// </summary>
        int BasketItemCount { get; set; }
    }
}