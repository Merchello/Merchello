using System;
using Umbraco.Core;

namespace Merchello.Core
{
    /// <summary>
    /// Helper methods for Activation
    /// </summary>
	internal static class ActivatorHelper
	{
		/// <summary>
		/// Creates an instance of a type using that type's default constructor.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static T CreateInstance<T>() where T : class, new()
		{
			return Activator.CreateInstance(typeof(T)) as T;
		}

        /// <summary>
        /// Creates an instance of a type using a constructor with specific arguments
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="ctrArgs"></param>
        /// <param name="ctrValues"></param>
        /// <returns></returns>
        /// <remarks>
        /// http://stackoverflow.com/questions/1288310/activator-createinstance-how-to-create-instances-of-classes-that-have-paramete
        /// </remarks>
        public static T CreateInstance<T>(Type type, Type[] ctrArgs, object[] ctrValues)
        {            
            Mandate.ParameterNotNull(ctrArgs, "ctrArgs");
            Mandate.ParameterNotNull(ctrValues, "ctrValues");
            
            var constructor = type.GetConstructor(ctrArgs);
            return (T)constructor.Invoke(ctrValues);
        }
	}
}
