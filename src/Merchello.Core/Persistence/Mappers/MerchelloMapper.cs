﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Models;
using Umbraco.Core;
using Umbraco.Core.Logging;

namespace Merchello.Core.Persistence.Mappers
{
    /// <summary>
    /// Provides a mapping by type between domain objects and their respective mapper classes.
    /// </summary>
    /// <remarks>
    /// This class basically short circuits the methodology Umbraco uses in it's MapperResolver implementation
    /// and allows us to reduce the number of internal classes that we need to copy into the Merchello core.
    /// </remarks>
    public class MerchelloMapper
    {
        private static readonly ConcurrentDictionary<Type, Type> MapperCache = new ConcurrentDictionary<Type, Type>();
        private static readonly Lazy<MerchelloMapper> Mapper = new Lazy<MerchelloMapper>(() => new MerchelloMapper());
 
        /// <summary>
        /// Singleton accessor
        /// </summary>
        public static MerchelloMapper Current { get { return Mapper.Value;} }

        private MerchelloMapper()
        {
            CacheMapper(typeof(ICustomerAddress), typeof(AddressMapper));
            CacheMapper(typeof(IAnonymousCustomer), typeof(AnonymousCustomerMapper));
            CacheMapper(typeof(IItemCache), typeof(CustomerItemCacheMapper));            
            CacheMapper(typeof(IItemCacheLineItem), typeof(CustomerItemCacheItemMapper));
            CacheMapper(typeof(ICustomer), typeof(CustomerMapper));
            CacheMapper(typeof(IInvoice), typeof(InvoiceMapper));
            CacheMapper(typeof(IInvoiceLineItem), typeof(InvoiceLineItemMapper));
            CacheMapper(typeof(IInvoiceStatus), typeof(InvoiceStatusMapper));
            CacheMapper(typeof(IOrderLineItem), typeof(OrderLineItemMapper));
            CacheMapper(typeof(IPayment), typeof(PaymentMapper));
            CacheMapper(typeof(IProduct), typeof(ProductMapper));
            CacheMapper(typeof(IProductVariant), typeof(ProductVariantMapper));
            CacheMapper(typeof(IProductOption), typeof(ProductOptionMapper));
            CacheMapper(typeof(IAppliedPayment), typeof(AppliedPaymentMapper));
            CacheMapper(typeof(IShipment), typeof(ShipmentMapper));
            CacheMapper(typeof(IShipMethod), typeof (ShipMethodMapper));
            CacheMapper(typeof(IGatewayProvider), typeof(RegisteredGatewayProviderMapper));
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
        
        /// <summary>
        /// Returns a list of entities that use guids as their primary keys
        /// </summary>
        private static readonly IEnumerable<Type> KeyedTypes = new List<Type>()
            {
                { typeof(ICustomer) },
                { typeof(IAnonymousCustomer) },
                { typeof(IProduct) },
                { typeof(IProductVariant) }
            };

        /// <summary>
        /// Returns True/false indicating whether or not the type was registered as a KeyBasedType
        /// </summary>
        internal static bool IsKeyBasedType(Type type)
        {
            return KeyedTypes.Contains(type);
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
