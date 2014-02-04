namespace Merchello.Core
{
    public enum AddressType
    {
        Shipping,
        Billing,
        Custom
    }

    public enum ItemCacheType
    {
        Basket,
        Wishlist,
        Checkout,
        Custom
    }

    public enum LineItemType
    {
        Product,
        Shipping,
        Tax,
        Discount,
        Custom
    }

    //public enum InvoiceItemType
    //{
    //    Item,
    //    Charge,
    //    Shipping,
    //    Tax,
    //    Credit,
    //    Custom
    //}

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

    //public enum ShipMethodType
    //{
    //    FlatRate,
    //    PercentTotal,
    //    Carrier,
    //    Custom
    //}

    public enum GatewayProviderType
    {
        Shipping,
        Payment,
        Taxation,
        Custom
    }
}