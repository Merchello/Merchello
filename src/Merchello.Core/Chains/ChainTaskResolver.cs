using System;
using System.Collections.Generic;
using Merchello.Core.Configuration;
using System.Linq;

namespace Merchello.Core.Chains
{
    /// <summary>
    /// Utility class to resolve chain tasks
    /// </summary>
    internal class ChainTaskResolver
    {
        /// <summary>
        /// Resolves a chain of tasks, where the tasks do not require have parameters in the constructor
        /// </summary>
        /// <typeparam name="T">The type of the task</typeparam>
        /// <param name="chainAlias">The 'configuration' alias of the chain.  This is the merchello.config value</param>
        /// <returns>A collection of instantiated of AttemptChainTask</returns>
        internal static IEnumerable<T> ResolveAttemptChainByAlias<T>(string chainAlias) where T : class, IAttemptChainTask<T>, new()
        {
            var types = new List<T>();
            var typeList = GetTypesForChain(chainAlias).ToArray();
            if (!typeList.Any()) return types;

            types.AddRange(typeList.Select(type => ActivatorHelper.CreateInstance<T>()));

            return types;
        }

        /// <summary>
        /// Resolves a chain of task, where task require parameters in the constructor
        /// </summary>
        /// <typeparam name="T">The type of the task</typeparam>
        /// <param name="chainAlias">The 'configuration' alias of the chain.  This is the merchello.config value</param>
        /// <param name="ctrArgs"></param>
        /// <param name="ctrValues"></param>
        /// <returns>A collection of instantiated of AttemptChainTask</returns>
        internal static IEnumerable<T> ResolveAttemptChainByAlias<T>(string chainAlias, Type[] ctrArgs, object[] ctrValues) where T : class, IAttemptChainTask<T>, new()
        {
            var types = new List<T>();
            var typeList = GetTypesForChain(chainAlias).ToArray();
            if (!typeList.Any()) return types;


            types.AddRange(typeList.Select(type => ActivatorHelper.CreateInstance<T>(Type.GetType(type), ctrArgs, ctrValues)));
            return types;
        }

        /// <summary>
        /// Gets a list of types from the merchello.config file
        /// </summary>
        /// <param name="chainAlias">The 'configuration' alias of the chain.  This is the merchello.config value</param>
        /// <returns></returns>
        internal static IEnumerable<string> GetTypesForChain(string chainAlias)
        {
            var config = MerchelloConfiguration.Current.GetTaskChainElement(chainAlias);

            return config == null ? 
                new List<string>() : 
                config.TaskConfigurationCollection.GetAllTypes().Select(x => x.Type);
        }
    }
}