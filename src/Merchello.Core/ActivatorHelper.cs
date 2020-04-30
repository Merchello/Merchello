﻿namespace Merchello.Core
{
    using System;
    using System.Linq;
    using System.Reflection;

    using Merchello.Core.Logging;

    using Umbraco.Core;

    /// <summary>
    /// Helper methods for Activation
    /// </summary>
	internal static class ActivatorHelper
	{
		/// <summary>
		/// Creates an instance of a type using that type's default constructor.
		/// </summary>
		/// <typeparam name="T">The type of instance to create</typeparam>
		/// <returns>An instantiation of T</returns>
		public static T CreateInstance<T>() where T : class, new()
		{
			return Activator.CreateInstance(typeof(T)) as T;
		}

        /// <summary>
        /// Creates an instance of a type using a constructor with specific arguments
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> or base class</typeparam>
        /// <param name="type">The <see cref="Type"/> to be instantiated</param>
        /// <param name="ctrArgs">The constructor arguments</param>
        /// <param name="ctrValues">The constructor argument values</param>
        /// <returns>An instantiated type of T</returns>
        public static T CreateInstance<T>(Type type, Type[] ctrArgs, object[] ctrValues)
        {
            Ensure.ParameterNotNull(type, "type");
            Ensure.ParameterNotNull(ctrArgs, "ctrArgs");
            Ensure.ParameterNotNull(ctrValues, "ctrValues");
            
            var constructor = type.GetConstructor(ctrArgs);
            if (constructor == null) return default(T);
            return (T)constructor.Invoke(ctrValues);
        }

        /// <summary>
        /// Creates an instance of a type using a constructor with specific arguments
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> or base class</typeparam>
        /// <param name="typeName">The TypeName information of the object to be instantiated</param>
        /// <param name="constructorArgumentValues">Object array containing constructor arguments</param>
        /// <returns>The result of the <see cref="Attempt{T}"/> to instantiate the object</returns>
        public static Attempt<T> CreateInstance<T>(string typeName, object[] constructorArgumentValues) where T : class
        {
            Ensure.ParameterNotNullOrEmpty(typeName, "typName");
            Ensure.ParameterNotNull(constructorArgumentValues, "constructorParameterValues");

            return CreateInstance<T>(Type.GetType(typeName), constructorArgumentValues);
        }

        /// <summary>
        /// Creates an instance of a type using a constructor with specific arguments
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> or base class</typeparam>
        /// <param name="type">The type of the object to be instantiated</param>
        /// <param name="constructorArgumentValues">Object array containing constructor arguments</param>
        /// <returns>The result of the <see cref="Attempt{T}"/> to instantiate the object</returns>
        public static Attempt<T> CreateInstance<T>(Type type, object[] constructorArgumentValues) where T : class
        {
            if (type == null || constructorArgumentValues == null) return Attempt<T>.Fail(new NullReferenceException("Failed to create Type due to null Type or null constructor args"));

            const BindingFlags BindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            
            var constructorArgumentTypes = constructorArgumentValues.Select(value => value.GetType()).ToList();

            var constructor = type.GetConstructor(BindingFlags, null, CallingConventions.Any, constructorArgumentTypes.ToArray(), null);

            try
            {
                var obj = constructor.Invoke(constructorArgumentValues);
                return (obj is T)
                           ? Attempt<T>.Succeed(obj as T)
                           : Attempt<T>.Fail(
                               new InvalidCastException("Object invoked was not of expected type: " + typeof(T).Name));
            }
            catch (Exception ex)
            {
                return Attempt<T>.Fail(ex);
            }
        }


        public static T CreateInstance<T>(object[] constructorArgs)
          where T : class
        {
            var type = typeof(T);

            const System.Reflection.BindingFlags BindingFlags =
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | BindingFlags.NonPublic;

            var constructorArgumentTypes =
                constructorArgs.Select(value => value.GetType()).ToList();

            var constructor = type.GetConstructor(
                BindingFlags,
                null,
                CallingConventions.Any,
                constructorArgumentTypes.ToArray(),
                null);

            try
            {
                var obj = constructor.Invoke(constructorArgs);
                if (obj is T)
                {
                    return obj as T;
                }

                throw new Exception("ActivatorHelper failed to instantiate class of type {type.Name}");
            }
            catch (Exception ex)
            {
                var logData = MultiLogger.GetBaseLoggingData();
                logData.AddCategory("Helpers");

                MultiLogHelper.Error(typeof(ActivatorHelper), "Failed to instantiate class", ex, logData);

                throw;
            }
        }

    }
}
