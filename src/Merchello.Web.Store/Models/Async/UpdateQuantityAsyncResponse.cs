namespace Merchello.Web.Store.Models.Async
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using Merchello.Core;
    using Merchello.Core.Models;
    using Merchello.Web.Models.Ui.Async;

    /// <summary>
    /// A response object to for an AJAX UpdateQuantity operation.
    /// </summary>
    internal class UpdateQuantityAsyncResponse : AsyncResponse, IEmitsBasketItemCount
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateQuantityAsyncResponse"/> class.
        /// </summary>
        public UpdateQuantityAsyncResponse()
        {
            this.UpdatedItems = new List<UpdateQuantityResponseItem>();
        }

        /// <summary>
        /// Gets or sets the formatted total.
        /// </summary>
        public string FormattedTotal { get; set; }

        /// <summary>
        /// Gets or sets the updated items.
        /// </summary>
        public List<UpdateQuantityResponseItem> UpdatedItems { get; set; }

        /// <summary>
        /// Gets or sets the item count.
        /// </summary>
        public int ItemCount { get; set; }
    }

    /// <summary>
    /// Extension methods for <see cref="UpdateQuantityAsyncResponse"/>.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1204:StaticElementsMustAppearBeforeInstanceElements", Justification = "Reviewed. Suppression is OK here.")]
    internal static class UpdateQuantityAsyncResponseExtensions
    {
        /// <summary>
        /// Maps a collection of <see cref="ILineItem"/> to a collection of <see cref="UpdateQuantityResponseItem"/>.
        /// </summary>
        /// <param name="resp">
        /// The resp.
        /// </param>
        /// <param name="items">
        /// The items.
        /// </param>
        public static void AddUpdatedItems(this UpdateQuantityAsyncResponse resp, IEnumerable<ILineItem> items)
        {
            foreach (var item in items)
            {
                resp.UpdatedItems.Add(
                    new UpdateQuantityResponseItem
                        {
                            Key = item.Key,
                            Quantity = item.Quantity,
                            FormattedTotal = item.TotalPrice.AsFormattedCurrency()
                        });
            }
        }
    }
}