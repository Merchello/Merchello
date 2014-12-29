namespace Merchello.Core.Services
{
    /// <summary>
    /// Defines the ServiceContext, which provides access to the following services:
    /// <see cref="ICustomerService"/>
    /// </summary>
    public interface IServiceContext
    {
        /// <summary>
        /// Gets the <see cref="IAuditLogService"/>
        /// </summary>
        IAuditLogService AuditLogService { get; }

        /// <summary>
        /// Gets the <see cref="ICustomerService"/>
        /// </summary>
        ICustomerService CustomerService { get; }

        /// <summary>
        /// Gets the <see cref="IGatewayProviderService"/>
        /// </summary>
        IGatewayProviderService GatewayProviderService { get; }

        /// <summary>
        /// Gets the <see cref="IInvoiceService"/>
        /// </summary>
        IInvoiceService InvoiceService { get; }

        /// <summary>
        /// Gets the <see cref="ItemCacheService"/>
        /// </summary>
        IItemCacheService ItemCacheService { get; }

        /// <summary>
        /// Gets the <see cref="IOrderService"/>
        /// </summary>
        IOrderService OrderService { get; }

        /// <summary>
        /// Gets the <see cref="IPaymentService"/>
        /// </summary>
        IPaymentService PaymentService { get; }

        /// <summary>
        /// Gets the <see cref="IProductService"/>
        /// </summary>
        IProductService ProductService { get; }
        
        /// <summary>
        /// Gets the <see cref="IProductVariantService"/>
        /// </summary>
        IProductVariantService ProductVariantService { get; }

        ///// <summary>
        ///// Gets the <see cref="IShipCountryService"/>
        ///// </summary>
        //IShipCountryService ShipCountryService { get; }

        /// <summary>
        /// Gets the <see cref="IStoreSettingService"/>
        /// </summary>
        IStoreSettingService StoreSettingService { get; }

        /// <summary>
        /// Gets the <see cref="IShipmentService"/>
        /// </summary>
        IShipmentService ShipmentService { get; }

        /// <summary>
        /// Gets the <see cref="IWarehouseService"/>
        /// </summary>
        IWarehouseService WarehouseService { get; }

    }
    
}
