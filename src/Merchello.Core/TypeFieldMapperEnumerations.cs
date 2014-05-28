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
        Denied,
        Refund,
        Custom
    }

    public enum EntityType
    {
        Customer,
        GatewayProvider,
        Invoice,
        ItemCache,
        Order,
        Payment,
        Product,
        Shipment,
        Warehouse,
        WarehouseCatalog,
        Custom
    }

    internal enum ProductType
    {
        Custom
    }


    public enum GatewayProviderType
    {
        Payment,
        Notification,
        Shipping,        
        Taxation,
        Custom
    }
}