using System;

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
        public static T CreateInstance<T>() where T : class
        {
            return Activator.CreateInstance(typeof (T)) as T;
        }

    }
}
