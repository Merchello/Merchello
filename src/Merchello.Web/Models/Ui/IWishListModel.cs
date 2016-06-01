namespace Merchello.Web.Models.Ui
{
    public interface IWishListModel<TLineItemModel> : IUiModel
        where TLineItemModel : class, ILineItemModel, new()
    {
        /// <summary>
        /// Gets or sets the basket items.
        /// </summary>
        TLineItemModel[] Items { get; set; }
    }
}