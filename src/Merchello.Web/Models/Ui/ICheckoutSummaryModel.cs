namespace Merchello.Web.Models.Ui
{
    public interface ICheckoutSummaryModel<TBillingAddress, TShippingAddress, TLineItem> : IUiModel
        where TBillingAddress : class, ICheckoutAddressModel, new()
        where TShippingAddress : class, ICheckoutAddressModel, new()
        where TLineItem : class, ILineItemModel, new()
    {

    }
}