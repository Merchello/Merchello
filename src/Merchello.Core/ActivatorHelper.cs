namespace Merchello.Core
{
    using System;
    using System.Linq;
    using System.Reflection;
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
            Mandate.ParameterNotNull(type, "type");
            Mandate.ParameterNotNull(ctrArgs, "ctrArgs");
            Mandate.ParameterNotNull(ctrValues, "ctrValues");
            
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
            Mandate.ParameterNotNullOrEmpty(typeName, "typName");
            Mandate.ParameterNotNull(constructorArgumentValues, "constructorParameterValues");

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

            var assembly = type.Assembly;
            const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var constructorArgumentTypes = constructorArgumentValues.Select(value => value.GetType()).ToList();

            var constructor = type.GetConstructor(bindingFlags, null, CallingConventions.Any, constructorArgumentTypes.ToArray(), null);

            try
            {
                return Attempt<T>.Succeed(constructor.Invoke(constructorArgumentValues) as T);
            }
            catch (Exception ex)
            {
                return Attempt<T>.Fail(ex);
            }
        }
	}
}
