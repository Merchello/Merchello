namespace Merchello.Core
{
    using System.Diagnostics.CodeAnalysis;

#pragma warning disable 1591

    /// <summary>
    /// The address type type field enumeration
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1602:EnumerationItemsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public enum AddressType
    {
        Shipping,
        Billing,
        Custom
    }

    /// <summary>
    /// The item cache type.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1602:EnumerationItemsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public enum ItemCacheType
    {
        Basket,
        Backoffice,
        Wishlist,
        Checkout,
        Custom
    }

    /// <summary>
    /// The line item type.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1602:EnumerationItemsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public enum LineItemType
    {
        Product,
        Shipping,
        Tax,
        Discount,
        Custom
    }

    /// <summary>
    /// The payment method type.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1602:EnumerationItemsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public enum PaymentMethodType
    {
        Cash,
        CreditCard,
        PurchaseOrder,
        Custom
    }

    /// <summary>
    /// The applied payment type.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1602:EnumerationItemsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public enum AppliedPaymentType
    {
        Credit,
        Debit,
        Void,
        Denied,
        Refund,
        Custom
    }

    /// <summary>
    /// The entity type.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1602:EnumerationItemsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
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


    /// <summary>
    /// The gateway provider type.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1602:EnumerationItemsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public enum GatewayProviderType
    {
        Payment,
        Notification,
        Shipping,
        Taxation,
        Custom
    }

    /// <summary>
    /// The product type.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1602:EnumerationItemsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    internal enum ProductType
    {
        Custom
    }

    #pragma warning restore 1591
}