using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Models;
using Merchello.Core.Models.GatewayProviders;
using Merchello.Core.Models.TypeFields;
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
    internal class MerchelloMapper
    {
        private static readonly ConcurrentDictionary<Type, Type> MapperCache = new ConcurrentDictionary<Type, Type>();
        private static readonly Lazy<MerchelloMapper> Mapper = new Lazy<MerchelloMapper>(() => new MerchelloMapper());
 
        /// <summary>
        /// Singleton accessor
        /// </summary>
        public static MerchelloMapper Current { get { return Mapper.Value;} }

        private MerchelloMapper()
        {
            CacheMapper(typeof(IAddress), typeof(AddressMapper));
            CacheMapper(typeof(IAnonymousCustomer), typeof(AnonymousCustomerMapper));
            CacheMapper(typeof(IBasket), typeof(BasketMapper));
            CacheMapper(typeof(IBasketItem), typeof(BasketItemMapper));
            CacheMapper(typeof(ICustomer), typeof(CustomerMapper));
            CacheMapper(typeof(IInvoice), typeof(InvoiceMapper));
            CacheMapper(typeof(IInvoiceItem), typeof(InvoiceItemMapper));
            CacheMapper(typeof(IInvoiceStatus), typeof(InvoiceStatusMapper));
            CacheMapper(typeof(IPayment), typeof(PaymentMapper));
            CacheMapper(typeof(IProduct), typeof(ProductMapper));
            CacheMapper(typeof(ITransaction), typeof(TransactionMapper));
            CacheMapper(typeof(IShipment), typeof(ShipmentMapper));
            CacheMapper(typeof(IShipMethod), typeof (ShipMethodMapper));
            CacheMapper(typeof(IRegisteredGatewayProvider), typeof(RegisteredGatewayProviderMapper));
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
                { typeof(IAnonymousCustomer)},
                { typeof(IProduct) }
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
                return Attempt<BaseMapper>.False;
            }
            try
            {
                var instance = Activator.CreateInstance(mapper) as BaseMapper;
                return instance != null
                    ? new Attempt<BaseMapper>(true, instance)
                    : Attempt<BaseMapper>.False;
            }
            catch (Exception ex)
            {
                LogHelper.Error(typeof(MerchelloMapper), "Could not instantiate mapper of type " + mapper, ex);
                return new Attempt<BaseMapper>(ex);
            }


        }

    }


}
