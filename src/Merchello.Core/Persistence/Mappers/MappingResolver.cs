﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.ObjectResolution;

using Merchello.Core.ObjectResolution;

namespace Merchello.Core.Persistence.Mappers
{
    internal class MappingResolver : LazyManyObjectsResolverBase<MappingResolver, BaseMapper>
    {
        /// <summary>
        /// Constructor accepting a list of BaseMapper types that are attributed with the MapperFor attribute
        /// </summary>
        /// <param name="assignedMapperTypes"></param>
        public MappingResolver(Func<IEnumerable<Type>> assignedMapperTypes)
            : base(assignedMapperTypes)
        {
            
        }

        /// <summary>
        /// Caches the type -> mapper so that we don't have to type check each time we want one or lookup the attribute
        /// </summary>
        private readonly ConcurrentDictionary<Type, BaseMapper> _mapperCache = new ConcurrentDictionary<Type, BaseMapper>();

        /// <summary>
        /// Return a mapper by type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal BaseMapper ResolveMapperByType(Type type)
        {
            return _mapperCache.GetOrAdd(type, type1 =>
                {
                    
                    //first check if we can resolve it by attribute

                    var byAttribute = TryGetMapperByAttribute(type);
                    if (byAttribute.Success)
                    {
                        return byAttribute.Result;
                    }

                    //static mapper registration if not using attributes, could be something like this:
                    //if (type == typeof (UserType))
                    //    return new UserTypeMapper();

                    throw new Exception("Invalid Type: A Mapper could not be resolved based on the passed in Type");
                });
        }

        /// <summary>
        /// Check the entity type to see if it has a mapper attribute assigned and try to instantiate it
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        private Attempt<BaseMapper> TryGetMapperByAttribute(Type entityType)
        {
            //get all BaseMapper types that have a MapperFor attribute:
            var assignedMapperTypes = InstanceTypes;

            //check if any of the mappers are assigned to this type
            var mapper = assignedMapperTypes.FirstOrDefault(
                x => x.GetCustomAttributes<MapperForAttribute>(false)
                      .Any(m => m.EntityType == entityType));

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
                LogHelper.Error(typeof(MappingResolver), "Could not instantiate mapper of type " + mapper, ex);
                return new Attempt<BaseMapper>(ex);
            }
        }  

        internal string GetMapping(Type type, string propertyName)
        {
            var mapper = ResolveMapperByType(type);
            var result = mapper.Map(propertyName);
            if(string.IsNullOrEmpty(result))
                throw new Exception("Invalid mapping: The passed in property name could not be mapped using the passed in type");

            return result;
        }

    }

}