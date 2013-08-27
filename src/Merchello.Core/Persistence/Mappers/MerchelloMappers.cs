using System;
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
    internal static class MerchelloMappers
    {
        private static readonly Dictionary<Type, Type> Mappers = new Dictionary<Type, Type>()
            {                
                { typeof(IAddress), typeof(AddressMapper) },
                { typeof(IAnonymousCustomer), typeof(AnonymousCustomerMapper) },
                { typeof(IBasket), typeof(BasketMapper)},
                { typeof(IBasketItem), typeof(BasketItemMapper) },
                { typeof(ICustomer), typeof(CustomerMapper) },
                { typeof(InvoiceStatus), typeof(InvoiceStatusMapper) } 
            };


        internal static Attempt<BaseMapper> ResolveByType(Type entityType)
        {
            var mapper = Mappers.FirstOrDefault(x => x.Key == entityType).Value;

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
                LogHelper.Error(typeof(MerchelloMappers), "Could not instantiate mapper of type " + mapper, ex);
                return new Attempt<BaseMapper>(ex);
            }


        }

    }


}
