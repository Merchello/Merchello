namespace Merchello.Web.Store.Models
{
    using Merchello.Web.Models.Ui;

    /// <summary>
    /// Represents a model to render an Item Cache (basket or wish list) in the UI.
    /// </summary>
    public class StoreItemCacheModel : IItemCacheModel<StoreLineItemModel>
    {
        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        public StoreLineItemModel[] Items { get; set; }
    }
}