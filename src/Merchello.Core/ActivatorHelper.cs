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
        /// <param name="constructorArgs"></param>
        /// <param name="constructorValues"></param>
        /// <returns></returns>
        /// <remarks>
        /// http://stackoverflow.com/questions/1288310/activator-createinstance-how-to-create-instances-of-classes-that-have-paramete
        /// </remarks>
        public static T CreateInstance<T>(Type type, Type[] constructorArgs, object[] constructorValues)
        {            
            Mandate.ParameterNotNull(constructorArgs, "constructorParamters");
            Mandate.ParameterNotNull(constructorValues, "constructorValues");
            
            var constructor = type.GetConstructor(constructorArgs);
            return (T)constructor.Invoke(constructorValues);
        }
	}
}
