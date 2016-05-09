namespace Merchello.Implementation.Models.Async
{
    /// <summary>
    /// A response object to for an AJAX AddItem operation.
    /// </summary>
    public class AddItemAsyncResponse : AsyncResponse, IEmitsBasketItemCount
    {
        /// <summary>
        /// Gets or sets the basket item count.
        /// </summary>
        public int BasketItemCount { get; set; }
    }
}