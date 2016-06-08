namespace Merchello.Web.Models.Ui
{
    /// <summary>
    /// Defines a checkout discount model.
    /// </summary>
    /// <typeparam name="TLineItemModel">
    /// The type of <see cref="ILineItemModel"/>
    /// </typeparam>
    public interface ICheckoutDiscountModel<TLineItemModel> : IUiModel
        where TLineItemModel : class, ILineItemModel, new()
    {
        /// <summary>
        /// Gets or sets the offer code.
        /// </summary>
        string OfferCode { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IDiscountViewData{TLineItemModel}"/>.
        /// </summary>
        IDiscountViewData<TLineItemModel> ViewData { get; set; }
    }
}