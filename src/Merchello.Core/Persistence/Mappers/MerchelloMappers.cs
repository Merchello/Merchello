using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Models;
using Umbraco.Core;
using Umbraco.Core.Logging;

namespace Merchello.Core.Persistence.Mappers
{
    internal class MerchelloMappers
    {
        /// <summary>
        /// Caches the type -> mapper so that we don't have to type check each time we want one or lookup the attribute
        /// </summary>
        
        private static readonly Dictionary<Type, Type> Mappers = new Dictionary<Type, Type>()
            {
                { typeof(ICustomer), typeof(CustomerMapper) }
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
