using Merchello.Core.Models.Interfaces;

namespace Merchello.Core.Persistence.Mappers
{
    using System;
    using System.Collections.Concurrent;

    using Merchello.Core.Models;

    using Umbraco.Core;
    using Umbraco.Core.Logging;

    /// <summary>
    /// Provides a mapping by type between domain objects and their respective mapper classes.
    /// </summary>
    /// <remarks>
    /// This class basically short circuits the methodology Umbraco uses in it's MapperResolver implementation
    /// and allows us to reduce the number of internal classes that we need to copy into the Merchello core.
    /// </remarks>
    public class MerchelloMapper
    {
        /// <summary>
        /// The cache of mappers.
        /// </summary>
        private static readonly ConcurrentDictionary<Type, Type> MapperCache = new ConcurrentDictionary<Type, Type>();

        /// <summary>
        /// Lazy access to the Mapper.
        /// </summary>
        private static readonly Lazy<MerchelloMapper> Mapper = new Lazy<MerchelloMapper>(() => new MerchelloMapper());
 
        /// <summary>
        /// Gets the singleton accessor
        /// </summary>
        public static MerchelloMapper Current
        {
            get
            {
                return Mapper.Value;
            }
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="MerchelloMapper"/> class from being created.
        /// </summary>
        private MerchelloMapper()
        {
            CacheMapper(typeof(IAuditLog), typeof(AuditLogMapper));
            CacheMapper(typeof(ITaxMethod), typeof(TaxMethodMapper));
            CacheMapper(typeof(ICustomerAddress), typeof(CustomerAddressMapper));
            CacheMapper(typeof(IAnonymousCustomer), typeof(AnonymousCustomerMapper));
            CacheMapper(typeof(IItemCache), typeof(ItemCacheMapper));
            CacheMapper(typeof(IItemCacheLineItem), typeof(ItemCacheLineItemMapper));
            CacheMapper(typeof(ICustomer), typeof(CustomerMapper));
            CacheMapper(typeof(ICatalogInventory), typeof(CatalogInventoryMapper));
            CacheMapper(typeof(IGatewayProviderSettings), typeof(GatewayProviderMapper));
            CacheMapper(typeof(IInvoice), typeof(InvoiceMapper));
            CacheMapper(typeof(IInvoiceLineItem), typeof(InvoiceLineItemMapper));
            CacheMapper(typeof(IInvoiceStatus), typeof(InvoiceStatusMapper));
            CacheMapper(typeof(IOrder), typeof(OrderMapper));
            CacheMapper(typeof(IOrderLineItem), typeof(OrderLineItemMapper));
            CacheMapper(typeof(INotificationMessage), typeof(NotificationMessageMapper));
            CacheMapper(typeof(INotificationMethod), typeof(NotificationMethodMapper));            
            CacheMapper(typeof(IPayment), typeof(PaymentMapper));
            CacheMapper(typeof(IPaymentMethod), typeof(PaymentMethodMapper));
            CacheMapper(typeof(IProduct), typeof(ProductMapper));
            CacheMapper(typeof(IProductVariant), typeof(ProductVariantMapper));
            CacheMapper(typeof(IProductOption), typeof(ProductOptionMapper));
            CacheMapper(typeof(IAppliedPayment), typeof(AppliedPaymentMapper));
            CacheMapper(typeof(IShipCountry), typeof(ShipCountryMapper));
            CacheMapper(typeof(IShipment), typeof(ShipmentMapper));
            CacheMapper(typeof(IShipMethod), typeof(ShipMethodMapper));
            CacheMapper(typeof(IShipmentStatus), typeof(ShipmentStatusMapper));
            CacheMapper(typeof(IShipRateTier), typeof(ShipRateTierMapper));
            CacheMapper(typeof(IStoreSetting), typeof(StoreSettingMapper));
            CacheMapper(typeof(IWarehouse), typeof(WarehouseMapper));
            CacheMapper(typeof(IWarehouseCatalog), typeof(WarehouseCatalogMapper));
        }



        /// <summary>
        /// Adds a key value pair to the dictionary
        /// </summary>
        /// <param name="key"></param>
        /// <param name="mapper"></param>
        private static void CacheMapper(Type key, Type mapper)
        {
            MapperCache.AddOrUpdate(key, mapper, (x, y) => mapper);
        }
        

        internal Attempt<BaseMapper> ResolveByType(Type entityType)
        {
            //var mapper = Mappers.FirstOrDefault(x => x.Key == entityType).Value;
            Type mapper;
            MapperCache.TryGetValue(entityType, out  mapper);

            if (mapper == null)
            {
                return Attempt<BaseMapper>.Fail();
            }
            try
            {
                var instance = Activator.CreateInstance(mapper) as BaseMapper;
                return instance != null
                    ? Attempt<BaseMapper>.Succeed(instance) //(true, instance)
                    : Attempt<BaseMapper>.Fail();
            }
            catch (Exception ex)
            {
                LogHelper.Error(typeof(MerchelloMapper), "Could not instantiate mapper of type " + mapper, ex);
                return Attempt<BaseMapper>.Fail(ex);
            }


        }

    }


}
