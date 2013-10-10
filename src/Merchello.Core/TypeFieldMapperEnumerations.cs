namespace Merchello.Core
{
    public enum AddressType
    {
        Residential,
        Commercial,
        Custom
    }

    public enum CustomerItemCacheType
    {
        Basket,
        Wishlist,
        Custom
    }

    public enum LineItemType
    {
        Product,
        Customer
    }

    public enum InvoiceItemType
    {
        Product,
        Charge,
        Shipping,
        Tax,
        Credit,
        Custom
    }

    public enum PaymentMethodType
    {
        Cash,
        CreditCard,
        PurchaseOrder,
        Custom
    }

   

    public enum AppliedPaymentType
    {
        Credit,
        Debit,
        Void,
        Custom
    }



    internal enum ProductType
    {
        Custom
    }

    public enum ShipMethodType
    {
        FlatRate,
        PercentTotal,
        Carrier,
        Custom
    }

    public enum GatewayProviderType
    {
        Shipping,
        Payment,
        Taxation,
        Custom
    }
}