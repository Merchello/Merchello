namespace Merchello.Web.Models.Ui
{
    /// <summary>
    /// A model for Item Cache Collections.
    /// </summary>
    /// <typeparam name="TLineItemModel">
    /// The type of the line items
    /// </typeparam>
    public interface IItemCacheModel<TLineItemModel> : IUiModel
        where TLineItemModel : class, ILineItemModel, new()
    {
        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        TLineItemModel[] Items { get; set; }
    }
}