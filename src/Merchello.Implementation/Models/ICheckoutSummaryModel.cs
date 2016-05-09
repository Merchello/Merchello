namespace Merchello.Implementation.Models
{
    public interface ICheckoutSummaryModel<TBillingAddress, TShippingAddress, TLineItem>
        where TBillingAddress : class, ICheckoutAddressModel, new()
        where TShippingAddress : class, ICheckoutAddressModel, new()
        where TLineItem : class, ILineItemModel, new()
    {

    }
}