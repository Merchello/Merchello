namespace Merchello.Core
{
    public enum AddressType
    {
        Residential,
        Commercial,
        Custom
    }

    public enum BasketType
    {
        Basket,
        Wishlist,
        Custom
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

    public enum TransactionType
    {
        Credit,
        Debit,
        Custom
    }

    public enum ProductType
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
}