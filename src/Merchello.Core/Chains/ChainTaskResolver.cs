using System;
using System.Collections.Generic;
using Merchello.Core.Configuration;
using System.Linq;

namespace Merchello.Core.Chains
{
    internal class ChainTaskResolver
    {

        internal static IEnumerable<T> ResolveAttemptChainByAlias<T>(string chainAlias) where T : class, IAttemptChainTask<T>, new()
        {
            var types = new List<T>();
            var typeList = GetTypesForChain(chainAlias).ToArray();
            if (!typeList.Any()) return types;

            types.AddRange(typeList.Select(type => ActivatorHelper.CreateInstance<T>()));

            return types;
        }

        internal static IEnumerable<T> ResolveAttemptChainByAlias<T>(string chainAlias, Type[] ctrArgs, object[] ctrValues) where T : class, IAttemptChainTask<T>, new()
        {
            var types = new List<T>();
            var typeList = GetTypesForChain(chainAlias).ToArray();
            if (!typeList.Any()) return types;


            types.AddRange(typeList.Select(type => ActivatorHelper.CreateInstance<T>(Type.GetType(type), ctrArgs, ctrValues)));
            return types;
        }

        internal static IEnumerable<string> GetTypesForChain(string chainAlias)
        {
            var config = MerchelloConfiguration.Current.GetTaskChainElement(chainAlias);

            return config == null ? 
                new List<string>() : 
                config.TaskConfigurationCollection.GetAllTypes().Select(x => x.Type);
        }
    }
}